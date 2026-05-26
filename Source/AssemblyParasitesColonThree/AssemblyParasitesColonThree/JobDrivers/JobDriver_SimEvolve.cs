using AssemblyParasitesColonThree.MapRelated;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.Noise;

namespace AssemblyParasitesColonThree.JobDrivers
{
    public class JobDriver_SimEvolve : JobDriver
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
                workLeft = 100;
            };
            dissolve.tickIntervalAction = delegate (int delta)
            {
                workLeft -= 1;
                if (workLeft < 0)
                {
                    Pawn child = PawnGenerator.GeneratePawn(new PawnGenerationRequest(ParasiteUtils.Primitives.RandomElement(), ParasiteUtils.OfParasiteFaction, PawnGenerationContext.NonPlayer, null, forceGenerateNewPawn: true, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, 100));
                    GenSpawn.Spawn(child, pawn.Position, pawn.Map, WipeMode.VanishOrMoveAside);
                    pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
                    pawn.Map?.GetComponent<MapComp_InfectionTracker>().GainPoints(10);
                    pawn.Destroy();
                }
            };
            dissolve.defaultCompleteMode = ToilCompleteMode.Never;
            dissolve.WithEffect(DefOf_Parasite.ScramSRP_AssimilationEffecter, TargetIndex.A);
            yield return dissolve;
        }
    }
}
