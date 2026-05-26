using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AssemblyParasitesColonThree.Misc
{
    public class PawnRenderNode_BodyReplacer : PawnRenderNode
    {
        public PawnRenderNode_BodyReplacer(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
            : base(pawn, props, tree)
        {
        }
        public override Graphic GraphicFor(Pawn pawn)
        {
            if (pawn.kindDef.HasModExtension<InfectedFormDefModExtension>())
            {
                Graphic graphic = pawn.kindDef.GetModExtension<InfectedFormDefModExtension>().infectedForm.newGraphicData.Graphic;
                return graphic;
            }
            return null;
        }
    }
}