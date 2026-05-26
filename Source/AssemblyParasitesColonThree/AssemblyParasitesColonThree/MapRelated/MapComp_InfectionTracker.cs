using AssemblyParasitesColonThree.Misc.Makers;
using AssemblyParasitesColonThree.Misc_Defs;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Jobs;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace AssemblyParasitesColonThree.MapRelated
{
    public class MapComp_InfectionTracker : MapComponent
    {
        public float points;
        public float deltaPoints;
        public WorldObject_Vector closestVector;
        public int parasitesOnMap
        {
            get {
                return map.mapPawns.SpawnedPawnsInFaction(ParasiteUtils.OfParasiteFaction).Count;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref points, "points");
            Scribe_Values.Look(ref deltaPoints, "deltaPoints");
        }
        public void ModifyVectorHealth(float addPoints)
        {
            if (map.Tile != null && TryFindBestVector())
            {
                closestVector.Health += Mathf.RoundToInt(addPoints);
            }
        }

        public PhaseDef phaseDef
        {
            get
            {
                foreach (PhaseDef phaseDef in ParasiteUtils.Phases)
                {
                    if (phaseDef.phase == ParasiteUtils.phase(points))
                    {
                        return phaseDef;
                    }
                }
                return null;
            }
        }

        public void SetPoints(float set, bool log = false)
        {
            if (ParasiteUtils.phase(set) != ParasiteUtils.phase(points))
            {
                foreach (PhaseDef phaseDef in DefDatabase<PhaseDef>.AllDefs)
                {
                    if (phaseDef.phase == ParasiteUtils.phase(set) && phaseDef.eventLetterName != null)
                    {
                        Find.LetterStack.ReceiveLetter(phaseDef.eventLetterName, "ScramSRP_PhaseIncrease".Translate(), phaseDef.eventLetter);
                    }
                }
            }
            points = set;
            if (log) Log.Message("Points set : " + set);
        }

        public void GainPoints(float gain, bool log = false)
        {
            if (ParasiteUtils.phase(points + gain) != ParasiteUtils.phase(points)) {
                foreach (PhaseDef phaseDef in DefDatabase<PhaseDef>.AllDefs)
                {
                    if (phaseDef.phase == ParasiteUtils.phase(points + gain) && phaseDef.eventLetterName != null)
                    {
                        Find.LetterStack.ReceiveLetter(phaseDef.eventLetterName, "ScramSRP_PhaseIncrease".Translate(), phaseDef.eventLetter);
                    }
                }
            }
            deltaPoints += gain;
            points += gain;
            if (log) Log.Message("Points gained : " + gain);
        }
        //
        public override void MapComponentTick()
        {
            if (map.IsHashIntervalTick(GenTicks.TickLongInterval) && TryFindBestVector())
            {
                if (parasitesOnMap <= phaseDef.maxcount)
                {
                    float rando = Rand.Range(0.0f, 1.0f);
                    if (((float)phaseDef.mincount / (parasitesOnMap*10 + 1) >= rando || phaseDef.mincount > parasitesOnMap) && RCellFinder.TryFindRandomPawnEntryCell(out var result, map, CellFinder.EdgeRoadChance_Hostile))
                    {
                        ParasiteArrival(result);
                    }
                } else
                {
                    DismissParasites();
                }
            }
        }
        private bool TryFindBestVector()
        {
            foreach (WorldObject_Vector ubiq in Find.World.GetComponent<WorldComp_UbiquitousDevelopment>().InfectionVectors())
            {
                if (ubiq.Radius > Find.WorldGrid.ApproxDistanceInTiles(ubiq.Tile, map.Tile) && (closestVector == null || closestVector.Health < ubiq.Health))
                {
                    closestVector = ubiq;
                }
            }
            if (closestVector == null)
            {
                return false;
            }
            return true;
        }
        public void DismissParasites()
        {
            Lord lord = LordMaker.MakeNewLord(ParasiteUtils.OfParasiteFaction, new LordJob_ExitMapBest(LocomotionUrgency.Jog, true, true), map);
            for (int i = 0; i < 99 && parasitesOnMap - phaseDef.maxcount > i ; i++) {
                if (map.mapPawns.SpawnedPawnsInFaction(ParasiteUtils.OfParasiteFaction).Where(p => p.lord == null).TryRandomElementByWeight((Pawn p) => 1/(p.kindDef.combatPower+1), out Pawn result))
                {
                    lord.AddPawn(result);
                }
            }
        }
        public void ParasiteArrival(IntVec3 pos)
        {
            List<Pawn> pawns = GenListParasitePawns();
            for (int i = 0; i < 10 && pawns.Count < phaseDef.mincount - parasitesOnMap; i++)
            {
                pawns.AddRange(GenListParasitePawns());
            }
            Rot4 rot = Rot4.FromAngleFlat((map.Center - pos).AngleFlat);
            foreach (Pawn pawn in pawns)
            {
                IntVec3 loc = CellFinder.RandomClosewalkCellNear(pos, map, 10);
                GenSpawn.Spawn(pawn, loc, map, rot);
            }
            Messages.Message("A group of parasites has arrived", new TargetInfo(pos, map), MessageTypeDefOf.NegativeEvent);
        }

        public List<Pawn> GenListParasitePawns()
        {
            ParasiteGroupMaker element = phaseDef.GetGroupMakers().RandomElementByWeight((ParasiteGroupMaker m) => m.weight);
            int count = element.pawnRange.RandomInRange;
            List<Pawn> resultlist = new List<Pawn>();
            for (int i = 0; i < count; i++)
            {
                Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(element.kind, ParasiteUtils.OfParasiteFaction, PawnGenerationContext.NonPlayer, null, forceGenerateNewPawn: true, allowDead: false, allowDowned: true, canGeneratePawnRelations: false, mustBeCapableOfViolence: false, forceNoGear: false, allowPregnant: false));
                if (element.sim) ParasiteUtils.Turn(pawn, DefOf_Parasite.ScramSrp_Sim, false);
                resultlist.Add(pawn);
            }
            return resultlist;
        }

        public MapComp_InfectionTracker(Map map) : base(map)
        {

        }

    }
}
