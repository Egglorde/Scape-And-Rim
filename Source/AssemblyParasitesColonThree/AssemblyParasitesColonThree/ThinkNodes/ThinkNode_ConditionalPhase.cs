using AssemblyParasitesColonThree.MapRelated;
using AssemblyParasitesColonThree.Misc;
using AssemblyParasitesColonThree.Misc_Defs;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Jobs;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AssemblyParasitesColonThree.ThinkNodes
{
    public class ThinkNode_ConditionalPhase : ThinkNode_Conditional
    {
        public int MinPhase;
        protected override bool Satisfied(Pawn pawn)
        {
            if (pawn.Map != null)
            {
                MapComp_InfectionTracker mapcomp = pawn.Map.GetComponent<MapComp_InfectionTracker>();
                return ParasiteUtils.phase(mapcomp.points) >= MinPhase;
            }
            return false;
        }
    }
}
