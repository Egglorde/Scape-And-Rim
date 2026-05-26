using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AssemblyParasitesColonThree.Harmony
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var harmony = new HarmonyLib.Harmony("rimworld.scrambled.scapeandrun");

            harmony.Patch(original: AccessTools.Method(typeof(GenRecipe), nameof(GenRecipe.MakeRecipeProducts)),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(MakeRecipeProductsPostfix)));
            harmony.PatchAll();
        }

        public static void MakeRecipeProductsPostfix(ref IEnumerable<Thing> __result,
            RecipeDef recipeDef, Pawn worker, Thing dominantIngredient)
        {
            List<Thing> modifiedProducts = new List<Thing>();
            if (recipeDef == DefOf_Parasite.ButcherCorpseFlesh)
            {
                Thing butcheredCorpse = worker.CurJob.GetTarget(TargetIndex.B).Thing;

                if (butcheredCorpse is Corpse corpse)
                {
                    if (corpse.InnerPawn.health.hediffSet.HasHediff<Hediff_Infected>() )
                    {
                        if (corpse.InnerPawn.health.hediffSet.GetNotMissingParts().Any((BodyPartRecord p) => p.def == BodyPartDefOf.Heart))
                        {
                            ThingDef lootdef = DefOf_Parasite.ScramSRP_DiseasedHeart;
                            Thing loot = ThingMaker.MakeThing(lootdef);
                            loot.stackCount = 1;
                            modifiedProducts.Add(loot);
                        }

                        float efficiency = ((recipeDef.efficiencyStat != null) ? worker.GetStatValue(recipeDef.efficiencyStat) : 1f);
                        foreach (Thing thing in corpse.ButcherProducts(worker, efficiency))
                        {
                            if (thing.def.thingCategories.Contains(ThingCategoryDefOf.MeatRaw))
                            {
                                Thing meat = ThingMaker.MakeThing(DefOf_Parasite.ScramSRP_InfectedFlesh);
                                meat.stackCount = thing.stackCount;
                                modifiedProducts.Add(meat);
                            }
                            else
                            {
                                modifiedProducts.Add(thing);
                            }
                        }
                        __result = modifiedProducts;
                    }
                }
            }
        }


    }
}
