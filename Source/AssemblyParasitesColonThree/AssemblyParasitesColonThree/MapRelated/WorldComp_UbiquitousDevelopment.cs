using AssemblyParasitesColonThree.Comps;
using AssemblyParasitesColonThree.Misc;
using AssemblyParasitesColonThree.Misc.Makers;
using AssemblyParasitesColonThree.Things;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;
using static UnityEngine.GraphicsBuffer;

namespace AssemblyParasitesColonThree.MapRelated
{
    public class WorldComp_UbiquitousDevelopment : WorldComponent
    {
        public int[] tileInfection;
        private int[] tilePropogation;
        private List<int> tileService;
        public Dictionary<PawnKindDef, int> KindsAndInfections;
        public IEnumerable<WorldObject_Vector> InfectionVectors()
        {
            for (int i = 0; i < Find.WorldObjects.AllWorldObjectsOnLayer(worldPlanetLayer).Count; i++)
            {
                if (Find.WorldObjects.AllWorldObjectsOnLayer(worldPlanetLayer)[i] is WorldObject_Vector vector)
                {
                    yield return vector;
                }
            }
        }
        public void AssimilateKind(PawnKindDef kind)
        {
            if (kind == null) return;
            if (!KindsAndInfections.TryGetValue(kind, out int value))
            {
                KindsAndInfections.Add(kind, 1);
                return;
            }
            KindsAndInfections[kind] += 1;
            if (value == 5) Find.LetterStack.ReceiveLetter("ScramSRP_AssimWorldEventTitle".Translate(kind.label), "ScramSRP_AssimWorldEventDesc".Translate(kind.GetLabelPlural()), DefOf_Parasite.ScramSRP_AssimWorldEvent);
        }
        private void RunNaturalInfection()
        {
            Dictionary<PawnKindDef, float> infectableKinds = new Dictionary<PawnKindDef, float>();
            foreach(PawnKindDef kind in DefDatabase<PawnKindDef>.AllDefs)
            {
                if (kind.RaceProps.IsFlesh && kind.RaceProps.FleshType != DefOf_Parasite.ScramSRP_ParasiteFlesh && !KindsAndInfections.ContainsKey(kind) && kind.combatPower > 0 )
                {
                    infectableKinds.Add(kind, 1f / kind.combatPower);
                }
            }
            AssimilateKind(infectableKinds.RandomElementByWeight((KeyValuePair<PawnKindDef, float> kvp) => kvp.Value).Key);
        }
        private PlanetLayer worldPlanetLayer => world.grid.Surface;
        public float ubiqDev => (float)tileInfection.Aggregate(ParasiteUtils.phase(tileInfection[0]), (a, b) => a + ParasiteUtils.phase(b)) / worldPlanetLayer.TilesCount;
        public int UbiquitousDevelopmentLevel
        {
            get
            {
                switch (ubiqDev)
                {
                    case > 14f / 36f:
                        return 4;
                    case > 10f / 36f:
                        return 3;
                    case > 7f / 36f:
                        return 2;
                    case > 4f / 36f:
                        return 1;
                    default:
                        return 0;
                }
            }
        }
        public static List<ParasiteGroupMaker> SimGroupMakers()
        {
            // Adds the list of assimilated pawns to the potential groupMakers of the phase
            List<ParasiteGroupMaker> tmpGroupMakers = new List<ParasiteGroupMaker>();
            // Cycles through each KVP in KindsAndInfections, and adds each to the list of groupMakers
            foreach (KeyValuePair<PawnKindDef, int> kvp in Find.World.GetComponent<WorldComp_UbiquitousDevelopment>().KindsAndInfections)
            {
                if (kvp.Value < 5 && kvp.Key.combatPower > 0)
                {
                    continue;
                }
                ParasiteGroupMaker groupMaker = new ParasiteGroupMaker();
                groupMaker.weight = 5;
                groupMaker.kind = kvp.Key;
                groupMaker.pawnRange = new IntRange(3, 5);
                groupMaker.sim = true;
                tmpGroupMakers.Add(groupMaker);
            }
            return tmpGroupMakers;
        }
        public override void WorldComponentTick()
        {
            base.WorldComponentTick();
            if (GenTicks.IsTickInterval(worldPlanetLayer.GetHashCode().HashOffset(), 60000))
            {
                if (InfectionVectors().Count() < 5)
                {
                    PlaceVector();
                }
                RunFullSpread();
                for (int i = 0; i <= UbiquitousDevelopmentLevel; i++)
                {
                    RunNaturalInfection();
                }
            }
            if (tileService.Count > 0)
            {
                // Im going to start actually commenting so I understand what Im writing

                // Get the tile in question
                int infectionTile = tileService.ElementAt(0);

                // Value setting stuff, better than += bc there's no chance of overflow
                int Value = Mathf.RoundToInt(Mathf.Clamp(tileInfection[infectionTile] + tilePropogation[infectionTile], -200, 2100000000));
                tileInfection[infectionTile] = Value;

                // Update phasedrawer
                WorldDrawLayer_Phase phaseDrawer = (WorldDrawLayer_Phase)worldPlanetLayer.WorldDrawLayers.Find(w => w is WorldDrawLayer_Phase);
                phaseDrawer.Notify_TileInfectionChanged(infectionTile);

                // This function just runs through the active maps and sets their point value to be equal to the new value within the tileInfection Array
                // Cycles through the existing maps, in a for loop because I don't trust myself
                for (int i = 0; i < Find.Maps.Count; i++)
                {
                    // Map is map
                    Map map = Find.Maps[i];
                    if (map.Tile == infectionTile)
                    {
                        // If the map has a tile, it changes the point value of said map to match that of the tile
                        // Note: Always do this AFTER the spread step, or else the map will gain no points for that day.
                        map.GetComponent<MapComp_InfectionTracker>().SetPoints(Value);
                    }
                }

                // Spread the infection between tiles
                List<PlanetTile> neighbors = new List<PlanetTile>();
                worldPlanetLayer.GetTileNeighbors(infectionTile, neighbors);
                foreach (PlanetTile tile2 in neighbors)
                {
                    float ripple = tilePropogation[infectionTile] / ((world.pathGrid.PerceivedMovementDifficultyAt(tile2) / Mathf.Clamp(ParasiteUtils.phase(tileInfection[tile2]), 1, 10)) + 1);

                    // This extra check allows us to service old tiles if their propgation value is higher
                    if (ripple > tilePropogation[tile2])
                    {
                        if (ParasiteUtils.phase(tileInfection[infectionTile]) == 10)
                        {
                            AddTileServicer(tile2, ripple);
                        }
                        else if (ripple > 1)
                        {
                            AddTileServicer(tile2, ripple);
                        }
                    }
                }

                // Remove the tile we just serviced from the queue
                tileService.Remove(infectionTile);
                tileService = tileService.InRandomOrder().ToList();
            } else
            {
                Array.Clear(tilePropogation, 0, tilePropogation.Length);
            }
        }
        private void AddTileServicer(int tileID, float value)
        {
            tileService.Add(tileID);
            tilePropogation[tileID] = Mathf.RoundToInt(Mathf.Clamp(value, -2100000000, 2100000000));
        }

        public void PlaceVector()
        {
            if (TileFinder.TryFindNewSiteTile(out PlanetTile tile, layer:worldPlanetLayer, tileFinderMode:InfectionVectors().Count() > 0 ? TileFinderMode.Random : TileFinderMode.Near, maxDist: InfectionVectors().Count() > 0 ? 10000 : 20))
            {
                WorldObject target = WorldObjectMaker.MakeWorldObject(InfectionVectors().Count() > 0 ? DefOf_Parasite.ScramSRP_Vector : DefOf_Parasite.ScramSRP_Meteor);
                target.SetFaction(ParasiteUtils.OfParasiteFaction);
                target.Tile = tile;
                Find.WorldObjects.Add(target);
                if (target.def == DefOf_Parasite.ScramSRP_Meteor)
                {
                    Find.LetterStack.ReceiveLetter("ScramSRP_MeteorCrashTitle".Translate(), "ScramSRP_MeteorCrashDesc".Translate(), DefOf_Parasite.ScramSRP_NodeEvent, target);
                }
            }
        }
        public void RunFullSpread()
        {
            foreach (WorldObject_Vector vector in InfectionVectors())
            {
                AddTileServicer(vector.Tile, vector.Health * 0.15f);
                vector.Health *= 1.15f;
                vector.Radius += 1.35f;
            }
            for (int i = 0; i < Find.Maps.Count; i++)
            {
                // Map is map
                Map map = Find.Maps[i];
                if (map.Tile != null)
                {
                    AddTileServicer(map.Tile, map.GetComponent<MapComp_InfectionTracker>().deltaPoints);
                    map.GetComponent<MapComp_InfectionTracker>().deltaPoints = 0;
                }
            }
        }

        public WorldComp_UbiquitousDevelopment(World world)
            :base (world)
        {
            tileInfection = new int[world.grid.TilesCount];
            tilePropogation = new int[world.grid.TilesCount];
            tileService = new List<int>();
            KindsAndInfections = new Dictionary<PawnKindDef, int>();
        }
        public override void ExposeData()
        {
            byte[] tileInfectionBytes = new byte[world.grid.TilesCount * 4];
            byte[] tilePropogationBytes = new byte[world.grid.TilesCount * 4];

            Scribe_Collections.Look(ref tileService, "tileService", LookMode.Value);
            Scribe_Collections.Look(ref KindsAndInfections, "KindsAndInfections", LookMode.Def, LookMode.Value);

            if (Scribe.mode == LoadSaveMode.Saving)
            {
                tileInfectionBytes = DataSerializeUtility.SerializeInt(tileInfection);
                tilePropogationBytes = DataSerializeUtility.SerializeInt(tilePropogation);
            }

            DataExposeUtility.LookByteArray(ref tileInfectionBytes, "ScramSRP_TilesBytes");
            DataExposeUtility.LookByteArray(ref tilePropogationBytes, "ScramSRP_TilesBytes");

            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                tileInfection = DataSerializeUtility.DeserializeInt(tileInfectionBytes);
                tilePropogation = DataSerializeUtility.DeserializeInt(tileInfectionBytes);
            }
        }
    }
}
