using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace AssemblyParasitesColonThree
{
    public class Gas_InfestedAir : ThingWithComps
    {
        protected int secondarySpawnTick;

        protected float dustMoteSpawnMTB = 0.2f;

        public int density = 15;

        private Effecter sustainedFx;

        private static readonly IntRange DefaultSpawnDelay = new IntRange(1560, 1800);

        protected virtual IntRange ResultSpawnDelay => DefaultSpawnDelay;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref secondarySpawnTick, "secondarySpawnTick", 0);
            Scribe_Values.Look(ref dustMoteSpawnMTB, "dustMoteSpawnMTB", 0.2f);
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                secondarySpawnTick = Find.TickManager.TicksGame + ResultSpawnDelay.RandomInRange;
                if (density >= 1)
                {
                    SpreadGas(this.MapHeld, this.PositionHeld);
                }
            }
            CreateFX();
        }
        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
        }
        protected override void Tick()
        {
            base.Tick();
            if (!base.Spawned || base.Destroyed)
            {
                return;
            }
            sustainedFx?.EffectTick(this, this);
            if (GridsUtility.GetFirstPawn(this.Position, this.Map) != null && this.Map.IsHashIntervalTick(GenTicks.TickRareInterval))
            {
                ParasiteUtils.TryTransmit(GridsUtility.GetFirstPawn(this.Position, this.Map), this, false, 0.1f, "ScramSRP_Infect_Gas");
            }
            if (secondarySpawnTick <= Find.TickManager.TicksGame)
            {
                sustainedFx?.Cleanup();
                Thing.allowDestroyNonDestroyable = true;
                Destroy();
                Thing.allowDestroyNonDestroyable = false;
            }
        }

        private void SpreadGas(Map map, IntVec3 pos)
        {
            foreach (IntVec3 item in GenAdj.CellsAdjacent8Way(this).InRandomOrder())
            {
                if (item.InBounds(map) && !item.Impassable(map))
                {
                    if (item.GetFirstThing(map, DefOf_Parasite.InfectorZoneThingDef) == null)
                    {
                        ParasiteUtils.SpawnInfectorGas(item, map, density - 1);
                    }
                    density--;
                }
            }
        }
        private void CreateFX()
        {
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                sustainedFx = DefOf_Parasite.InfectionZoneEffecterDef.Spawn(this, base.Map);
            });
        }
        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder(base.GetInspectString());
            stringBuilder.AppendLineIfNotEmpty();
            return stringBuilder.ToString();
        }
    }
}
