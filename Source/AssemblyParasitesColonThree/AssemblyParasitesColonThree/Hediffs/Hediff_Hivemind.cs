using AssemblyParasitesColonThree.Comps;
using AssemblyParasitesColonThree.MapRelated;
using AssemblyParasitesColonThree.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree.Hediffs
{
    public class Hediff_Hivemind : HediffWithComps
    {
        public override void Notify_KilledPawn(Pawn victim, DamageInfo? dinfo)
        {
            if (pawn.Map != null)
            {
                pawn.Map.GetComponent<MapComp_InfectionTracker>().GainPoints(1);
                if (ParasiteUtils.FetchKillList(pawn, out List<KillCredit> list))
                {
                    list.Add(new KillCredit(victim, 0.7f));
                    pawn.health.GetOrAddHediff(DefOf_Parasite.ScramSRP_Regen).Severity += victim.BodySize * 0.2f;
                }
            }
        }
    }
}
