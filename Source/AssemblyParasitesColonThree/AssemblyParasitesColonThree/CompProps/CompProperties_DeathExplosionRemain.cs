using AssemblyParasitesColonThree.Comps;
using AssemblyParasitesColonThree.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree.CompProps
{
    public class CompProperties_DeathExplosionRemain : CompProperties
    {
        public ParasiteTier remaintier;
        public CompProperties_DeathExplosionRemain()
        {
            compClass = typeof(Comp_DeathExplosionRemain);
        }
    }
}
