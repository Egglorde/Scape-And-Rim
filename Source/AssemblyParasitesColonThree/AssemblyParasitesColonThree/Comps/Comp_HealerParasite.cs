using AssemblyParasitesColonThree.CompProps;
using AssemblyParasitesColonThree.MapRelated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static UnityEngine.GraphicsBuffer;

namespace AssemblyParasitesColonThree.Comps
{
    public class Comp_HealerParasite : ThingComp
    {
        public CompProperties_HealerParasite Props => (CompProperties_HealerParasite)props;
        public Effecter effecter;
        public Effecter Effecter
        {
            get
            {
                if (effecter == null)
                {
                    effecter = Props.effecterDef.SpawnAttached(parent, parent.MapHeld);
                }
                return effecter;
            }
        }
        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
        }
        public override void CompTick()
        {
            if (!parent.IsHashIntervalTick(Props.interval) || !parent.SpawnedOrAnyParentSpawned)
            {
                return;
            }
            foreach (Pawn item in parent.MapHeld.mapPawns.AllPawnsSpawned)
            {
                if (item.Faction == parent.Faction && item.Position.DistanceToSquared(parent.Position) < 256 && item != parent)
                {
                    Hediff hediff = item.health.GetOrAddHediff(DefOf_Parasite.ScramSRP_Regen);
                    hediff.Severity += hediff.Severity < Props.maxRegen ? Props.regenSevGain : 0;
                    Effecter.EffectTick(parent, item);
                }
            }
        }

    }
}
