using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AssemblyParasitesColonThree
{
    public class InfectedFormTypeDef : HediffDef
    {
        public ThinkTreeDef thinkTree;
        public ThinkTreeDef thinkTreeConstant;
        // Will always remove ideo and aging.
        [MustTranslate]
        public string namePrefix = "";
        // Infected variants will only have the biomass need, so no other needs are needed, (geddit?)
        public List<InfectedFormDef> formDefs;
        public Color? colorOverride;
        public PawnKindDef headBasePawnKind;
        public List<BodyTypeGraphicData> bodyTypeGraphicPaths = new List<BodyTypeGraphicData>();
        public List<HeadTypeDef> forcedHeadTypes;
        // Hair and beard tags filters are not needed because they will always be bald
        public bool needsflesh = true;
        public bool weaponuser = false;
        public bool appareluser = false;
        public string GetBodyGraphicPath(Pawn pawn)
        {
            for (int i = 0; i < bodyTypeGraphicPaths.Count; i++)
            {
                if (bodyTypeGraphicPaths[i].bodyType == pawn.story.bodyType)
                {
                    return bodyTypeGraphicPaths[i].texturePath;
                }
            }
            return null;
        }
    }
}
