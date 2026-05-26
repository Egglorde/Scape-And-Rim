using AssemblyParasitesColonThree.CompProps;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static UnityEngine.GraphicsBuffer;

namespace AssemblyParasitesColonThree.Comps
{
    public class AbilityComp_OrbComp : CompAbilityEffect
    {
        public CompProperties_OrbUser Props2 => parent.pawn.GetComp<Comp_OrbUser>().Props;
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Pawn pawn = target.Pawn;
            if (pawn == null)
                return;
            if (pawn == parent.pawn && Props2.selfHediffs != null)
                ApplyHediffs(Props2.selfHediffs, pawn);
            else if (pawn.Faction == parent.pawn.Faction && Props2.allyHediffs != null)
                ApplyHediffs(Props2.allyHediffs, pawn);
            else if (pawn.Faction != parent.pawn.Faction && Props2.enemyHediffs != null)
                ApplyHediffs(Props2.enemyHediffs, pawn);
                
        }
        private void ApplyHediffs(List<HediffDef> hediffDefs, Pawn pawn)
        {
            for (int i = 0; i < hediffDefs.Count; i++)
            {
                if (pawn.health.hediffSet.HasHediff(hediffDefs[i]))
                {
                    float potency = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDefs[i]).Severity;
                    pawn.health.hediffSet.GetFirstHediffOfDef(hediffDefs[i]).Severity = potency > Props2.Potency ? potency : Props2.Potency;
                }
                else
                    pawn.health.GetOrAddHediff(hediffDefs[i]).Severity = Props2.Potency;
            }
        }
        public override bool AICanTargetNow(LocalTargetInfo target)
        {
            return parent.pawn.mindState.enemyTarget?.Position.DistanceTo(parent.pawn.Position) <= parent.def.EffectRadius && parent.pawn.mindState.enemyTarget == target;
        }
    }
}
