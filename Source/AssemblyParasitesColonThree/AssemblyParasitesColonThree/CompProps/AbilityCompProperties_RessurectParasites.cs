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
    public class AbilityCompProperties_RessurectParasites : CompProperties_AbilityEffect
    {
        public AbilityCompProperties_RessurectParasites()
        {
            compClass = typeof(AbilityComp_ResurrectParasites);
        }
    }
}
