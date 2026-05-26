using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AssemblyParasitesColonThree.Things.Projectiles
{
    public class Projectile_Charger : Projectile_Junk
    {
        private bool damagedAnything = false;
        private static List<Thing> tmpThings = new List<Thing>();
        public override void Launch(Thing launcher, Vector3 origin, LocalTargetInfo usedTarget, LocalTargetInfo intendedTarget, ProjectileHitFlags hitFlags, bool preventFriendlyFire = false, Thing equipment = null, ThingDef targetCoverDef = null)
        {
            base.Launch(launcher, origin, usedTarget, intendedTarget, hitFlags, preventFriendlyFire, equipment, targetCoverDef);
            launcher.DeSpawn();
            innerContainer.TryAdd(launcher);
        }
        protected override void Tick()
        {
            base.Tick();
            if (Spawned && this.Map.IsHashIntervalTick(3))
            {
                foreach (IntVec3 vec3 in GenRadial.RadialCellsAround(Position, 2.5f, true))
                {
                    if (vec3.InBounds(Map))
                    {
                        DamageStuffInCell(vec3);
                    }
                }
            }
        }
        public void DamageStuffInCell(IntVec3 vector)
        {
            tmpThings.Clear();
            tmpThings.AddRange(vector.GetThingList(base.Map));
            for (int i = 0; i < tmpThings.Count; i++)
            {
                tmpThings[i].TakeDamage(new DamageInfo(DefOf_Parasite.ScramSRP_ExplosionBiological, def.projectile.GetDamageAmount(equipment), 0f, 0f, InnerThing));
                if (tmpThings[i] is Pawn)
                {
                    damagedAnything = true;
                }
            }
        }
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            if (!damagedAnything && InnerThing is Pawn pawnThing)
            {
                pawnThing.stances.stunner.StunFor(120, pawnThing);
            }
            base.Impact(hitThing, blockedByShield);
        }
    }
}
