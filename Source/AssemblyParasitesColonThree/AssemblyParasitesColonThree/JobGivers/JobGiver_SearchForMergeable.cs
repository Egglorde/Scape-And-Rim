using AssemblyParasitesColonThree.HediffComps;
using AssemblyParasitesColonThree.LordRelated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace AssemblyParasitesColonThree.JobGivers
{
    public class JobGiver_SearchForMergeable : ThinkNode_JobGiver
    {
        private float killCountTotal = -1000;
        protected override Job TryGiveJob(Pawn pawn)
        {
            IEnumerable<Pawn> tempMergeResult = SearchForMergeable(pawn);
            killCountTotal = pawn.health.hediffSet.GetHediffComps<HediffComp_KillTracker>().First().kills;
            foreach (Pawn pawn2 in tempMergeResult)
            {
                killCountTotal += pawn2.health.hediffSet.GetHediffComps<HediffComp_KillTracker>().First().kills;
            }
            if (killCountTotal > 9)
            {
                Lord lord = LordMaker.MakeNewLord(ParasiteUtils.OfParasiteFaction, new LordJob_Merge(tempMergeResult.RandomElementByWeight((Pawn p) => p.health.hediffSet.GetHediffComps<HediffComp_KillTracker>().First().kills)), pawn.Map, tempMergeResult);
            }
            return null;
            //pawn.Map.mapPawns.PawnsInFaction(ParasiteUtils.OfParasiteFaction).Where(i => i.health.hediffSet.HasHediff(DefOf_Parasite.ScramSrp_Sim)).RandomElement()
        }

        private IEnumerable<Pawn> SearchForMergeable(Pawn pawn)
        {
            IEnumerable<Pawn> tempMergeInsert = pawn.Map.mapPawns.PawnsInFaction(ParasiteUtils.OfParasiteFaction).Where((Pawn p) => p.health.hediffSet.HasHediff(DefOf_Parasite.ScramSrp_Sim) && p.Position.DistanceTo(pawn.Position) <= 30 && p.lord == null);
            return tempMergeInsert.TakeRandomDistinct(3);
        }
    }
}
