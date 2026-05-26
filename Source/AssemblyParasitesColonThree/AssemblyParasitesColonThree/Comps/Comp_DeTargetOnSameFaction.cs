using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace AssemblyParasitesColonThree
{
    public class Comp_DeTargetOnSameFaction : ThingComp
    {
        private CompProperties_DeTargetOnSameFaction Props => (CompProperties_DeTargetOnSameFaction)props;
        private Pawn Infected => (Pawn)parent;

        private LocalTargetInfo Ignore;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override void CompTick()
        {
            if (Infected != null && Infected.LastAttackedTarget != null)
            {
                if (Infected.LastAttackedTarget != Ignore && Infected.LastAttackedTarget.Thing.Faction == parent.Faction)
                {
                    Infected.jobs.EndCurrentJob(JobCondition.InterruptForced);
                    Ignore = Infected.LastAttackedTarget;
                }
            }
        }
    }
}
