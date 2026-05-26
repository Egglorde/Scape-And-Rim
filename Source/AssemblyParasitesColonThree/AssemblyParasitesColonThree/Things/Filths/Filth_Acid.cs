using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree.Things.Filths
{
    public class Filth_Acid : LiquidFuel
    {
        protected override void Tick()
        {
            base.Tick();
            if (Spawned && this.Map.IsHashIntervalTick(9))
            {
                List<Thing> thingList = Position.GetThingList(Map);
                for (int i = 0; i < thingList.Count; i++)
                {
                    if (thingList[i] is Pawn {Flying: false} result)
                    {
                        result.TakeDamage(new DamageInfo(DamageDefOf.AcidBurn, 3f, instigator:this, spawnFilth:false));
                    }
                }
            }
        }
    }
}
