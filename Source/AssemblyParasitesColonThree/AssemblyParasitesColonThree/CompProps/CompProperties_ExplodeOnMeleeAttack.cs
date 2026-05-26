using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree
{
    public class CompProperties_ExplodeOnMeleeAttack : CompProperties
    {
        public DamageDef damage;

        public float radius;
        public CompProperties_ExplodeOnMeleeAttack()
        {
            compClass = typeof(Comp_ExplodeOnMeleeAttack);
        }
    }
}
