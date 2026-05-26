using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;

namespace AssemblyParasitesColonThree.LordRelated
{
    public class LordJob_Riot : LordJob
    {
        public Pawn summoner;
        public override StateGraph CreateGraph()
        {
            StateGraph stateGraph = new StateGraph();
            LordToil_Riot riot = new LordToil_Riot();
            riot.summoner = summoner;
            stateGraph.AddToil(riot);
            return stateGraph;
        }

        public LordJob_Riot()
        {
        }
        public override void ExposeData()
        {
            Scribe_References.Look(ref summoner, "summoner");
        }
    }
}
