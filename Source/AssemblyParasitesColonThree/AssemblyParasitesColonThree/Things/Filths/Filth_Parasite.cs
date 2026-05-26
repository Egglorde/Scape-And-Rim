using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree.Things.Filths
{
    public class Filth_Parasite : Filth
    {
        public override void TickLong()
        {
            if (Spawned)
            {
                if (0.0016f >= Rand.Range(0.0f, 1.0f))
                {
                    GenSpawn.Spawn(ThingMaker.MakeThing(DefOf_Parasite.TunnelBeckonSpawner), Position, Map, WipeMode.VanishOrMoveAside);
                    Destroy();
                }
                if (Position.GetTerrain(Map) == DefOf_Parasite.ScramSRP_InfestedSoil &&
                    GenClosest.ClosestThing_Global(Position, Map.mapPawns.SpawnedPawnsInFaction(ParasiteUtils.OfParasiteFaction), maxDistance: 9, validator: (Thing b) => b.HasComp<Comp_BeckonSpreadInfection>()) == null &&
                    spawnedTick + 100 <= Find.TickManager.TicksGame)
                {
                    Map.terrainGrid.SetTerrain(Position, TerrainDefOf.Soil);
                    foreach (IntVec3 item in GenAdj.CellsAdjacentCardinal(Position, Rot4.North, IntVec2.Zero))
                    {
                        if (item.GetTerrain(Map) == DefOf_Parasite.ScramSRP_InfestedSoil)
                        {
                            FilthMaker.TryMakeFilth(item, Map, DefOf_Parasite.ScramSRP_Filth_ResidueInfested);
                        }
                    }
                }
                if (Position.GetFirstPawn(Map) != null)
                {
                    ParasiteUtils.TryTransmit(Position.GetFirstPawn(Map), this, false, 0.2f, "ScramSRP_Infect_Residue");
                }
            }
        }
    }
}
