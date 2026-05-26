using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace AssemblyParasitesColonThree.JobGivers
{
    public class JobGiver_Dissolve : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (!pawn.health.hediffSet.HasHediff(DefOf_Parasite.ScramSRP_Dissolved))
            {
                Job job = JobMaker.MakeJob(DefOf_Parasite.ScramSRP_Dissolve);
                return job;
            }
            return null;
        }
    }
}
