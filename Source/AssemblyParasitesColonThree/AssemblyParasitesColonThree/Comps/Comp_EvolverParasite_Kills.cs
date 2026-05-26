using AssemblyParasitesColonThree.CompProps;
using AssemblyParasitesColonThree.Things;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static HarmonyLib.Code;

namespace AssemblyParasitesColonThree.Comps
{
    public class Comp_EvolverParasite_Kills : Comp_EvolverParasite
    {
        public List<KillCredit> killCredits = new List<KillCredit>();
        public CompProperties_EvolverParasite_Kills Props => (CompProperties_EvolverParasite_Kills)props;
        public Pawn parentPawn => parent as Pawn;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref killCredits, "killcredits");
        }
        public override void CompTick()
        {
            base.CompTick();
            if (Props.evolutionTarget != null && kills >= Props.ReqKills)
            {
                EvolvePawn(parent, Props.evolutionTarget);
            }
        }
        public override string CompInspectStringExtra()
        {
            return "Current points : " + kills;
        }
        
        public float kills
        {
            get
            {
                float total = 0;
                foreach (KillCredit kill in killCredits)
                {
                    total += kill.points;
                }
                return total;
            }
        }
    }
}
