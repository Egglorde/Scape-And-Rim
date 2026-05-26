using AssemblyParasitesColonThree.HediffComps;
using AssemblyParasitesColonThree.MapRelated;
using AssemblyParasitesColonThree.Misc;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;
using Verse.Noise;

namespace AssemblyParasitesColonThree
{
    public class Hediff_COTH : HediffWithComps
    {
        private TransmissionData transmissionData;
        public InfectedFormTypeDef form = DefOf_Parasite.ScramSrp_Sim;
        public TransmissionData TransmissionData
        {
            get
            {
                if (transmissionData == null)
                {
                    transmissionData = new TransmissionData(null, this);
                }
                return transmissionData;
            }
            set
            {
                transmissionData = value;
            }
        }
        public float radius = 8.9f;

        public override void Tick()
        {
            if (Severity >= 1 && Severity < 1.1)
            {
                Severity = 1.1f;
                pawn.Map?.GetComponent<MapComp_InfectionTracker>().GainPoints(6);
                pawn.Map?.GetComponent<MapComp_InfectionTracker>().ModifyVectorHealth(pawn.BodySize * 20);
            }
            if (Severity >= 1.1 && pawn.Map != null && pawn.Map.IsHashIntervalTick(GenTicks.TickLongInterval))
            {
                if (RCellFinder.TryFindRandomCellNearWith(pawn.Position, (IntVec3 c) => !c.Fogged(pawn.Map) && c.GetFirstPawn(pawn.Map) != null, pawn.Map, out var result, 5, 5))
                {
                    ParasiteUtils.TryTransmit(result.GetFirstPawn(pawn.Map), this.pawn, true, 0.35f, "ScramSRP_Infect_Dopple");
                }
                if (Severity * 0.005 >= Rand.Range(0.0f, 1.0f))
                {
                    DoEmergance();
                }
            }
            base.Tick();
        }

        public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            if (Severity >= 0.3f && (totalDamageDealt * 0.05 >= Rand.Range(0.0f, 1.0f) || dinfo.Def == DamageDefOf.Flame))
            {
                DoEmergance();
            }
        }

        public void DoEmergance()
        {
            if (!pawn.Spawned) return;
            List<Pawn> sympList = new List<Pawn>{ pawn };
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("This " + TransmissionData.Desc());
            for (int i = 1; i < pawn.Map.mapPawns.AllPawnsSpawned.Count; i++)
            {
                Pawn pawn2 = pawn.Map.mapPawns.AllPawnsSpawned[i];
                if (pawn2 != this.pawn && pawn2.health.hediffSet.TryGetHediff(out Hediff_COTH coth) && coth.Severity > 0.3f && pawn2.Position.InHorDistOf(pawn.Position, 9.9f) && GenSight.LineOfSightToThing(pawn.Position, pawn2, pawn.Map))
                {
                    sympList.Add(pawn2);
                    stringBuilder.Append("\n"+coth.TransmissionData.Desc().CapitalizeFirst());
                }
            }
            TaggedString taggedString = (sympList.Count != 1) ? "ScramSRP_SimResponseEventDescPlural".Translate(pawn) : "ScramSRP_SimResponseEventDesc".Translate(pawn, pawn.def);
            taggedString += "\n\n" + stringBuilder;
            Find.LetterStack.ReceiveLetter("ScramSRP_SimResponseEventTitle".Translate(pawn), taggedString.Resolve(), LetterDefOf.ThreatBig, sympList);
            foreach (Pawn pawn2 in sympList)
            {
                ParasiteUtils.BeginConversion(pawn2, false);
                Hediff hediff2 = HediffMaker.MakeHediff(DefOf_Parasite.ScramSRP_Conversion, pawn2);
                hediff2.Severity = 0.1f;
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref transmissionData, "transmissionData");
        }
    }
}
