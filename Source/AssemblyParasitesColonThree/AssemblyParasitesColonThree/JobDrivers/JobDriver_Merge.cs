using AssemblyParasitesColonThree.Comps;
using AssemblyParasitesColonThree.HediffComps;
using AssemblyParasitesColonThree.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace AssemblyParasitesColonThree.JobDrivers
{
    public class JobDriver_Merge : JobDriver
    {
        protected Pawn dominant => job.GetTarget(TargetIndex.A).Pawn;
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
            Toil merge = ToilMaker.MakeToil("MakeNewToils");
            merge.initAction = delegate
            {
                if (ParasiteUtils.FetchKillList(dominant, out List<KillCredit> list) && ParasiteUtils.FetchKillList(pawn, out List<KillCredit> list2))
                {
                    list.AddRange(list2);
                    list.Add(new KillCredit(pawn, 0.3f));
                    dominant.health.GetOrAddHediff(DefOf_Parasite.ScramSRP_Regen).Severity += pawn.BodySize * 0.2f;
                }
                pawn.Destroy();
            };
            yield return merge;
        }
    }
}
