using AssemblyParasitesColonThree.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree.CompProps
{
    public class CompProperties_HealerParasite : CompProperties
    {
        public float maxRegen;
        public float regenSevGain;
        public int interval;
        public EffecterDef effecterDef;

        public CompProperties_HealerParasite()
        {
            compClass = typeof(Comp_HealerParasite);
        }
    }
}
