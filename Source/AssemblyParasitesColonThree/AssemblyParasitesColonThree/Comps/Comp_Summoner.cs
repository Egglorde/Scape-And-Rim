using AssemblyParasitesColonThree.CompProps;
using AssemblyParasitesColonThree.LordRelated;
using AssemblyParasitesColonThree.Misc.Makers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;

namespace AssemblyParasitesColonThree.Comps
{
    public class Comp_Summoner : ThingComp
    {
        public CompProperties_Summoner Props => (CompProperties_Summoner)props;

        public int lastSummonTick;
        public bool canSummonAgain => lastSummonTick + Props.cooldown < Find.TickManager.TicksGame;

        private Lord summonLord;
        public Lord SummonerLord
        {
            get
            {
                if (summonLord == null) {
                    LordJob_Riot riot = new LordJob_Riot();
                    riot.summoner = (Pawn)parent;
                    summonLord = LordMaker.MakeNewLord(parent.Faction, riot, parent.Map);
                }
                return summonLord;
            }
        }
        public int SlavePoints
        {
            get
            {
                if (!SummonerLord.AnyActivePawn)
                {
                    return 0;
                }
                List<Pawn> lordOwnedPawns = SummonerLord.ownedPawns;
                int Points = 0;
                foreach (Pawn pawn in lordOwnedPawns)
                {
                    //
                    if (pawn.Dead || pawn.MapHeld != parent.Map) continue;
                    foreach (ParasiteGroupMaker groupMaker in Props.spawnablepawns)
                    {
                        if (groupMaker.kind == pawn.kindDef)
                        {
                            Points += groupMaker.points;
                        }
                    }
                }
                return Points;
            }
        }

    }
}
