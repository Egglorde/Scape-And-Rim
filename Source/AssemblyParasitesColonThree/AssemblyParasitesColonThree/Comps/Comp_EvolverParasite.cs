using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static HarmonyLib.Code;

namespace AssemblyParasitesColonThree.Comps
{
    public class Comp_EvolverParasite : ThingComp
    {
        public void EvolvePawn(ThingWithComps pawn, PawnKindDef targetkind)
        {
            Pawn child = PawnGenerator.GeneratePawn(new PawnGenerationRequest(targetkind, ParasiteUtils.OfParasiteFaction, PawnGenerationContext.NonPlayer, null, forceGenerateNewPawn: true, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null));
            GenSpawn.Spawn(child, pawn.Position, pawn.Map, WipeMode.VanishOrMoveAside);
            pawn.Destroy();
        }
    }
}
