using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AssemblyParasitesColonThree.MapRelated
{
    public class WorldObject_Vector : MapParent
    {
        public float Health = 350;
        public float Radius = 1;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref Radius, "ScramSRP_Radius", defaultValue: 1);
            Scribe_Values.Look(ref Health, "ScramSRP_Health", defaultValue: 350);
        }
    }
}
