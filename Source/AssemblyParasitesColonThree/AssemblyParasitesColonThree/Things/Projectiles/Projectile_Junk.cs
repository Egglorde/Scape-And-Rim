using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using static UnityEngine.GraphicsBuffer;

namespace AssemblyParasitesColonThree.Things.Projectiles
{
    public class Projectile_Junk : Projectile, IThingHolder
    {
        private bool pawnWasDrafted;
        private bool pawnCanFireAtWill = true;
        protected Thing InnerThingRef;

        protected ThingOwner<Thing> innerContainer;
        public ThingDef Filth;
        public override int DamageAmount => Mathf.RoundToInt(InnerThing.GetStatValue(StatDefOf.Mass));
        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }
        public ThingOwner GetDirectlyHeldThings()
        {
            return innerContainer;
        }
        public Projectile_Junk()
        {
            innerContainer = new ThingOwner<Thing>(this);
        }
        public Thing InnerThing
        {
            get
            {
                if (innerContainer.Count <= 0)
                {
                    return null;
                }
                return innerContainer[0];
            }
            set
            {
                if (value == null)
                {
                    Log.Error("Tried to set innerpawn to a null value");
                    return;
                }
                if (value is Pawn pawn && pawn.drafter != null)
                {
                    pawnWasDrafted = pawn.Drafted;
                    pawnCanFireAtWill = pawn.drafter.FireAtWill;
                }
                if (value.Spawned)
                {
                    value.DeSpawn();
                }
                innerContainer.TryAddOrTransfer(value);
            }
        }
        public override void DynamicDrawPhaseAt(DrawPhase phase, Vector3 drawLoc, bool flip = false)
        {
            float num = def.projectile.arcHeightFactor * GenMath.InverseParabola(DistanceCoveredFractionArc);
            Vector3 vector = drawLoc + new Vector3(0f, 0f, 1f) * num;
            if (InnerThing != null)
            {
                InnerThing.DynamicDrawPhaseAt(phase, vector);
            } else
            {
                if (phase == DrawPhase.Draw)
                {
                    DrawAt(drawLoc, flip);
                }
            }
        }
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            if (!def.projectile.soundImpact.NullOrUndefined())
            {
                def.projectile.soundImpact.PlayOneShot(SoundInfo.InMap(this));
            }
            Map map = base.Map;
            IntVec3 loc = base.Position;
            InnerThingRef = InnerThing;
            base.Impact(hitThing, blockedByShield);
            if (Filth != null)
            {
                FilthMaker.TryMakeFilth(loc, map, Filth);
            }
            if (def.projectile.tryAdjacentFreeSpaces && base.Position.GetFirstBuilding(map) != null)
            {
                foreach (IntVec3 item in GenAdjFast.AdjacentCells8Way(base.Position))
                {
                    if (item.GetFirstBuilding(map) == null && item.Standable(map))
                    {
                        loc = item;
                        break;
                    }
                }
            }
            innerContainer.TryDropAll(loc, map, ThingPlaceMode.Direct);
            if (InnerThingRef is Pawn pawn)
            {
                if (pawn.drafter != null)
                {
                    pawn.drafter.Drafted = pawnWasDrafted;
                    pawn.drafter.FireAtWill = pawnCanFireAtWill;
                }
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref pawnWasDrafted, "pawnWasDrafted", defaultValue: false);
            Scribe_Values.Look(ref pawnCanFireAtWill, "pawnCanFireAtWill", defaultValue: true);
        }
    }
}
