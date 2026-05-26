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
    public class LordToil_Merge : LordToil
    {
        public Pawn dominant;
        public override void UpdateAllDuties()
        {
            for (int i = 0; i < lord.ownedPawns.Count; i++)
            {
                Pawn pawn = lord.ownedPawns[i];
                if (!pawn.Awake())
                {
                    RestUtility.WakeUp(pawn);
                }
                if (pawn == dominant)
                {
                    PawnDuty pawnDuty = new PawnDuty(DefOf_Parasite.ScramSRP_Merge_Dominant);
                    pawn.mindState.duty = pawnDuty;
                }
                else
                {
                    PawnDuty pawnDuty2 = new PawnDuty(DefOf_Parasite.ScramSRP_Merge_Submissive, dominant);
                    pawn.mindState.duty = pawnDuty2;
                }
                pawn.jobs?.CheckForJobOverride();
            }
        }
        public LordToil_Merge(Pawn dom) { 
            this.dominant = dom;
        }
    }
}
