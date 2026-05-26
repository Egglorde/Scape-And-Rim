using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace AssemblyParasitesColonThree.JobGivers
{
    public class JobGiver_ConsumeCorpse : ThinkNode_JobGiver
    {
        public float range = 20f;
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (FindDeadPawn(pawn, range, out Thing victim) && Find.TickManager.TicksGame - pawn.mindState.lastHarmTick >= 1200)
            {
                return JobMaker.MakeJob(DefOf_Parasite.ScramSRP_ConsumeDead, victim);
            }
            return null;
        }
        private bool FindDeadPawn(Pawn converter, float maxDist, out Thing victim)
        {
            Predicate<Thing> validator = delegate (Thing t)
            {
                Corpse corpse = t as Corpse;
                if (corpse.Faction == converter.Faction)
                {
                    return false;
                }
                if (!converter.CanReserve(corpse))
                {
                    return false;
                }
                return (corpse.InnerPawn.RaceProps.IsFlesh) ? true : false;
            };
            victim = GenClosest.ClosestThingReachable(converter.Position, converter.Map, ThingRequest.ForGroup(ThingRequestGroup.Corpse), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly), maxDist, validator);
            return victim != null;
        }
    }
}
