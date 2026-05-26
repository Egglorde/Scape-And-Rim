using AssemblyParasitesColonThree.Comps;
using AssemblyParasitesColonThree.MapRelated;
using AssemblyParasitesColonThree.Misc.Makers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree.CompProps
{
    public class CompProperties_Summoner : CompProperties
    {
        public int maxpoints = 200;

        public int pawnsperwave = 4;

        public List<ParasiteGroupMaker> spawnablepawns = new List<ParasiteGroupMaker>();

        public SoundDef soundDef;

        public bool allowSims;

        public int cooldown = 600;

        public IEnumerable<ParasiteGroupMaker> Spawnables
        {
            get
            {
                List<ParasiteGroupMaker> groupMakers = spawnablepawns;
                if (allowSims)
                {
                    groupMakers.AddRange(WorldComp_UbiquitousDevelopment.SimGroupMakers());
                }
                return groupMakers;
            }
        }
        public CompProperties_Summoner()
        {
            compClass = typeof(Comp_Summoner);
        }
    }
}
