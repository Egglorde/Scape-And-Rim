using AssemblyParasitesColonThree.CompProps;
using AssemblyParasitesColonThree.Hediffs;
using AssemblyParasitesColonThree.MapRelated;
using AssemblyParasitesColonThree.Misc;
using AssemblyParasitesColonThree.Things;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
using static HarmonyLib.Code;

namespace AssemblyParasitesColonThree.Comps
{
    public class Comp_Adaptation : ThingComp
    {
        // WE ARE THE BORG. YOU WILL BE ASSIMILATED. YOUR UNIQUENESS WILL BE ADDED TO OUR COLLECTIVE. RESISTANCE IS FUTILE. 
        public CompProperties_Adaptation Props => (CompProperties_Adaptation)props;

        Dictionary<ThingDef, int> ThingDefsLearned = new Dictionary<ThingDef, int>();
        public override void PostPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            if (parent is Pawn pawnParent && !pawnParent.health.hediffSet.HasHediff(DefOf_Parasite.ScramSRP_Adaptation))
            {
                pawnParent.health.AddHediff(DefOf_Parasite.ScramSRP_Adaptation);
            }
            if (dinfo.Weapon != null)
            {
                if (HasAdaptationOrCanApply(dinfo.Weapon)) dinfo.SetAmount(ApplyAdaptation(dinfo, dinfo.Weapon));
            } else if (dinfo.Instigator != null)
            {
                if (HasAdaptationOrCanApply(dinfo.Instigator.def)) dinfo.SetAmount(ApplyAdaptation(dinfo, dinfo.Instigator.def));
            }
            if (dinfo.Amount == 0)
            {
                absorbed = true;
            }
            absorbed = false;
        }
        private float ApplyAdaptation(DamageInfo dinfo, ThingDef thing)
        {
            if (Props.learnChance <= Rand.Range(0.0f, 1.0f)) return dinfo.Amount;
            if (parent.HasAttachment(ThingDefOf.Fire) && Props.failChance >= Rand.Range(0.0f, 1.0f)) return dinfo.Amount;
            if (ThingDefsLearned[thing] <= Props.reductionCap)
            {
                DefOf_Parasite.ScramSRP_AdaptationIncompleteSound.PlayOneShot(new TargetInfo(parent.Position, parent.Map));
                Thing mote = MoteMaker.MakeAttachedOverlay(parent, DefOf_Parasite.ScramSRP_Mote_AdaptationIncomplete, Vector3.zero);
                ThingDefsLearned[thing] += 1;
            }
            else
            {
                DefOf_Parasite.ScramSRP_AdaptationCompleteSound.PlayOneShot(new TargetInfo(parent.Position, parent.Map));
                Thing mote = MoteMaker.MakeAttachedOverlay(parent, DefOf_Parasite.ScramSRP_Mote_AdaptationComplete, Vector3.zero);
            }
            float newAmount = (1f - (ThingDefsLearned[thing] * Props.reductionPerPoint)) * dinfo.Amount;
            return newAmount;
        }
        private bool HasAdaptationOrCanApply(ThingDef thing)
        {
            if (ThingDefsLearned.ContainsKey(thing))
            {
                return true;
            } else if (ThingDefsLearned.Count < Props.maxLearnedDamageSources)
            {
                ThingDefsLearned.Add(thing, 0);
                return true;
            }
            return false;
        }
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<ThingDef, int> kv in ThingDefsLearned)
            {
                stringBuilder.Append(kv.Key.label.CapitalizeFirst() + " " + (kv.Value * 100 * Props.reductionPerPoint) + "%");
                if (kv.Key != ThingDefsLearned.Last().Key)
                {
                    stringBuilder.Append(", ");
                }
            }
            return stringBuilder.ToString();
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref ThingDefsLearned, "ScramSRP_ThingDefsLearned");
        }
    }
}