using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree
{
    public class GroundSpawner_Beckon : GroundSpawner
    {
        protected override SoundDef SustainerSound => DefOf_Parasite.TunnelBell;

        protected override void Spawn(Map map, IntVec3 loc)
        {
            Pawn Spawned = PawnGenerator.GeneratePawn(new PawnGenerationRequest(DefOf_Parasite.ScramSRP_Beckon, ParasiteUtils.OfParasiteFaction, PawnGenerationContext.NonPlayer, null, forceGenerateNewPawn: true, allowDead: false, allowDowned: true, canGeneratePawnRelations: false, mustBeCapableOfViolence: false, forceNoGear: true, allowPregnant: false));
            GenSpawn.Spawn(Spawned, loc, map, WipeMode.VanishOrMoveAside);
            map.terrainGrid.SetTerrain(loc, DefOf_Parasite.ScramSRP_InfestedSoil);
        }
    }
}
