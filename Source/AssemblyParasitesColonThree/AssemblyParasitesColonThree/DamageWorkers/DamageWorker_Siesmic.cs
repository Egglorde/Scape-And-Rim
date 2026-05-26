using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree
{
    public class DamageWorker_Siesmic : DamageWorker_AddInjury
    {
        protected override void ExplosionDamageThing(Explosion explosion, Thing t, List<Thing> damagedThings, List<Thing> ignoredThings, IntVec3 cell)
        {
            if (t.Faction == ParasiteUtils.OfParasiteFaction)
            {
                return;
            }
            base.ExplosionDamageThing(explosion, t, damagedThings, ignoredThings, cell);
        }
    }
}
