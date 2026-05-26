using AssemblyParasitesColonThree.CompProps;
using AssemblyParasitesColonThree.Comps;
using AssemblyParasitesColonThree.Misc;
using AssemblyParasitesColonThree.Things.Projectiles;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace AssemblyParasitesColonThree.Things
{
    public class Explosion_Death_Aneurysm : ThingWithComps, IThingHolder
    {
        protected ThingOwner<Corpse> innerContainer;

        public int deathtime = 120;

        public Pawn InnerPawn => InnerCorpse.InnerPawn;

        public int spawnTick;

        public int DestructionTick => spawnTick + deathtime;

        public int TicksLeft => DestructionTick - Find.TickManager.TicksGame;
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            GenExplosion.NotifyNearbyPawnsOfDangerousExplosive(this, DefOf_Parasite.ScramSRP_ExplosionBiological, instigator:this);
            if (!respawningAfterLoad && !BeingTransportedOnGravship)
            {
                spawnTick = Find.TickManager.TicksGame;
            }
        }
        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return innerContainer;
        }
        public Explosion_Death_Aneurysm()
        {
            innerContainer = new ThingOwner<Corpse>(this, oneStackOnly: true);
        }
        public override void DynamicDrawPhaseAt(DrawPhase phase, Vector3 drawLoc, bool flip = false)
        {
            InnerCorpse.InnerPawn.DynamicDrawPhaseAt(phase, drawLoc);
        }
        public Corpse InnerCorpse
        {
            get
            {
                if (innerContainer.Count <= 0)
                {
                    Log.Error("An explosion exists without a pawn inside it! You shouldn't be seeing this unless something is wrong");
                    return null;
                }
                return innerContainer[0];
            }
            set
            {
                if (value == null)
                {
                    innerContainer.Clear();
                    Log.Error("Tried to set innerpawn to a null value");
                    return;
                }
                if (innerContainer.Count > 0)
                {
                    Log.Error("Setting InnerPawn in explosion that already has one.");
                    innerContainer.Clear();
                }
                innerContainer.TryAddOrTransfer(value);
            }
        }
        private void LaunchGibs()
        {
            if (InnerPawn.RaceProps.BloodDef != null)
            {
                FilthMaker.TryMakeFilth(Position, Map, InnerPawn.RaceProps.BloodDef, Mathf.RoundToInt(InnerPawn.BodySize * Rand.Range(2, 5)));
            }
            IEnumerable<IntVec3> potentialpositions = GenRadial.RadialCellsAround(Position, InnerPawn.BodySize * 2, true);
            List<Thing> giblets = InnerPawn.ButcherProducts(InnerPawn, Rand.Range(0.1f, 0.5f)).ToList();
            for (int i = 0; i < giblets.Count; i++) {
                Thing gib = giblets[i];
                if (gib.stackCount > 1 && giblets.Count < potentialpositions.Count()/2)
                {
                    giblets.Add(gib.SplitOff(Rand.Range(1, gib.stackCount-1)));
                }
                for (int k = 0; k < 3; k++)
                {
                    IntVec3 potpos = potentialpositions.RandomElement();
                    if (potpos.GetFirstBuilding(Map) == null && !potpos.Roofed(Map) && potpos.GetFirstItem(Map) == null)
                    {
                        Projectile_Junk giblet = (Projectile_Junk)GenSpawn.Spawn(DefOf_Parasite.ScramSRP_Giblet, Position, Map);
                        if (gib.def.thingCategories.Contains(ThingCategoryDefOf.MeatRaw) && InnerPawn.health.hediffSet.HasHediff<Hediff_Infected>())
                        {
                            Thing meat = ThingMaker.MakeThing(DefOf_Parasite.ScramSRP_InfectedFlesh);
                            meat.stackCount = gib.stackCount;
                            giblet.InnerThing = meat;
                        }
                        else
                        {
                            giblet.InnerThing = gib;
                        }
                        if (InnerPawn.RaceProps.BloodDef != null)
                        {
                            giblet.Filth = InnerPawn.RaceProps.BloodDef;
                        }
                        giblet.Launch(this, potpos, potpos, ProjectileHitFlags.All);
                        break;
                    }
                }
            }
        }
        protected override void Tick()
        {
            base.Tick();
            switch (TicksLeft)
            {
                case <= 0:
                    
                    Thing remain = ThingMaker.MakeThing(DefOf_Parasite.ScramSRP_ParasiteGoreLarge);
                    remain.TryGetComp<Comp_Remain>().internalTier = InnerPawn.TryGetComp(out Comp_DeathExplosionRemain comp) ? comp.Props.remaintier : ParasiteTier.Assimilated;
                    remain.TryGetComp<Comp_Remain>().killCredit = new KillCredit(InnerPawn, 0.8f);
                    LaunchGibs();
                    GenSpawn.Spawn(remain, Position, Map);
                    if (InnerPawn.health.hediffSet.TryGetHediff( out Hediff_Infected hediff_Infected))
                    {
                        hediff_Infected.Postkill();
                    }
                    GenExplosion.DoExplosion(Position, Map, InnerPawn.BodySize * 2, DefOf_Parasite.ScramSRP_ExplosionBiological, this);
                    InnerCorpse.Destroy();
                    Destroy();
                    break;
                case > 120:
                    if (InnerPawn.Drawer.renderer.CurAnimation != DefOf_Parasite.ScramSRP_ExplodeStart)
                    {
                        InnerPawn.Drawer.renderer.SetAnimation(DefOf_Parasite.ScramSRP_ExplodeStart);
                    }
                    break;
                case > 60:
                    if (InnerPawn.Drawer.renderer.CurAnimation != DefOf_Parasite.ScramSRP_ExplodeMiddle)
                    {
                        InnerPawn.Drawer.renderer.SetAnimation(DefOf_Parasite.ScramSRP_ExplodeMiddle);
                    }
                    break;
                case > 0:
                    if (InnerPawn.Drawer.renderer.CurAnimation != DefOf_Parasite.ScramSRP_ExplodeEnd)
                    {
                        InnerPawn.Drawer.renderer.SetAnimation(DefOf_Parasite.ScramSRP_ExplodeEnd);
                    }
                    break;
            }
        }

    }
}
