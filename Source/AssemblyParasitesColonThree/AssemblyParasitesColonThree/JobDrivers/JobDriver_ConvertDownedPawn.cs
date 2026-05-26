using AssemblyParasitesColonThree.Comps;
using AssemblyParasitesColonThree.HediffComps;
using AssemblyParasitesColonThree.MapRelated;
using AssemblyParasitesColonThree.Things;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace AssemblyParasitesColonThree.JobDrivers
{
    public class JobDriver_ConvertDownedPawn : JobDriver
    {
        private float workLeft = -1000f;
        protected Pawn victim => job.GetTarget(TargetIndex.A).Pawn;
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(victim, job, 1, -1, null, errorOnFailed);
        }
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
            Toil doWork = ToilMaker.MakeToil("MakeNewToils");
            doWork.initAction = delegate
            {
                workLeft = 360;
            };
            doWork.tickIntervalAction = delegate(int delta) 
            {
                workLeft -= 1;
                if (workLeft <= 0f)
                {
                    ParasiteUtils.BeginConversion(victim, true);
                    if (ParasiteUtils.FetchKillList(pawn, out List<KillCredit> list))
                    {
                        list.Add(new KillCredit(victim, 1));
                        pawn.health.GetOrAddHediff(DefOf_Parasite.ScramSRP_Regen).Severity += pawn.BodySize;
                    }
                    pawn.Map.GetComponent<MapComp_InfectionTracker>().GainPoints(6);
                    ReadyForNextToil();
                }
            };
            doWork.defaultCompleteMode = ToilCompleteMode.Never;
            doWork.WithEffect(DefOf_Parasite.ScramSRP_AssimilationEffecter, TargetIndex.A);
            doWork.WithProgressBar(TargetIndex.A, () => workLeft, interpolateBetweenActorAndTarget: false, 0f);
            doWork.FailOnMobile(TargetIndex.A);
            doWork.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            doWork.FailOnNotDowned(TargetIndex.A);
            yield return doWork;
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref workLeft, "workLeft", 0f);
        }
    }
}
