using AssemblyParasitesColonThree.MapRelated;
using AssemblyParasitesColonThree.Misc;
using AssemblyParasitesColonThree.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using static HarmonyLib.Code;

namespace AssemblyParasitesColonThree.Comps
{
    public class Comp_Remain : ThingComp
    {
        public KillCredit killCredit;
        public ParasiteTier internalTier = ParasiteTier.Assimilated;
        public override Color? ForceColor()
        {
            switch (internalTier)
            {
                case ParasiteTier.Assimilated:
                    return new Color(0.5f, 0.06f, 0.12f);
                case ParasiteTier.Primitive:
                    return new Color(0.39f, 0.31f, 0.23f);
                case ParasiteTier.Adapted:
                    return new Color(0.29f, 0.29f, 0.27f);
                case ParasiteTier.Pure:
                    return new Color(0.42f, 0.43f, 0.42f);
            }
            return null;
        }
        public void Resurrect(Map map)
        {
            // Using the same filling function used in prodromal and incomplete assimilations
            List<Pawn> pawns = ParasiteUtils.FillToTotalWeight(killCredit.points, map.GetComponent<MapComp_InfectionTracker>().phaseDef.incompleteGroupMakers);
            for (int i = 0; i < pawns.Count; i++)
            {
                GenSpawn.Spawn(pawns.ElementAt(i), parent.Position, map);
                if (i != 0)
                {
                    ParasiteUtils.YeetSpawnParasite(pawns.ElementAt(i), map, parent.Position, 3);
                }
            }
            parent.Destroy();
        }
        public override string TransformLabel(string label)
        {
            switch (internalTier)
            {
                case ParasiteTier.Assimilated:
                    return "Assimilated " + base.TransformLabel(label);
                case ParasiteTier.Primitive:
                    return "Primitive " + base.TransformLabel(label);
                case ParasiteTier.Adapted:
                    return "Adapted " + base.TransformLabel(label);
                case ParasiteTier.Pure:
                    return "Pure " + base.TransformLabel(label);
            }
            return null;
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref internalTier, "internalTier", ParasiteTier.Assimilated);
            Scribe_Deep.Look(ref killCredit, "killCredit");
        }
    }
}
