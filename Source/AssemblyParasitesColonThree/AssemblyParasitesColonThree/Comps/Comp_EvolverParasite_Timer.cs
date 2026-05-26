using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace AssemblyParasitesColonThree.Comps
{
    internal class Comp_EvolverParasite_Timer : Comp_EvolverParasite
    {
        public int spawnTick;
        public int DestructionTick => spawnTick + Props.ReqTimeTicks;
        public CompProperties_EvolverParasite_Timer Props => (CompProperties_EvolverParasite_Timer)props;
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad && !parent.BeingTransportedOnGravship)
            {
                spawnTick = Find.TickManager.TicksGame;
            }
        }
        public override void CompTick()
        {
            base.CompTick();
            if (Find.TickManager.TicksGame >= DestructionTick)
            {
                EvolvePawn(parent, Props.evolutionTarget);
            }
        }
        public override string CompInspectStringExtra()
        {
            int numTicks = Mathf.Max(0, DestructionTick - Find.TickManager.TicksGame);
            if (numTicks <= 0)
            {
                return "Can evolve now";
            }
            return "Evolves in : " + numTicks.ToStringTicksToPeriod();
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref spawnTick, "spawnTick", 0);
        }
    }
}
