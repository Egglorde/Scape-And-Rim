using AssemblyParasitesColonThree.Comps;
using AssemblyParasitesColonThree.Things;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree.Harmony
{
    [HarmonyPatch(typeof(Pawn), "Kill")]
    public class KillExplosionPatch
    {
        public static void Postfix(Pawn __instance)
        {
            if (__instance.ParentHolder is Corpse && (__instance.health.hediffSet.TryGetHediff(out Hediff_Infected hediff_Infected) || __instance.TryGetComp(out Comp_DeathExplosionRemain comp)))
            {
                if (0.5 >= Rand.Range(0.0f, 1.0f))
                {
                    hediff_Infected?.Postkill();
                } else
                {
                    Corpse corpse = __instance.Corpse;
                    Explosion_Death_Aneurysm explosion = ThingMaker.MakeThing(DefOf_Parasite.ScramSRP_ExplodingPawn) as Explosion_Death_Aneurysm;
                    GenPlace.TryPlaceThing(explosion, corpse.Position, corpse.Map, ThingPlaceMode.Near);
                    corpse.DeSpawn();
                    explosion.InnerCorpse = corpse;
                }
            }
        }
    }
}
