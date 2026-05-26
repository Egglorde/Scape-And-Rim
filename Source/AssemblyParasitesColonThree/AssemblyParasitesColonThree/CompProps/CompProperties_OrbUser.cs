using AssemblyParasitesColonThree.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree.CompProps
{
    public class CompProperties_OrbUser : CompProperties
    {
        public List<HediffDef> enemyHediffs;
        public List<HediffDef> allyHediffs;
        public List<HediffDef> selfHediffs;
        public float Potency;

        public CompProperties_OrbUser()
        {
            compClass = typeof(Comp_OrbUser);
        }
    }
}
