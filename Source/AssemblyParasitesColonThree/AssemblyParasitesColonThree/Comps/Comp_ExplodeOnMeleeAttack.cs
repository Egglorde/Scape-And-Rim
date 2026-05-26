using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree
{
    public class Comp_ExplodeOnMeleeAttack : ThingComp
    {
        public CompProperties_ExplodeOnMeleeAttack Props => (CompProperties_ExplodeOnMeleeAttack)props;

        public override void Notify_UsedVerb(Pawn pawn, Verb verb)
        {
            if (verb.IsMeleeAttack)
            {
                GenExplosion.DoExplosion(verb.CurrentTarget.Cell, pawn.Map, Props.radius, Props.damage, this.parent, intendedTarget: verb.CurrentTarget.Thing);
            }
        }
    }
}
