using AssemblyParasitesColonThree.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree.Hediffs
{
    public class Hediff_AdaptationDisplay : HediffWithComps
    {
        public override string LabelInBrackets
        {
            get
            {
                if (ThingCompUtility.TryGetComp(pawn, out Comp_Adaptation comp))
                {
                    return comp.ToString();
                } else
                {
                    Log.Error("A pawn has AdaptationDisplay without needing it, removing hediff");
                    this.Severity = 0;
                    return null;
                }
            }
        }
    }
}
