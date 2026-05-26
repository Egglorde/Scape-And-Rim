using AssemblyParasitesColonThree.CompProps;
using AssemblyParasitesColonThree.Comps;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using static HarmonyLib.Code;

namespace AssemblyParasitesColonThree
{
    public class JobGiver_Summon : ThinkNode_JobGiver
    {
        protected float targetAcquireRadius = 56f;
        public int thresholdTicks = 2500;
        public int radius;
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (!HasAttackTarget(pawn) && !HasBeenHarmedRecently(pawn))
            {
                return null; 
            }
            if (!pawn.HasComp<Comp_Summoner>())
            {
                return null;
            }
            Comp_Summoner Comp = pawn.GetComp<Comp_Summoner>();
            CompProperties_Summoner Props = Comp.Props;
            if (Comp.SlavePoints < Props.maxpoints && Comp.canSummonAgain)
            {
                Job job = JobMaker.MakeJob(DefOf_Parasite.ScramSRP_GestateNeutral);
                job.targetQueueA = new List<LocalTargetInfo>();
                for (int i = 0; i < Props.pawnsperwave && i + Comp.SlavePoints < Props.maxpoints; i++)
                {
                    if (RCellFinder.TryFindRandomCellNearWith(pawn.Position, (IntVec3 c) => !c.Fogged(pawn.Map) && c.Standable(pawn.Map) && c.DistanceTo(pawn.Position) <= radius && c.GetEdifice(pawn.Map) == null && c.GetFirstThing<Thing_BiomassParasite>(pawn.Map) == null, pawn.Map, out var result, 5, radius))
                    {
                        job.targetQueueA.Add(result);
                    }
                }
                if (job.targetQueueA == null) return null;
                Comp.lastSummonTick = Find.TickManager.TicksGame;
                return job;
            }
            return null;
        }
        protected virtual bool HasBeenHarmedRecently(Pawn pawn)
        {
            if (pawn.mindState.lastHarmTick > 0 && Find.TickManager.TicksGame < pawn.mindState.lastHarmTick + thresholdTicks)
            {
                return true;
            }
            return false;
        }
        protected virtual bool HasAttackTarget(Pawn pawn)
        {
            TargetScanFlags targetScanFlags = TargetScanFlags.NeedLOSToPawns | TargetScanFlags.NeedThreat | TargetScanFlags.NeedAutoTargetable | TargetScanFlags.NeedLOSToNonPawns | TargetScanFlags.IgnoreNonCombatants;
            if (AttackTargetFinder.BestAttackTarget(pawn, targetScanFlags, (Thing x) => x is not Pawn || (x is Pawn p && !p.IsPsychologicallyInvisible()), 0f, targetAcquireRadius, canBashDoors: true, canTakeTargetsCloserThanEffectiveMinRange: true, canBashFences: true) != null)
            {
                return true;
            }
            return false;
        }
    }
}
