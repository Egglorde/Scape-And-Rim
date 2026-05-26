using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;

namespace AssemblyParasitesColonThree
{
    public class DeathActionWorker_PopIntoParas : DeathActionWorker
    {
        public DeathActionProperties_PopIntoParas Props => (DeathActionProperties_PopIntoParas)props;

        public override void PawnDied(Corpse corpse, Lord prevLord)
        {
            Pawn innerPawn = corpse.InnerPawn;
            if (innerPawn == null)
            {
                return;
            }
            int dividePawnCount = Props.dividePawnCount; 
            for (int i = 0; i < dividePawnCount; i++)
            {
                PawnKindDef kind = Props.carrierpawnkindoptions.RandomElement();
                Faction faction = corpse.InnerPawn.Faction;
                float? fixedBiologicalAge = 0f;
                Pawn child = PawnGenerator.GeneratePawn(new PawnGenerationRequest(kind, faction, PawnGenerationContext.NonPlayer, null, forceGenerateNewPawn: true, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, fixedBiologicalAge));
                SpawnPawn(child, innerPawn, corpse.PositionHeld, corpse.MapHeld, prevLord);
            }
            foreach (PawnKindDef item in Props.dividePawnKindAdditionalForced)
            {
                Faction faction2 = corpse.InnerPawn.Faction;
                float? fixedBiologicalAge = 0f;
                Pawn child2 = PawnGenerator.GeneratePawn(new PawnGenerationRequest(item, faction2, PawnGenerationContext.NonPlayer, null, forceGenerateNewPawn: true, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, fixedBiologicalAge));
                SpawnPawn(child2, innerPawn, corpse.PositionHeld, corpse.MapHeld, prevLord);
            }
            ParasiteUtils.SpawnInfectorGas(corpse.PositionHeld, corpse.MapHeld, 10);
            if (!Props.leaveCorpse)
            {
                corpse.Destroy();
            }
        }
        private void SpawnPawn(Pawn child, Pawn parent, IntVec3 position, Map map, Lord lord)
        {
            GenSpawn.Spawn(child, position, map, WipeMode.VanishOrMoveAside);
            lord?.AddPawn(child);
            CompInspectStringEmergence compInspectStringEmergence = child.TryGetComp<CompInspectStringEmergence>();
            if (compInspectStringEmergence != null)
            {
                compInspectStringEmergence.sourcePawn = parent;
            }
            ParasiteUtils.YeetSpawnParasite(child, map, position, Props.popRange);
        }
    }
}
