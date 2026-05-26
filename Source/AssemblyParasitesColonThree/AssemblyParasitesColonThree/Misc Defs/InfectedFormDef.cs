using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree
{
    public class InfectedFormDefModExtension : DefModExtension
    {
        public InfectedFormDef infectedForm;
    }
    public class InfectedFormDef : Def
    {
        public PawnKindDef headPawnKind;
        public GraphicData newGraphicData;
        public bool humanlike;
    }
}
