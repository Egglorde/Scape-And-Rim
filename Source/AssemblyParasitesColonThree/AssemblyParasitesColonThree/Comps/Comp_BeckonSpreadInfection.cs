using AssemblyParasitesColonThree.MapRelated;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree
{
    public class Comp_BeckonSpreadInfection : ThingComp
    {
        public List<Pawn> beckonSlaves = new List<Pawn>();
        public CompProperties_BeckonSpreadInfection Props => (CompProperties_BeckonSpreadInfection)props;

        public void UpdateSlaves()
        {
            beckonSlaves.RemoveAll(b => b.health.Dead);
        }
        public float SlavePoints()
        {
            UpdateSlaves();
            float num = 0f;
            if (beckonSlaves != null)
            {
                foreach (Pawn p in beckonSlaves)
                {
                    num += p.kindDef.combatPower;
                }
            }
            else
            {
                Log.Warning("Slaves is null!");
            }
            return num;
        }

        public override void CompTickRare()
        {
            for (int i = 0; i < Props.checks; i++)
            {
                if (parent.Spawned && RCellFinder.TryFindRandomCellNearWith(parent.Position, (IntVec3 c) => !c.Fogged(parent.Map) && c.Standable(parent.Map) && c.GetTerrain(parent.Map) == DefOf_Parasite.ScramSRP_InfestedSoil && c.DistanceTo(parent.Position) <= Props.radius, parent.Map, out var result, Props.radius, Props.radius))
                {
                    foreach (IntVec3 item in GenAdj.CellsAdjacentCardinal(result, Rot4.North, IntVec2.Zero))
                    {
                        Building building1 = item.GetEdifice(parent.Map);

                        if (building1 == null && !item.Impassable(parent.Map) && item.InBounds(parent.Map) && item.GetTerrain(parent.Map) != DefOf_Parasite.ScramSRP_InfestedSoil)
                        {
                            parent.Map.terrainGrid.SetTerrain(item, DefOf_Parasite.ScramSRP_InfestedSoil);
                            parent.Map.GetComponent<MapComp_InfectionTracker>().GainPoints(50);
                            if (item.GetFirstThing(parent.Map, DefOf_Parasite.ScramSRP_Filth_ResidueInfested) != null)
                            {
                                item.GetFirstThing(parent.Map, DefOf_Parasite.ScramSRP_Filth_ResidueInfested).Destroy();
                            }
                        }
                    }
                    if (result.GetPlant(parent.Map) == null || result.GetPlant(parent.Map).def != DefOf_Parasite.ScramSRP_ParasiteBrambles)
                    {
                        Plant plant = (Plant)ThingMaker.MakeThing(DefOf_Parasite.ScramSRP_ParasiteBrambles);
                        GenSpawn.Spawn(plant, result, parent.Map);
                    }
                }
            }

        }
        public override string CompInspectStringExtra()
        {
            return "Total points: " + SlavePoints();
        }
    }
}
