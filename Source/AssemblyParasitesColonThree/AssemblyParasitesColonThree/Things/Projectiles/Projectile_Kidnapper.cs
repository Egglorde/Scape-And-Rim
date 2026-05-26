using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AssemblyParasitesColonThree.Things.Projectiles
{
    public class Projectile_Kidnapper : Projectile_Junk
    {
        private MoteDualAttached mote;
        protected virtual IntRange ExpiryInterval_Melee => new IntRange(360, 480);
        public override void DynamicDrawPhaseAt(DrawPhase phase, Vector3 drawLoc, bool flip = false)
        {
            float num = def.projectile.arcHeightFactor * GenMath.InverseParabola(DistanceCoveredFractionArc);
            Vector3 vector = drawLoc + new Vector3(0f, 0f, 1f) * num;
            if (InnerThing != null)
            {
                InnerThing.DynamicDrawPhaseAt(phase, vector);
            } else
            {
	            if (phase == DrawPhase.Draw)
	            {
		            DrawAt(drawLoc, flip);
	            }
                base.DynamicDrawPhaseAt(phase, vector);
            }
        }
        public override void Launch(Thing launcher, Vector3 origin, LocalTargetInfo usedTarget, LocalTargetInfo intendedTarget, ProjectileHitFlags hitFlags, bool preventFriendlyFire = false, Thing equipment = null, ThingDef targetCoverDef = null)
        {
            base.Launch(launcher, origin, usedTarget, intendedTarget, hitFlags, preventFriendlyFire, equipment, targetCoverDef);
            Vector3 offsetA = (ExactPosition - launcher.Position.ToVector3Shifted()).Yto0().normalized * def.projectile.beamStartOffset;
            if (def.projectile.beamMoteDef != null)
            {
                mote = MoteMaker.MakeInteractionOverlay(def.projectile.beamMoteDef, launcher, this, offsetA, Vector3.zero);
            }
        }
        protected override void Tick()
        {
            base.Tick();
            if (Launcher?.Map != this.Map)
            {
                base.Impact(null, false);
            }
            if (mote != null)
            {
                mote.Maintain();
            }

        }
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            GenClamor.DoClamor(this, 12f, ClamorDefOf.Impact);
            if (!blockedByShield && def.projectile.landedEffecter != null)
            {
                def.projectile.landedEffecter.Spawn(base.Position, base.Map).Cleanup();
            }
            if (hitThing != Launcher && hitThing is Pawn {def.race.doesntMove: false} && InnerThing == null)
            {
                InnerThing = hitThing;
                Launch(Launcher, hitThing.Position.ToVector3Shifted(), Launcher, Launcher, ProjectileHitFlags.All);
                return;
            }
            base.Impact(hitThing, blockedByShield);
            if (launcher is Pawn pawn && InnerThingRef != null)
            {
                Job job = JobMaker.MakeJob(JobDefOf.AttackMelee, InnerThingRef);
                job.expiryInterval = ExpiryInterval_Melee.RandomInRange;
                job.checkOverrideOnExpire = true;
                job.expireRequiresEnemiesNearby = true;
                pawn.jobs.StartJob(job, JobCondition.InterruptOptional);
            }
        }

    }
}
