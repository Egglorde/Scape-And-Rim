using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree.Harmony
{
    [HarmonyPatch(typeof(Pawn_MutantTracker), "GetGizmos")]
    public class MutantErrorPatch
    {
        static bool Prefix(ref IEnumerable<Gizmo> __result)
        {
            if (ModsConfig.AnomalyActive) { return true; }
            __result = Enumerable.Empty<Gizmo>();
            return false;
        }
    }
}
