using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree.HediffComps
{
    public class HediffComp_ConvertComplete : HediffComp
    {
        public override void CompPostTickInterval(ref float severityAdjustment, int delta)
        {
            if (parent.Severity >= 1)
            {
                ParasiteUtils.CompleteConversion(Pawn, Pawn.health.hediffSet.GetFirstHediff<Hediff_COTH>());
                base.Pawn.health.RemoveHediff(parent);
            }
        }
    }
}
