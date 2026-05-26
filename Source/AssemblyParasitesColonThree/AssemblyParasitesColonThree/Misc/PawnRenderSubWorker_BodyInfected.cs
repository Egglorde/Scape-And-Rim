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
    public class PawnRenderSubWorker_BodyInfected : PawnRenderSubWorker
    {
        public override void EditMaterial(PawnRenderNode node, PawnDrawParms parms, ref Material material)
        {
            if (parms.pawn.GetRotStage() != RotStage.Dessicated && node.tree.TryGetNodeByTag(DefOf_Parasite.BodyReplacer, out PawnRenderNode node2))
            {
                material = node2.PrimaryGraphic?.NodeGetMat(parms) ?? material;
            }
        }
        public override void TransformScale(PawnRenderNode node, PawnDrawParms parms, ref Vector3 scale)
        {
            if (node.tree.TryGetNodeByTag(DefOf_Parasite.BodyReplacer, out PawnRenderNode node2) && node2.PrimaryGraphic != null)
            {
                Vector2 scale2 = parms.pawn.kindDef.GetModExtension<InfectedFormDefModExtension>().infectedForm.newGraphicData.drawSize;
                scale.z *= scale2.y;
                scale.x *= scale2.x;
            }
        }
    }
}
