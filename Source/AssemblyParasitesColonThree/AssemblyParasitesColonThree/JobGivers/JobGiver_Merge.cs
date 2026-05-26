using AssemblyParasitesColonThree.HediffComps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Jobs;
using Verse;
using Verse.AI;

namespace AssemblyParasitesColonThree.JobGivers
{
    public class JobGiver_Merge : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            PawnDuty duty = pawn.mindState.duty;
            if (duty != null && pawn.health.hediffSet.HasHediff(DefOf_Parasite.ScramSRP_Dissolved) && duty.focus.Pawn != null && !duty.focus.Pawn.Destroyed)
            {
                Job job = JobMaker.MakeJob(DefOf_Parasite.ScramSRP_Merge, duty.focus.Pawn);
                return job;
            }
            return null;
            //pawn.Map.mapPawns.PawnsInFaction(ParasiteUtils.OfParasiteFaction).Where(i => i.health.hediffSet.HasHediff(DefOf_Parasite.ScramSrp_Sim)).RandomElement()
        }
    }
}
