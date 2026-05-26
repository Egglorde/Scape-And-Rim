using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree
{
    public class DeathActionProperties_PopIntoParas : DeathActionProperties
    {
        public List<PawnKindDef> carrierpawnkindoptions = new List<PawnKindDef>();

        public int dividePawnCount;

        public List<PawnKindDef> dividePawnKindAdditionalForced = new List<PawnKindDef>();

        public IntRange divideBloodFilthCountRange;

        public int popRange;

        public bool leaveCorpse = false;

        public DeathActionProperties_PopIntoParas()
        {
            workerClass = typeof(DeathActionWorker_PopIntoParas);
        }

        public override IEnumerable<string> ConfigErrors()
        {
            if (dividePawnCount <= 0 && dividePawnKindAdditionalForced.NullOrEmpty())
            {
                yield return "deathActionWorkerClass is DeathActionWorker_Divide or subclass, but dividePawnCount <= 0.";
            }
            if (carrierpawnkindoptions.NullOrEmpty() && dividePawnKindAdditionalForced.NullOrEmpty())
            {
                yield return "deathActionWorkerClass is DeathActionWorker_Divide or subclass, but dividePawnKindOptions is null or empty.";
            }
        }
    }
}
