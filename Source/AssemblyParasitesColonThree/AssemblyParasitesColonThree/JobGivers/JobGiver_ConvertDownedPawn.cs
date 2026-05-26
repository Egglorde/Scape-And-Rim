using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace AssemblyParasitesColonThree.JobGivers
{
    public class JobGiver_ConvertDownedPawn : ThinkNode_JobGiver
    {
        public float range = 20f;
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (FindDownedPawn(pawn, range, out Pawn victim) && Find.TickManager.TicksGame - pawn.mindState.lastHarmTick >= 1200)
            {
                return JobMaker.MakeJob(DefOf_Parasite.ScramSRP_ConvertDowned, victim);
            }
            return null;
        }
        private bool FindDownedPawn(Pawn converter, float maxDist, out Pawn victim)
        {
            Predicate<Thing> validator = delegate (Thing t)
            {
                Pawn pawn = t as Pawn;
                if (!pawn.Downed)
                {
                    return false;
                }
                if (pawn.Faction == converter.Faction)
                {
                    return false;
                }
                if (!converter.CanReserve(pawn))
                {
                    return false;
                }
                return (pawn.RaceProps.IsFlesh) ? true : false;
            };
            victim = (Pawn)GenClosest.ClosestThingReachable(converter.Position, converter.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly), maxDist, validator);
            return victim != null;
        }
    }
}
