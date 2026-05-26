using RimWorld;
using Verse;
using Verse.AI;

namespace AssemblyParasitesColonThree
{
    [DefOf]
    public static class DefOf_Parasite
    {
        public static RecipeDef ButcherCorpseFlesh;
        public static FactionDef ParasiteFaction;
        public static PawnKindDef ScramSRP_Beckon;
        public static PawnKindDef ScramSRP_Beckon2;
        public static PawnKindDef ScramSRP_Buglin;
        public static ThingDef BiomassSmall;
        public static ThingDef TunnelBeckonSpawner;
        public static ThingDef ScramSRP_Giblet;
        public static HediffDef ScramSRP_HediffCOTHDef;
        public static HediffDef ScramSRP_Conversion;
        public static HediffDef ScramSRP_Regen;
        public static HediffDef ScramSRP_Dissolved;
        public static HediffDef ScramSRP_Adaptation;
        public static InfectedFormTypeDef ScramSrp_Sim;

        public static PawnRenderNodeTagDef BodyReplacer;

        public static FleshTypeDef ScramSRP_ParasiteFlesh;
        // Effecter defs
        public static EffecterDef InfectionZoneEffecterDef;
        public static EffecterDef ScramSRP_AssimilationEffecter;
        public static EffecterDef ScramSRP_ResurrectionEffecter;
        public static EffecterDef ScramSRP_BiomassEffecter;
        public static EffecterDef ScramSRP_BiomassShareEffecter;

        // Other shit idk
        public static ThingDef InfectorZoneThingDef;
        public static ThingDef ScramSRP_PawnBurrower;
        public static ThingDef ScramSRP_ParasiteBrambles;
        public static ThingDef ScramSRP_TransformationCutscene;
        public static TerrainDef ScramSRP_InfestedSoil;
        public static JobDef ScramSRP_GestateNeutral;
        public static ThingDef ScramSRP_Filth_ResidueInfested;
        public static ThingDef BeckonRace;
        public static ThingDef ScramSRP_DiseasedHeart;
        public static ThingDef ScramSRP_InfectedFlesh;
        public static ThingDef ScramSRP_ParasiteGoreLarge;
        public static ThingDef Beckon2Race;

        // LetterDefs
        public static LetterDef ScramSRP_AssimEvent;
        public static LetterDef ScramSRP_NodeEvent;
        public static LetterDef ScramSRP_AssimWorldEvent;


        public static JobDef CastAbilityOnThingMelee;
        public static JobDef ScramSRP_ConvertDowned;
        public static JobDef ScramSRP_Dissolve;
        public static JobDef ScramSRP_Merge;
        public static JobDef ScramSRP_SimEvolve;
        public static JobDef ScramSRP_ConsumeDead;
        public static MutantDef ScramSRP_AssimilatedMutant;
        public static AnimationDef ScramSRP_ExplodeStart;
        public static AnimationDef ScramSRP_ExplodeMiddle;
        public static AnimationDef ScramSRP_ExplodeEnd;
        public static AnimationDef ScramSRP_Resurrect;
        public static ThingDef ScramSRP_ExplodingPawn;
        public static DamageDef ScramSRP_ExplosionBiological;

        // Pawn Duties
        public static DutyDef ScramSRP_Merge_Dominant;
        public static DutyDef ScramSRP_Merge_Submissive;
        public static DutyDef ScramSRP_Riot_Frenzy;
        // Motes
        public static ThingDef ScramSRP_Mote_AdaptationIncomplete;
        public static ThingDef ScramSRP_Mote_AdaptationComplete;

        // Sounds
        public static SoundDef ScramSRP_AdaptationCompleteSound;
        public static SoundDef ScramSRP_AdaptationIncompleteSound;
        public static SoundDef ScramSRP_AssimSound;
        public static SoundDef TunnelBell;

        // Primitive Parasites
        public static PawnKindDef ScramSRP_Pri_LongArmsKind;
        public static PawnKindDef ScramSRP_Pri_SummonerKind;
        public static PawnKindDef ScramSRP_Pri_BolsterKind;
        public static PawnKindDef ScramSRP_Pri_ManducaterKind;
        public static PawnKindDef ScramSRP_Pri_YelloweyeKind;
        public static PawnKindDef ScramSRP_Pri_ReekerKind;
        public static PawnKindDef ScramSRP_Pri_ArachnidaKind;

        // WorldObjects
        public static WorldObjectDef ScramSRP_Meteor;
        public static WorldObjectDef ScramSRP_Vector;
        static DefOf_Parasite()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(DefOf_Parasite));
        }
    }
}
