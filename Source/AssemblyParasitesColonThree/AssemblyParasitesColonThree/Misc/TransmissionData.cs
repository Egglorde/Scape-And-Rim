using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree.Misc
{
    public class TransmissionData : IExposable
    {
        private Thing culprit;
        private Hediff_COTH hediff;
        private string desc;
        public TransmissionData() { }
        public TransmissionData(Thing Culprit, Hediff_COTH sourceHediff, string Desc = null) {
            desc = Desc;
            culprit = Culprit;
            hediff = sourceHediff;
        }
        public string Desc()
        {
            if (hediff == null)
            {
                return "ScramSRP_Infect_Error".Translate();
            }
            if (string.IsNullOrEmpty(desc) || culprit == null)
            {
                return "ScramSRP_Infect_Unknown".Translate(hediff.pawn.Named("VICTIM"));
            }
            return desc.Translate(hediff.pawn.Named("VICTIM"), culprit.Named("CULPRIT"));
        }
        public void ExposeData()
        {
            Scribe_References.Look(ref culprit, "culprit", saveDestroyedThings: true);
            Scribe_References.Look(ref hediff, "hediff");
            Scribe_Values.Look(ref desc, "descKeyDesc");
        }
    }
}
