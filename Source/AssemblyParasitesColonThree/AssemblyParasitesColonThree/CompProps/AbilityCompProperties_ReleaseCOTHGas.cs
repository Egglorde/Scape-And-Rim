using AssemblyParasitesColonThree.Comps;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree.CompProps
{
    public class AbilityCompProperties_ReleaseCOTHGas : CompProperties_AbilityEffect
    {
        public int density;
        public AbilityCompProperties_ReleaseCOTHGas()
        {
            compClass = typeof(AbilityComp_ReleaseCOTHGas);
        }
    }
}
