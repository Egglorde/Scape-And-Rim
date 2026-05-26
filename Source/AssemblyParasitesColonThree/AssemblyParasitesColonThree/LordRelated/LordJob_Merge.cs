using AssemblyParasitesColonThree.HediffComps;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using static UnityEngine.GraphicsBuffer;

namespace AssemblyParasitesColonThree.LordRelated
{
    public class LordJob_Merge : LordJob
    {
        public Pawn dominant;
        public override StateGraph CreateGraph()
        {
            StateGraph stateGraph = new StateGraph();
            LordToil_Merge merge = new LordToil_Merge(dominant);
            stateGraph.AddToil(merge);
            return stateGraph;
        }

        public override void LordJobTick()
        {
            if (dominant == null || dominant.Dead)
            {
                dominant = lord.ownedPawns.RandomElementByWeight((Pawn p) => p.health.hediffSet.GetFirstHediff<Hediff_Infected>().TryGetComp<HediffComp_KillTracker>().kills);
            }
        }

        public LordJob_Merge()
        {
        }
        public LordJob_Merge(Pawn dominant)
        {
            this.dominant = dominant;
        }

        public override void ExposeData()
        {
            Scribe_References.Look(ref dominant, "dominant");
        }
    }
}
