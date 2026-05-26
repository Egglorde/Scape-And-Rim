using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace AssemblyParasitesColonThree.LordRelated
{
    public class LordToil_Riot : LordToil
    {
        public Pawn summoner;
        public LordToil_Riot() {}
        public override void UpdateAllDuties()
        {
            for (int i = 0; i < lord.ownedPawns.Count; i++)
            {
                lord.ownedPawns[i].mindState.duty = new PawnDuty(DefOf_Parasite.ScramSRP_Riot_Frenzy, summoner);
            }
        }
    }
}
