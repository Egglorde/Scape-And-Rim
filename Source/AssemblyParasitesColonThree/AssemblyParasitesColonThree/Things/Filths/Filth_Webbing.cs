using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree.Things.Filths
{
    public class Filth_Webbing : Filth
    {
        protected override void Tick()
        {
            base.Tick();
            if (Spawned && this.Map.IsHashIntervalTick(9))
            {
                List<Thing> thingList = Position.GetThingList(Map);
                for (int i = 0; i < thingList.Count; i++)
                {
                    if (thingList[i] is Pawn { Flying: false } result)
                    {
                        result.stances.stunner.StunFor(120, this);
                        ThinFilth();
                    }
                }
            }
        }
    }
}
