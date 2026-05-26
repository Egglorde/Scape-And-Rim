using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static HarmonyLib.Code;

namespace AssemblyParasitesColonThree.Hediffs
{
    public class Hediff_Regeneration : HediffWithComps
    {
        private int interval = 80;
        public override void Tick()
        {
            if (!pawn.IsHashIntervalTick(interval))
            {
                return;
            }
            for (int i = 0; i < pawn.health.hediffSet.hediffs.Count; i++)
            {
                if (pawn.health.hediffSet.hediffs[i] is Hediff_Injury injury)
                {
                    injury.Severity -= Severity*2*Rand.Range(0, Severity);
                }
            }
        }
    }
}
