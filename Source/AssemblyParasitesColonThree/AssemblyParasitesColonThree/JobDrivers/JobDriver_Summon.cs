using AssemblyParasitesColonThree.CompProps;
using AssemblyParasitesColonThree.Comps;
using AssemblyParasitesColonThree.MapRelated;
using AssemblyParasitesColonThree.Misc.Makers;
using AssemblyParasitesColonThree.Things;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Noise;
using Verse.Sound;
using static HarmonyLib.Code;


namespace AssemblyParasitesColonThree
{
    public class JobDriver_Summon : JobDriver
    {
        private float workLeft = -1000f;
        private Dictionary<LocalTargetInfo, Effecter> markdict = new Dictionary<LocalTargetInfo, Effecter>();
        private Comp_Summoner Comp => pawn.GetComp<Comp_Summoner>();
        private CompProperties_Summoner Props => Comp.Props;
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }
        protected override IEnumerable<Toil> MakeNewToils()
        {
            Toil doWork = ToilMaker.MakeToil("MakeNewToils");
            doWork.initAction = delegate
            {
                workLeft = 360;
            };
            doWork.tickIntervalAction = delegate
            {
                workLeft -= 1;
                if (workLeft <= 0f)
                {
                    foreach (KeyValuePair<LocalTargetInfo, Effecter> kvp in markdict)
                    {
                        IntVec3 c = kvp.Key.Cell;
                        if (c.GetEdifice(pawn.Map) == null && c.Standable(pawn.Map) && c.GetFirstThing<Thing_BiomassParasite>(pawn.Map) == null && Props.maxpoints - Comp.SlavePoints >= 1)
                        {
                            Thing_BiomassParasite thing = ThingMaker.MakeThing(DefOf_Parasite.BiomassSmall) as Thing_BiomassParasite;
                            ParasiteGroupMaker groupMaker = Props.Spawnables.Where(p => p.points <= Props.maxpoints - Comp.SlavePoints).RandomElementByWeight(p => p.weight);
                            Pawn child = PawnGenerator.GeneratePawn(new PawnGenerationRequest(groupMaker.kind, ParasiteUtils.OfParasiteFaction, PawnGenerationContext.NonPlayer, null, forceGenerateNewPawn: true, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null));
                            if (groupMaker.sim)
                            {
                                ParasiteUtils.Turn(child, DefOf_Parasite.ScramSrp_Sim, alert: false);
                            }
                            thing.GetDirectlyHeldThings().TryAdd(child);
                            Comp.SummonerLord.AddPawn(child);
                            thing.spawner = pawn;
                            thing.gestationPeriod = 900;
                            GenPlace.TryPlaceThing(thing, c, pawn.Map, ThingPlaceMode.Near);
                        }
                        kvp.Value.Cleanup();
                    }
                    markdict.Clear();
                    job.targetQueueA.Clear();
                    ReadyForNextToil();
                } else
                {
                    foreach (LocalTargetInfo target in job.targetQueueA)
                    {
                        if (markdict.ContainsKey(target))
                        {
                            markdict[target].EffectTick(pawn, target.ToTargetInfo(pawn.Map));
                        }
                        else
                        {
                            EffecterDef effecterDef = DefOf_Parasite.ScramSRP_BiomassEffecter;
                            if (effecterDef != null)
                            {
                                markdict.Add(target, effecterDef.Spawn(target.Cell, pawn.Map));
                                markdict[target].Trigger(pawn, target.ToTargetInfo(pawn.Map));
                            }
                        }
                    }
                }
            };
            doWork.defaultCompleteMode = ToilCompleteMode.Never;
            if(Props.soundDef != null) doWork.PlaySoundAtStart(Props.soundDef);
            doWork.PlaySoundAtEnd(DefOf_Parasite.ScramSRP_AssimSound);
            yield return doWork;
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref workLeft, "workLeft", 0f);
            Scribe_Collections.Look(ref markdict, "MarkDict");
        }
    }
}
