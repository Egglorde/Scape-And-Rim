using AssemblyParasitesColonThree.Misc;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AssemblyParasitesColonThree.WorkRelated
{
    
    public class JobGiver_AICastAbilityMelee : JobGiver_AICastAbility
    {
        private static readonly SimpleCurve DistanceSquaredToTargetSelectionWeightCurve = new SimpleCurve
        {
            new CurvePoint(50f, 100f),
            new CurvePoint(200f, 10f),
            new CurvePoint(315f, 0.01f)
        };
        protected override LocalTargetInfo GetTarget(Pawn caster, Ability ability)
        {
            IEnumerable<Pawn> mappawns = caster.Map.mapPawns.AllPawns.Where((Pawn p) => p.Spawned && p.AnimalOrWildMan() && !p.Position.Fogged(p.Map) && !p.health.hediffSet.HasHediff<Hediff_COTH>());
            if (mappawns != null && mappawns.TryRandomElementByWeight((Pawn p) => DistanceSquaredToTargetSelectionWeightCurve.Evaluate(p.Position.DistanceToSquared(caster.Position)), out Pawn pawn))
            {
                LocalTargetInfo targetInfo = new LocalTargetInfo(pawn);
                if (caster.CanReserveAndReach(pawn, PathEndMode.Touch, Danger.Some))
                {
                    return targetInfo;
                }
            }
            return LocalTargetInfo.Invalid;
        }
    }
}
