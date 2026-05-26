using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree
{
    public class CompProperties_BeckonSpreadInfection : CompProperties
    {
        public int checks = 3;
        public int radius = 7;

        public CompProperties_BeckonSpreadInfection()
        {
            compClass = typeof(Comp_BeckonSpreadInfection);
        }
    }

}
