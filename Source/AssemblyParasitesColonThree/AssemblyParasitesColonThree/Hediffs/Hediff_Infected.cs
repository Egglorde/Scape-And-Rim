using AssemblyParasitesColonThree.MapRelated;
using AssemblyParasitesColonThree.Misc;
using AssemblyParasitesColonThree.Things;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AssemblyParasitesColonThree
{
    public class Hediff_Infected : HediffWithComps
    {
        private LocalTargetInfo Ignore;
        private InfectedFormTypeDef FormTypedef => (InfectedFormTypeDef)def;
        private InfectedFormDef infectedFormDef => pawn.kindDef.GetModExtension<InfectedFormDefModExtension>()?.infectedForm;

        public override void Tick()
        {
            if (pawn != null && pawn.LastAttackedTarget != null)
            {
                if (pawn.LastAttackedTarget != Ignore && pawn.LastAttackedTarget.Thing.Faction == pawn.Faction)
                {
                    pawn.jobs.StopAll();
                    Ignore = pawn.LastAttackedTarget;
                }
            }
        }
        public override void Notify_KilledPawn(Pawn victim, DamageInfo? dinfo)
        {
            if (pawn.Map != null)
            {
                pawn.Map.GetComponent<MapComp_InfectionTracker>().GainPoints(1);
            }
        }

        public void Postkill()
        {
            pawn.MapHeld?.GetComponent<MapComp_InfectionTracker>().ModifyVectorHealth(pawn.BodySize * -20);
            if (pawn.RaceProps.intelligence == Intelligence.Humanlike && FormTypedef.headBasePawnKind != null)
            {
                if (pawn.health.hediffSet.HasHead && 0.5 >= Rand.Range(0.0f, 1.0f))
                {
                    MakeHeadPawnKind(FormTypedef.headBasePawnKind, pawn.Faction);
                }
            }
            else
            {
                if (infectedFormDef?.headPawnKind != null)
                {
                    if (pawn.health.hediffSet.HasHead && 0.5 >= Rand.Range(0.0f, 1.0f))
                    {
                        MakeHeadPawnKind(infectedFormDef.headPawnKind, pawn.Faction);
                    }
                }
            }
        }

        public override void Notify_PawnDamagedThing(Thing thing, DamageInfo dinfo, DamageWorker.DamageResult result)
        {
            if (result.hediffs == null)
            {
                return;
            }
            for (int i = 0; i < result.hediffs.Count; i++)
            {
                Hediff hediff = result.hediffs[i];
                if (hediff.Bleeding)
                {
                    ParasiteUtils.TryTransmit(hediff.pawn, this.pawn, true, 0.3f, "ScramSRP_Infect_Attacked");
                }
                ParasiteUtils.TryTransmit(hediff.pawn, this.pawn, false, 0.05f, "ScramSRP_Infect_Attacked");

            }
        }


        public override void PostAdd(DamageInfo? dinfo)
        {
            ParasiteUtils.Turn(this.pawn, FormTypedef, false);
        }

        public void MakeHeadPawnKind(PawnKindDef kind, Faction faction)
        {
            pawn.health.AddHediff(HediffDefOf.MissingBodyPart, pawn.health.hediffSet.GetNotMissingParts().First((BodyPartRecord p) => p.def == BodyPartDefOf.Head));
            Pawn child = PawnGenerator.GeneratePawn(new PawnGenerationRequest(kind, faction, PawnGenerationContext.NonPlayer, null, forceGenerateNewPawn: true, allowDead: false, allowDowned: false, canGeneratePawnRelations: false, mustBeCapableOfViolence: true, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, pawn.ageTracker.AgeBiologicalYearsFloat));
            GenSpawn.Spawn(child, pawn.PositionHeld, pawn.MapHeld, WipeMode.VanishOrMoveAside);
            if (pawn.Corpse?.ParentHolder is Explosion_Death_Aneurysm)
            {
                ParasiteUtils.YeetSpawnParasite(child, pawn.MapHeld, pawn.PositionHeld, 3);
            }
            pawn.lord?.AddPawn(child);
        }
    }
}
