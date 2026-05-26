using AssemblyParasitesColonThree.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree.HediffComps
{
    public class HediffComp_KillTracker : HediffComp
    {
        public List<KillCredit> killCredits = new List<KillCredit>();
        public override void Notify_KilledPawn(Pawn victim, DamageInfo? dinfo)
        {
            killCredits.Add(new KillCredit(victim, 0.7f));
            Pawn.health.GetOrAddHediff(DefOf_Parasite.ScramSRP_Regen).Severity += victim.BodySize * 0.2f;
        }
        public override void CompExposeData()
        {
            Scribe_Collections.Look(ref killCredits, "killcredits");
        }
        public float kills
        {
            get
            {
                float total = 0;
                foreach (KillCredit kill in killCredits)
                {
                    total += kill.points;
                }
                total += new KillCredit(Pawn, 1).points;
                return total;
            }
        }
    }
}
