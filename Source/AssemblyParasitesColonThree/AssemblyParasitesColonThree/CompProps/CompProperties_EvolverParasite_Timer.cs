using AssemblyParasitesColonThree.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree
{
    public class CompProperties_EvolverParasite_Timer : CompProperties
    {
        public PawnKindDef evolutionTarget;

        public int ReqTimeTicks = 0;

        public CompProperties_EvolverParasite_Timer()
        {
            compClass = typeof(Comp_EvolverParasite_Timer);
        }
    }
}
