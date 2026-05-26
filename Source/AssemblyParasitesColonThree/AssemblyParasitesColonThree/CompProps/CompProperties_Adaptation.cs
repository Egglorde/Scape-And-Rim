using AssemblyParasitesColonThree.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree.CompProps
{
    public class CompProperties_Adaptation : CompProperties
    {
        public float learnChance;
        public float failChance;
        public float reductionPerPoint;
        public int reductionCap;
        public int maxLearnedDamageSources;

        public CompProperties_Adaptation()
        {
            compClass = typeof(Comp_Adaptation);
        }
    }
}
