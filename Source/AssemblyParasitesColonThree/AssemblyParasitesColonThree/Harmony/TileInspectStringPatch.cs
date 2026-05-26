using AssemblyParasitesColonThree.MapRelated;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree.Harmony
{
    [HarmonyPatch(typeof(WorldInspectPane), "TileInspectString", MethodType.Getter)]
    public class TileInspectStringPatch
    {
        public static void Postfix(ref string __result, WorldInspectPane __instance)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(__result);
            stringBuilder.AppendLine();
            stringBuilder.Append(string.Format("{0}: {1}", "ScramSRP_PhaseDisplay".Translate(), ParasiteUtils.phase(Find.World.GetComponent<WorldComp_UbiquitousDevelopment>().tileInfection[Find.WorldSelector.SelectedTile])));
            __result = stringBuilder.ToString();
        }
    }
}
