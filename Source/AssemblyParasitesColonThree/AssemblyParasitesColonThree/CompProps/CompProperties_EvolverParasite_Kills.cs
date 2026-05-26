using AssemblyParasitesColonThree.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree.CompProps
{
    public class CompProperties_EvolverParasite_Kills : CompProperties
    {
        public PawnKindDef evolutionTarget;

        public int ReqKills = 0;

        public CompProperties_EvolverParasite_Kills()
        {
            compClass = typeof(Comp_EvolverParasite_Kills);
        }
    }
}
