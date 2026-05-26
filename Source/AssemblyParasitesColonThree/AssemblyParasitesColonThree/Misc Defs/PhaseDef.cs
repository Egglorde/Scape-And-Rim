using AssemblyParasitesColonThree.MapRelated;
using AssemblyParasitesColonThree.Misc.Makers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using Verse;

namespace AssemblyParasitesColonThree.Misc_Defs
{
    public class PhaseDef : Def
    {
        public List<ParasiteGroupMaker> groupMakers;
        public List<ParasiteGroupMaker> incompleteGroupMakers;
        public int phase;
        public int mincount;
        public int maxcount;
        public bool allowSims = false;
        public LetterDef eventLetter;
        public string eventLetterName;

        public List<ParasiteGroupMaker> GetGroupMakers()
        {
            // Adds the list of assimilated pawns to the potential groupMakers of the phase
            List<ParasiteGroupMaker> tmpGroupMakers = groupMakers;
            if (allowSims)
            {
                tmpGroupMakers.AddRange(WorldComp_UbiquitousDevelopment.SimGroupMakers());
            }
            return tmpGroupMakers;
        }
    }
}
