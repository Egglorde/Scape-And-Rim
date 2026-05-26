using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree.Hediffs
{
    public class Hediff_InvisExtraBehavior : HediffWithComps
    {
        private HediffComp_Invisibility invisComp => GetComp<HediffComp_Invisibility>();
        public override void Notify_PawnDamagedThing(Thing thing, DamageInfo dinfo, DamageWorker.DamageResult result)
        {
            invisComp.BecomeVisible(true);
            Severity = 0.1f;
        }
        public override void PostTick()
        {
            if (pawn.mindState.enemyTarget == null)
            {
                invisComp.BecomeInvisible();
            }
            if (!invisComp.PsychologicallyVisible)
            {
                Severity = 1f;
            } else
            {
                Severity = 0.1f;
            }
        }

    }
}
