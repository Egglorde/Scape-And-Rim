using AssemblyParasitesColonThree.Comps;
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
    public class Thing_BiomassParasite : ThingWithComps, IThingHolder
    {
        protected ThingOwner<Pawn> innerContainer;

        public Pawn spawner;

        public int gestationPeriod;

        public int spawnTick;

        public int DestructionTick => spawnTick + gestationPeriod;

        public int TicksLeft => DestructionTick - Find.TickManager.TicksGame;
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad && !BeingTransportedOnGravship)
            {
                spawnTick = Find.TickManager.TicksGame;
            }
        }
        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return innerContainer;
        }
        public Thing_BiomassParasite()
        {
            innerContainer = new ThingOwner<Pawn>(this, oneStackOnly: false);
        }
        protected override void Tick()
        {
            base.Tick();
            if (TicksLeft < 0)
            {
                innerContainer.TryDropAll(Position, Map, ThingPlaceMode.Near);
                Destroy();
            }
        }
        public override string GetInspectString()
        {
            string text = base.GetInspectString();
            string str = innerContainer.ContentsString;
            if (!text.NullOrEmpty())
            {
                text += "\n";
            }
            return text + ("CasketContains".Translate() + ": " + str.CapitalizeFirst());
        }
    }
}
