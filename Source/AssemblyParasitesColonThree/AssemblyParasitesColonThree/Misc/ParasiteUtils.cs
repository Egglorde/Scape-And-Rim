using AssemblyParasitesColonThree.Comps;
using AssemblyParasitesColonThree.HediffComps;
using AssemblyParasitesColonThree.MapRelated;
using AssemblyParasitesColonThree.Misc.Makers;
using AssemblyParasitesColonThree.Misc;
using AssemblyParasitesColonThree.Misc_Defs;
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
using Verse.Noise;
using static RimWorld.BaseGen.SymbolStack;
using static UnityEngine.GraphicsBuffer;

namespace AssemblyParasitesColonThree
{
    public static class ParasiteUtils
    {
        public static List<PawnKindDef> Primitives = [
            DefOf_Parasite.ScramSRP_Pri_LongArmsKind,
            DefOf_Parasite.ScramSRP_Pri_SummonerKind,
            DefOf_Parasite.ScramSRP_Pri_BolsterKind,
            DefOf_Parasite.ScramSRP_Pri_ManducaterKind,
            DefOf_Parasite.ScramSRP_Pri_YelloweyeKind,
            DefOf_Parasite.ScramSRP_Pri_ReekerKind,
            DefOf_Parasite.ScramSRP_Pri_ArachnidaKind
            ];
        public static int phase(float points)
        {

            switch (points)
            {
                case >= 1800000000: return 10;
                case >= 1000000000: return 9;
                case >= 500000000: return 8;
                case >= 25000000: return 8;
                case >= 5000000: return 6;
                case >= 200000: return 5;
                case >= 30000: return 4;
                case >= 5000: return 3;
                case >= 1600: return 2;
                case >= 800: return 1;
                case >= 0: return 0;
                default:
                    return -1;
            }
        }
        public static void TryTransmit(Pawn target, Thing culprit,  bool direct, float transmissionPower, string desc)
        {
            float toxEnvResist = target.GetStatValue(StatDefOf.ToxicEnvironmentResistance);
            if (target.RaceProps.FleshType != DefOf_Parasite.ScramSRP_ParasiteFlesh && !target.health.hediffSet.HasHediff<Hediff_Infected>() && target.RaceProps.FleshType.isOrganic && !target.health.hediffSet.HasHediff<Hediff_COTH>())
            {
                if (direct ? transmissionPower >= Rand.Range(0.0f, 1.0f) : (1 - toxEnvResist) * transmissionPower + 0.01 >= Rand.Range(0.0f, 1.0f))
                {
                    Hediff_COTH Cothhediff = (Hediff_COTH)HediffMaker.MakeHediff(DefOf_Parasite.ScramSRP_HediffCOTHDef, target);
                    Cothhediff.TransmissionData = new TransmissionData(culprit, Cothhediff, desc);
                    target.health.AddHediff(Cothhediff, null);
                }
            }

        }

        public static void BeginConversion(Pawn pawn, bool direct)
        {
            Thing_TransformationCutscene transformationCutscene = ThingMaker.MakeThing(DefOf_Parasite.ScramSRP_TransformationCutscene) as Thing_TransformationCutscene;
            transformationCutscene.DirectAssim = direct;
            GenSpawn.Spawn(transformationCutscene, pawn.Position, pawn.Map, WipeMode.VanishOrMoveAside);
            pawn.equipment?.DropAllEquipment(pawn.Position);
            pawn.health.AddHediff(DefOf_Parasite.ScramSRP_Conversion);
            if (pawn.Faction == Faction.OfPlayer)
            {
                Find.LetterStack.ReceiveLetter("ScramSRP_SimAsimEventTitle".Translate(pawn, pawn.def), "ScramSRP_SimAsimEventDesc".Translate(pawn, pawn.def), DefOf_Parasite.ScramSRP_AssimEvent, pawn);
            }
            pawn.DeSpawn();
            transformationCutscene.InnerPawn = pawn;
        }

        public static void CompleteConversion(Pawn pawn, Hediff coth)
        {
            // COTH severity * Bodysize
            if (pawn.Map == null) {
                return; 
            }
            if (coth == null)
            {
                Turn(pawn, DefOf_Parasite.ScramSrp_Sim, false);
            }
            if (coth.Severity <= 1)
            {
                Log.Warning("Attempted to use CompleteConversion() on a pawn without COTH, use Turn() instead");
                List<Pawn> pawns = FillToTotalWeight(pawn.BodySize * coth.Severity, pawn.Map.GetComponent<MapComp_InfectionTracker>().phaseDef.incompleteGroupMakers);
                for (int i = 0; i < pawns.Count; i++)
                {
                    GenSpawn.Spawn(pawns.ElementAt(i), pawn.Position, pawn.Map);
                    if (i != 0)
                    {
                        YeetSpawnParasite(pawns.ElementAt(i), pawn.Map, pawn.PositionHeld, 3);
                    }
                }
            }
            switch (coth.Severity)
            {
                case > 1:
                    // Hidden
                    Log.Message("Hidden");
                    Turn(pawn, DefOf_Parasite.ScramSrp_Sim, false);
                    break;
                case >= 0.6f:
                    // Incomplete
                    Log.Message("Incomplete");
                    pawn.Destroy();
                    break;
                case >= 0.3f:
                    // Prodromal
                    Log.Message("Prodromal");
                    if (coth.Severity + (1-pawn.health.hediffSet.GetCoverageOfNotMissingNaturalParts(pawn.RaceProps.body.corePart)) >= 1)
                    {
                        pawn.Destroy();
                    }
                    else
                    {
                        RemovePercentageBodyParts(pawn, coth.Severity);
                        coth.Severity = 0.1f;
                    }
                    break;
                default:
                    // A log warning because there really should not be anything converting at incubation stage
                    Log.Warning("Tried to complete a conversion on a pawn with a COTH severity of less than 0.3f");
                    break;
            }
        }

        public static void RemovePercentageBodyParts(Pawn pawn, float percentage)
        {
            for (int i = 0; i < 99 && (1 - pawn.health.hediffSet.GetCoverageOfNotMissingNaturalParts(pawn.RaceProps.body.corePart)) < percentage; i++)
            {
                pawn.health.AddHediff(HediffDefOf.MissingBodyPart, pawn.health.hediffSet.GetNotMissingParts().RandomElementByWeight((BodyPartRecord b) => DistanceToCore(b)+1));
                if (i == 99)
                {
                    Log.Error("Could not remove enough parts!");
                }
            }
        }

        public static List<Pawn> FillToTotalWeight(float maxSize, List<ParasiteGroupMaker> optionList)
        {
            List<Pawn> assimList = new List<Pawn>();
            float totalSize = 0;
            for (int i = 0; i < 99 && optionList.Where(p => p.weight + totalSize < maxSize).Any(); i++) {
                ParasiteGroupMaker result = optionList.Where(p => p.weight + totalSize < maxSize).RandomElementByWeight((ParasiteGroupMaker g) => g.weight);
                assimList.Add(PawnGenerator.GeneratePawn(new PawnGenerationRequest(result.kind, ParasiteUtils.OfParasiteFaction, PawnGenerationContext.NonPlayer, null, forceGenerateNewPawn: true, canGeneratePawnRelations: false)));
                totalSize += result.weight;
                if (i == 99)
                {
                    Log.Error("2 big :<");
                }
            }
            return assimList;
        }

        public static int DistanceToCore(BodyPartRecord input)
        {
            BodyPartRecord currentBodyPart = input;
            for (int i = 0; i < 99 && currentBodyPart.parent != null; i++)
            {
                if (currentBodyPart.parent == null) return i;
                currentBodyPart = currentBodyPart.parent;
            }
            return 0;
        }

        public static bool FetchKillList(Thing thing, out List<KillCredit> killList)
        {
            if (thing.HasComp<Comp_EvolverParasite_Kills>())
            {
                killList = thing.TryGetComp<Comp_EvolverParasite_Kills>().killCredits;
                return true;
            }
            else if (thing is Pawn pawn && !pawn.health.hediffSet.GetHediffComps<HediffComp_KillTracker>().EnumerableNullOrEmpty())
            {
                killList = pawn.health.hediffSet.GetHediffComps<HediffComp_KillTracker>().First().killCredits;
                return true;
            }
            killList = null;
            return false;
        }
            //pawn.health.GetOrAddHediff(DefOf_Parasite.ScramSRP_Regen).Severity += killCredit.points * 0.2f;

        public static void Turn(Pawn pawn, InfectedFormTypeDef type, bool alert)
        {
            if (pawn.Faction != OfParasiteFaction) pawn.SetFaction(OfParasiteFaction);
            if (pawn.Faction == Faction.OfPlayer && alert)
            {
                Find.LetterStack.ReceiveLetter("ScramSRP_SimAsimEventTitle".Translate(pawn, pawn.def), "ScramSRP_SimAsimEventDesc".Translate(pawn, pawn.def), DefOf_Parasite.ScramSRP_AssimEvent, pawn);
            }
            HealthUtility.HealNonPermanentInjuriesAndRestoreLegs(pawn);
            if (pawn.story != null)
            {
                if (!type.forcedHeadTypes.NullOrEmpty())
                {
                    pawn.story.TryGetRandomHeadFromSet(type.forcedHeadTypes);
                }
            }
            Find.World.GetComponent<WorldComp_UbiquitousDevelopment>().AssimilateKind(pawn.kindDef);
            pawn.equipment?.DestroyAllEquipment();
            MutantUtility.SetPawnAsMutantInstantly(pawn, DefOf_Parasite.ScramSRP_AssimilatedMutant);
            pawn.Drawer.renderer.SetAllGraphicsDirty();
        }


        public static Faction OfParasiteFaction => Find.FactionManager.FirstFactionOfDef(DefOf_Parasite.ParasiteFaction);
        public static List<PhaseDef> Phases => DefDatabase<PhaseDef>.AllDefsListForReading;
        public static void SpawnInfectorGas(IntVec3 vec, Map map, int density)
        {
            Gas_InfestedAir gas = (Gas_InfestedAir)ThingMaker.MakeThing(DefOf_Parasite.InfectorZoneThingDef);
            gas.density = density;
            GenSpawn.Spawn(gas, vec, map);
        }

        private static List<IntVec3> tmpTakenCells = new List<IntVec3>();
        public static Thing YeetSpawnParasite(Pawn pawn, Map map, IntVec3 rootCell, int throwdist, bool los = true)
        {
            if (!pawn.Spawned)
            {
                GenSpawn.Spawn(pawn, rootCell, map);
            }
            tmpTakenCells.Clear();
            if (RCellFinder.TryFindRandomCellNearWith(rootCell, (IntVec3 c) => !c.Fogged(map) && c.Standable(map) && !tmpTakenCells.Contains(c) && c.GetFirstPawn(map) == null && (!los || GenSight.LineOfSight(rootCell, c, map, skipFirstCell: true)), map, out var result, 5, throwdist))
            {
                pawn.rotationTracker.FaceCell(result);
                tmpTakenCells.Add(result);
                PawnFlyer pawnFlyer = PawnFlyer.MakeFlyer(ThingDefOf.PawnFlyer_Stun, pawn, result, null, null);
                if (pawnFlyer != null)
                {
                    GenSpawn.Spawn(pawnFlyer, result, map);
                }
                return pawnFlyer;
            }
            return null;
        }
    }
}
