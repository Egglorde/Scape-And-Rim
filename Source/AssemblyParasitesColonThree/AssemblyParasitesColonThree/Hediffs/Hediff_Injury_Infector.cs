using AssemblyParasitesColonThree.MapRelated;
using AssemblyParasitesColonThree.Things;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree.Hediffs
{
    public class Hediff_Injury_Infector : Hediff_Injury
    {
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            if (pawn.health.ShouldBeDead() || pawn.health.WouldBeDownedAfterAddingHediff(this))
            {
                if (pawn.health.ShouldBeDead())
                {
                    pawn.health.Notify_Resurrected();
                    if (pawn.health.ShouldBeDead())
                    {
                        return;
                    }
                }
                if (dinfo?.Instigator != null && ParasiteUtils.FetchKillList(dinfo?.Instigator, out List<KillCredit> list))
                {
                    list.Add(new KillCredit(pawn, 1));
                    if (dinfo?.Instigator is Pawn pawn2) pawn2.health.GetOrAddHediff(DefOf_Parasite.ScramSRP_Regen).Severity += pawn.BodySize * 0.2f;
                }
                if (pawn.Map != null)
                {
                    pawn.Map.GetComponent<MapComp_InfectionTracker>().GainPoints(6);
                    ParasiteUtils.BeginConversion(pawn, true);
                }
                else
                    ParasiteUtils.Turn(pawn, DefOf_Parasite.ScramSrp_Sim, pawn.IsColonist);
                Severity = 0;
            }
            if (dinfo?.Instigator != null) ParasiteUtils.TryTransmit(pawn, dinfo?.Instigator, true, 0.7f, "ScramSRP_Infect_Attacked");
        }
    }
}
