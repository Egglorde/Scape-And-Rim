using AssemblyParasitesColonThree.Comps;
using AssemblyParasitesColonThree.MapRelated;
using AssemblyParasitesColonThree.Things;
using AssemblyParasitesColonThree.HediffComps;
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
    public class JobDriver_ConsumeCorpse : JobDriver
    {
        private float workLeft = -1000f;
        protected Corpse corpse => (Corpse)job.targetA.Thing;
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(TargetIndex.A), job, 1, -1, null, errorOnFailed);
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
            doWork.tickIntervalAction = delegate (int delta)
            {
                workLeft -= 1;
                if (workLeft <= 0f)
                {
                    if (ParasiteUtils.FetchKillList(pawn, out List<KillCredit> list))
                    {
                        list.Add(new KillCredit(corpse.InnerPawn, 0.3f));
                        pawn.health.GetOrAddHediff(DefOf_Parasite.ScramSRP_Regen).Severity += corpse.InnerPawn.BodySize * 0.2f;
                    }
                    pawn.Map.GetComponent<MapComp_InfectionTracker>().GainPoints(1);
                    corpse.Destroy();
                    ReadyForNextToil();
                }
            };
            doWork.defaultCompleteMode = ToilCompleteMode.Never;
            doWork.WithEffect(DefOf_Parasite.ScramSRP_AssimilationEffecter, TargetIndex.A);
            doWork.WithProgressBar(TargetIndex.A, () => workLeft, interpolateBetweenActorAndTarget: false, 0f);
            doWork.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            yield return doWork;
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref workLeft, "workLeft", 0f);
        }
    }
}
