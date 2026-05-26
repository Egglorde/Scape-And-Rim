using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Noise;
using Verse.Sound;


namespace AssemblyParasitesColonThree
{
    public class JobDriver_SummonBeckon : JobDriver
    {
        Comp_BeckonSpreadInfection Comp => pawn.GetComp<Comp_BeckonSpreadInfection>();
        CompProperties_BeckonSpreadInfection Props => Comp.Props;
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_General.Wait(Props.spawnticks);
            yield return Toils_General.Do(delegate
            {
                List<Pawn> spawned = new List<Pawn>();
                for (int i = 0; i < Props.pawnsperwave || Comp.SlavePoints() < Props.maxpoints; i++)
                {
                    if (RCellFinder.TryFindRandomCellNearWith(pawn.Position, (IntVec3 c) => !c.Fogged(pawn.Map) && c.Standable(pawn.Map) && c.DistanceTo(pawn.Position) <= Props.radius && c.GetEdifice(pawn.Map) == null && c.GetFirstThing<Thing_BiomassParasite>(pawn.Map) == null, pawn.Map, out var result, 5, Props.radius))
                    {
                        Thing_BiomassParasite thing = ThingMaker.MakeThing(DefOf_Parasite.BiomassSmall) as Thing_BiomassParasite;
                        Pawn child = PawnGenerator.GeneratePawn(new PawnGenerationRequest(Props.spawnablepawns.RandomElement<PawnKindDef>(), ParasiteUtils.OfParasiteFaction, PawnGenerationContext.NonPlayer, null, forceGenerateNewPawn: true, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null));
                        spawned.Add(child);
                        thing.GetDirectlyHeldThings().TryAdd(child);
                        Comp.beckonSlaves.Add(child);
                        thing.spawner = pawn;
                        thing.gestationPeriod = 900;
                        int radius = pawn.GetComp<Comp_BeckonSpreadInfection>().Props.radius;
                        GenPlace.TryPlaceThing(thing, result, pawn.Map, ThingPlaceMode.Near);
                    }
                }
                Lord lord = LordMaker.MakeNewLord(ParasiteUtils.OfParasiteFaction, new LordJob_DefendPoint(pawn.Position, wanderRadius: Props.radius, addFleeToil: false), pawn.Map, spawned);
            });
        }
    }
}
