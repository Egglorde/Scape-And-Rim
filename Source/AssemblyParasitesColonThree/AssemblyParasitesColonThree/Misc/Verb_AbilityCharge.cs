using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree.Misc
{
    public class Verb_AbilityCharge : Verb_AbilityShoot
    {
        public override void WarmupComplete()
        {
            burstShotsLeft = ShotsPerBurst;
            state = VerbState.Bursting;
            TryCastNextBurstShot();
        }
    }
}
