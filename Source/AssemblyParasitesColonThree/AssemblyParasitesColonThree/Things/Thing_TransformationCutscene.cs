using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using static RimWorld.FleshTypeDef;

namespace AssemblyParasitesColonThree.Things
{
    public class Thing_TransformationCutscene : ThingWithComps, IThingHolder
    {
        public bool DirectAssim;
        protected ThingOwner<Pawn> innerContainer;
        public Pawn InnerPawn
        {
            get
            {
                if (innerContainer.Count <= 0)
                {
                    return null;
                }
                return innerContainer[0];
            }
            set
            {
                if (value == null)
                {
                    innerContainer.Clear();
                    Log.Error("Tried to set innerpawn to a null value");
                    return;
                }
                if (innerContainer.Count > 0)
                {
                    Log.Error("Setting InnerPawn in corpse that already has one.");
                    innerContainer.Clear();
                }
                innerContainer.TryAddOrTransfer(value);
            }
        }
        public Thing_TransformationCutscene()
        {
            innerContainer = new ThingOwner<Pawn>(this, oneStackOnly:true);
        }
        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            DefOf_Parasite.ScramSRP_AssimSound.PlayOneShot(new TargetInfo(Position, Map));
            if (innerContainer.TryDrop(InnerPawn, Position, Map, ThingPlaceMode.Near, out Pawn pawn))
                if (DirectAssim)
                {
                    ParasiteUtils.Turn(pawn, DefOf_Parasite.ScramSrp_Sim, false);
                }
                else
                {
                    ParasiteUtils.CompleteConversion(pawn, pawn.health.hediffSet.GetFirstHediff<Hediff_COTH>());
                }
            base.Destroy(mode);
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }
	    public override void DynamicDrawPhaseAt(DrawPhase phase, Vector3 drawLoc, bool flip = false)
        {
            InnerPawn.DynamicDrawPhaseAt(phase, drawLoc.WithYOffset(InnerPawn.Drawer.SeededYOffset));
        }
        public ThingOwner GetDirectlyHeldThings()
        {
            return innerContainer;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
            Scribe_Values.Look(ref DirectAssim, "DirectAssim");
        }
    }
}
