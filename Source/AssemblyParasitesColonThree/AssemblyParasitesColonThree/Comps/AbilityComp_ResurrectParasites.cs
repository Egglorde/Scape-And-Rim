using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AssemblyParasitesColonThree.Comps
{
    public class AbilityComp_ResurrectParasites : CompAbilityEffect
    {
        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (!base.CanApplyOn(target, dest))
            {
                return false;
            }
            if (!target.HasThing)
            {
                return false; 
            }
            if (target.Thing is Corpse corpse)
            {
                if (corpse.GetRotStage() == RotStage.Fresh && corpse.InnerPawn.Faction == parent.pawn.Faction)
                {
                    return true;
                }
                return false;
            }
            if (target.Thing.HasComp<Comp_Remain>())
            {
                return true;
            }
            return false;
        }
        public override IEnumerable<PreCastAction> GetPreCastActions()
        {
            yield return new PreCastAction
            {
                action = delegate(LocalTargetInfo a, LocalTargetInfo b)
                {
                    if (a.Thing is Corpse corpse)
                    {
                        corpse.InnerPawn.Drawer.renderer.SetAnimation(DefOf_Parasite.ScramSRP_Resurrect);
                    }
                    Effecter effecter = DefOf_Parasite.ScramSRP_ResurrectionEffecter.Spawn(a.Thing, a.Thing.MapHeld, Vector2.zero);
                    a.Thing.MapHeld.effecterMaintainer.AddEffecterToMaintain(effecter, a.Thing, 300);
                }
            };
        }
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            Log.Message("Something Something apply");
            if (target.Thing is Corpse corpse)
            {
                ResurrectionUtility.TryResurrect(corpse.InnerPawn);
                corpse.InnerPawn.Drawer.renderer.SetAnimation(null);
            } else if (ThingCompUtility.TryGetComp(target.Thing, out Comp_Remain comp))
            {
                comp.Resurrect(target.Thing.Map);
            }
        }
    }
}
