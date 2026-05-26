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
    public class JobDriver_Dissolve : JobDriver
    {
        private float workLeft = -1000;
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }
        protected override IEnumerable<Toil> MakeNewToils()
        {
            Toil dissolve = ToilMaker.MakeToil("MakeNewToils");
            dissolve.initAction = delegate
            {
                workLeft = 180;
            };
            dissolve.tickIntervalAction = delegate (int delta)
            {
                workLeft -= 1;
                if (workLeft < 0)
                {
                    Hediff converthediff = HediffMaker.MakeHediff(DefOf_Parasite.ScramSRP_Dissolved, pawn);
                    pawn.health.AddHediff(converthediff, null);
                    pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
                }
            };
            dissolve.defaultCompleteMode = ToilCompleteMode.Never;
            dissolve.WithEffect(DefOf_Parasite.ScramSRP_AssimilationEffecter, TargetIndex.A);
            yield return dissolve;
        }
    }
}
