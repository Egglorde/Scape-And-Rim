using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace AssemblyParasitesColonThree
{
    public class JobGiver_SummonBeckon : ThinkNode_JobGiver
    {
        public int gesttimer;
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (!pawn.HasComp<Comp_BeckonSpreadInfection>())
            {
                return null;
            }
            if (pawn.GetComp<Comp_BeckonSpreadInfection>().SlavePoints() < pawn.GetComp<Comp_BeckonSpreadInfection>().Props.maxpoints)
            {
                return JobMaker.MakeJob(DefOf_Parasite.ScramSRP_GestateNeutral);
            }
            return null;
        }
    }
}
