using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree.Things
{
    public class KillCredit : IExposable
    {
        public float points;
        public void ExposeData()
        {
            Scribe_Values.Look(ref points, "points", defaultValue:1);
        }

        public KillCredit(Pawn pawn, float mul) 
        {
            points = pawn.BodySize * mul;
        }
        public KillCredit() {}
    }
}
