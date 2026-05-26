using AssemblyParasitesColonThree.MapRelated;
using LudeonTK;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree.Misc
{
    public class DebugActions
    {
        [DebugAction("General", null, false, false, false, false, false, 0, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void ParasiteDiagnostic()
        {
            WorldComp_UbiquitousDevelopment comp = Find.World.GetComponent<WorldComp_UbiquitousDevelopment>();
            MapComp_InfectionTracker mapcomp = Find.CurrentMap.GetComponent<MapComp_InfectionTracker>();
            string message = "Points: " + mapcomp.points
                + " DeltaPoints: " + mapcomp.deltaPoints
                + " PointsAtTile: " + comp.tileInfection[mapcomp.map.Tile]
                + "\nPhase: " + mapcomp.phaseDef?.defName
                + "\nMaxParasites: " + mapcomp.phaseDef?.maxcount
                + "\nMinParasites: " + mapcomp.phaseDef?.mincount
                +"\nCurparasites: " + mapcomp.parasitesOnMap
                + "\nCurUbiq: " + comp.UbiquitousDevelopmentLevel
                + "\nCurUbiq: " + comp.ubiqDev; ;
            Log.Message(message);
            Messages.Message(message, MessageTypeDefOf.TaskCompletion, historical: false);
        }
        [DebugAction("General", null, false, false, false, false, false, 0, false, allowedGameStates = AllowedGameStates.WorldRenderedNow)]
        private static void TestUbiq()
        {
            WorldComp_UbiquitousDevelopment worldcomp = Find.World.GetComponent<WorldComp_UbiquitousDevelopment>();
            worldcomp.RunFullSpread();
            foreach (WorldObject_Vector infectionVector in worldcomp.InfectionVectors())
            {
                Log.Message(infectionVector.Radius);
            }
        }
        [DebugAction("General", null, false, false, false, false, false, 0, false, allowedGameStates = AllowedGameStates.WorldRenderedNow)]
        private static void ClenseUbiq()
        {
            WorldComp_UbiquitousDevelopment worldcomp = Find.World.GetComponent<WorldComp_UbiquitousDevelopment>();
            Array.Clear(worldcomp.tileInfection, 0, worldcomp.tileInfection.Length);
            WorldDrawLayer_Phase phaseDrawer = (WorldDrawLayer_Phase)Find.World.grid.Surface.WorldDrawLayers.Find(w => w is WorldDrawLayer_Phase);
            phaseDrawer.RegenerateNow();
        }

        [DebugAction("Spawning", null, false, false, false, false, false, 0, false, allowedGameStates = AllowedGameStates.PlayingOnMap, displayPriority = 1000)]
        private static List<DebugActionNode> SpawnSimmerPawn()
        {
            List<DebugActionNode> list = new List<DebugActionNode>();
            foreach (InfectedFormTypeDef item in from iftd in DefDatabase<InfectedFormTypeDef>.AllDefs
                                                 orderby iftd.defName
                                                 select iftd)
            {
                foreach (PawnKindDef item2 in from kd in DefDatabase<PawnKindDef>.AllDefs
                                              where kd.showInDebugSpawner && kd.defaultFactionDef != DefOf_Parasite.ParasiteFaction
                                              orderby kd.defName
                                              select kd)
                {
                    PawnKindDef localKindDef = item2;
                    list.Add(new DebugActionNode(item.namePrefix+" "+localKindDef.defName, DebugActionType.ToolMap)
                    {
                        category = DebugToolsSpawning.GetCategoryForPawnKind(localKindDef),
                        action = delegate
                        {
                            Faction faction = FactionUtility.DefaultFactionFrom(localKindDef.defaultFactionDef);
                            Pawn pawn = PawnGenerator.GeneratePawn(localKindDef, faction, Find.CurrentMap.Tile);
                            GenSpawn.Spawn(pawn, UI.MouseCell(), Find.CurrentMap);
                            pawn.equipment?.DestroyAllEquipment();
                            ParasiteUtils.Turn(pawn, item, false);
                            pawn.Rotation = Rot4.South;
                        }
                    });
                }

            }
            return list;
        }
    }
}
