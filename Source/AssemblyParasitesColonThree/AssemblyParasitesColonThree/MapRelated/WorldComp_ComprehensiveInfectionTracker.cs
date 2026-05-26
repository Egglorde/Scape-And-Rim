using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyParasitesColonThree.MapRelated
{
    public class WorldComp_ComprehensiveInfectionTracker : WorldComponent
    {
        public Dictionary<PlanetTile, int> relevantTiles = new Dictionary<PlanetTile, int>();
        public WorldComp_ComprehensiveInfectionTracker(World world)
            : base(world)
        {
        }
    }
}
