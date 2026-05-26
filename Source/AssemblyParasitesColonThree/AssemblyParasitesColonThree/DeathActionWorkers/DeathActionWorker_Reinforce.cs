using AssemblyParasitesColonThree.MapRelated;
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
    public class DeathActionWorker_Reinforce : DeathActionWorker
    {
        private static List<IntVec3> tmpTakenCells = new List<IntVec3>();
        public override void PawnDied(Corpse corpse, Lord prevLord)
        {
            if (corpse.MapHeld != null)
            {
                Map map = corpse.MapHeld;
                tmpTakenCells.Clear();
                int randnum = UnityEngine.Random.Range(0, 10);
                if (RCellFinder.TryFindRandomCellNearWith(corpse.PositionHeld, (IntVec3 c) => !c.Fogged(map) && c.Standable(map) && !tmpTakenCells.Contains(c) && c.GetFirstPawn(map) == null, map, out var result, 5, 5)
                    && ParasiteUtils.phase(map.GetComponent<MapComp_InfectionTracker>().points) >= 3)
                {
                    if (randnum > 8)
                    {
                        spawnTunnel(result, map);
                    }
                }

            }

        }
        public void spawnTunnel(IntVec3 spawnCell, Map map)
        {
            GenSpawn.Spawn(ThingMaker.MakeThing(DefOf_Parasite.TunnelBeckonSpawner), spawnCell, map, WipeMode.VanishOrMoveAside);
        }
    }
}
