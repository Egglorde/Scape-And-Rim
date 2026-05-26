using AssemblyParasitesColonThree.HediffComps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace AssemblyParasitesColonThree.JobGivers
{
    public class JobGiver_SimEvolve : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.health.hediffSet.GetHediffComps<HediffComp_KillTracker>().First().kills >= 9 || !pawn.health.hediffSet.HasHediff(DefOf_Parasite.ScramSRP_Dissolved)) return null;
            Job job = JobMaker.MakeJob(DefOf_Parasite.ScramSRP_SimEvolve, pawn.Map.mapPawns.PawnsInFaction(ParasiteUtils.OfParasiteFaction).Where(i => i.health.hediffSet.HasHediff(DefOf_Parasite.ScramSrp_Sim) && i != pawn).RandomElement());
            return job;
        }
    }
}
