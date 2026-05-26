using AssemblyParasitesColonThree.CompProps;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree.Comps
{
    public class AbilityComp_ReleaseCOTHGas : CompAbilityEffect
    {
        public new AbilityCompProperties_ReleaseCOTHGas Props => (AbilityCompProperties_ReleaseCOTHGas)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            ParasiteUtils.SpawnInfectorGas(target.Cell, parent.pawn.MapHeld, Props.density);
            ParasiteUtils.TryTransmit(target.Pawn, this.parent.pawn, true, 0.99f, "ScramSRP_Infect_Injected");
        }
    }
}
