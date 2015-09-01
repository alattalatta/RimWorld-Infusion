using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.Sound;

namespace Infusion
{
    public class CompInfusion : ThingComp
    {
        public bool Infused => prefix != null || suffix != null;

        public InfusionSet Infusions => new InfusionSet( prefix, suffix );

        //Did we tried to infuse this item?
        public bool tried;

        private string prefix, suffix;

        private static readonly SoundDef InfusionSound = SoundDef.Named( "Infusion_Infused" );

        public void SetInfusion( bool shouldFireMote = false )
        {
            if ( tried )
            {
                return;
            }
            var compQuality = parent.GetComp< CompQuality >();
            if ( compQuality == null )
            {
                tried = true;
                return;
            }

            var qc = compQuality.Quality;
            if ( qc <= QualityCategory.Normal )
            {
                prefix = null;
                suffix = null;
                tried = true;
            }
            else
            {
                GenerateInfusion( qc, shouldFireMote );
                tried = true;
            }
        }

        private float GetChance( QualityCategory qc, InfusionType type )
        {
            float result;
            switch ( qc )
            {
                //									  Pre : Suf
                case QualityCategory.Good:
                    result = type == InfusionType.Prefix ? 27 : 33;
                    break;
                case QualityCategory.Superior:
                    result = type == InfusionType.Prefix ? 36 : 41;
                    break;
                case QualityCategory.Excellent:
                    result = type == InfusionType.Prefix ? 50 : 59;
                    break;
                case QualityCategory.Masterwork:
                    result = type == InfusionType.Prefix ? 68 : 73;
                    break;
                case QualityCategory.Legendary:
                    result = type == InfusionType.Prefix ? 72 : 80;
                    break;
                default:
                    result = 0;
                    break;
            }
            if ( parent.def.IsRangedWeapon )
            {
                result *= 0.90f;
            }
            else if ( parent.def.IsApparel )
            {
                result *= 0.80f;
            }

            return result;
        }

        private static InfusionTier GetTier( QualityCategory qc, InfusionType type )
        {
            var rand = Rand.Range( 0, 100 );
            //												   Pre : Suf
            if ( rand > (type == InfusionType.Prefix ? 48 + (int) qc : 43 + (int) qc) )
            {
                return InfusionTier.Uncommon;
            }
            if ( rand > (type == InfusionType.Prefix ? 19 + (int) qc : 13 + (int) qc) )
            {
                return InfusionTier.Rare;
            }
            if ( rand > (type == InfusionType.Prefix ? 6 : 4) )
            {
                return InfusionTier.Epic;
            }
            if ( rand > (type == InfusionType.Prefix ? 2 : 1) )
            {
                return InfusionTier.Legendary;
            }
            return InfusionTier.Artifact;
        }

        private void GenerateInfusion( QualityCategory qc, bool shouldThrowMote )
        {
            bool passPre = false, passSuf = false;

            var chance = GetChance( qc, InfusionType.Prefix );
            var rand = Rand.Range( 0, 100 );

            if ( rand >= chance )
            {
                passPre = true;
            }

            chance = GetChance( qc, InfusionType.Suffix );
            rand = Rand.Range( 0, 100 );

            if ( rand >= chance )
            {
                passSuf = true;
            }

            if ( passPre && passSuf )
            {
                //None has made this far
                return;
            }

            if ( !passPre )
            {
                InfusionDef preTemp;
                var tier = GetTier( qc, InfusionType.Prefix );
                if (
                    !(
                        from t in DefDatabase< InfusionDef >.AllDefs.ToList()
                        where
                            t.tier == tier &&
                            t.type == InfusionType.Prefix &&
                            t.MatchItemType( parent.def )
                        select t
                        ).TryRandomElement( out preTemp ) )
                {
                    //No infusion available from defs
                    Log.Warning( "Couldn't find any prefixed InfusionDef! Tier: " + tier );
                    shouldThrowMote = false;
                }
                prefix = preTemp.defName;
            }

            if ( !passSuf )
            {
                InfusionDef preTemp;
                var tier = GetTier( qc, InfusionType.Suffix );
                if ( !
                    (from t in DefDatabase< InfusionDef >.AllDefs.ToList()
                     where
                         t.tier == tier &&
                         t.type == InfusionType.Suffix &&
                         t.MatchItemType( parent.def )
                     select t
                        ).TryRandomElement( out preTemp ) )
                {
                    //No infusion available from defs
                    Log.Warning( "Couldn't find any suffixed InfusionDef! Tier: " + tier );
                    shouldThrowMote = false;
                }
                suffix = preTemp.defName;
            }

            //For additional hit points
            parent.HitPoints = parent.MaxHitPoints;

            if ( !shouldThrowMote )
            {
                return;
            }

            var msg = new StringBuilder();
            msg.Append( ResourceBank.QualityAsTranslated( qc ) + " " );
            if ( parent.Stuff != null )
            {
                msg.Append( parent.Stuff.LabelAsStuff + " " );
            }
            msg.Append( parent.def.label );
            Messages.Message( ResourceBank.StringInfusionMessage.Translate( msg ), MessageSound.Silent );
            InfusionSound.PlayOneShotOnCamera();
            MoteThrower.ThrowText( parent.Position.ToVector3Shifted(), ResourceBank.StringInfused,
                                   GenInfusionColor.Legendary );
        }

        public override void PostSpawnSetup()
        {
            base.PostSpawnSetup();
            SetInfusion( true );
            if ( Infused )
            {
                InfusionLabelManager.Register( this );
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            
            Scribe_Values.LookValue( ref tried, "tried", false );
            Scribe_Values.LookValue( ref prefix, "prefix", null );
            Scribe_Values.LookValue( ref suffix, "suffix", null );
        }

        public override void PostDeSpawn()
        {
            base.PostDeSpawn();

            if ( Infused )
            {
                InfusionLabelManager.DeRegister( this );
            }
        }

        public override bool AllowStackWith( Thing other )
        {
            if ( other.TryGetComp< CompInfusion >() == null )
            {
                return false;
            }

            InfusionSet otherSet;
            other.TryGetInfusions( out otherSet );

            return Infusions.Equals( otherSet );
        }

        public override void PostSplitOff( Thing piece )
        {
            base.PostSplitOff( piece );
            piece.TryGetComp< CompInfusion >().tried = tried;
            piece.TryGetComp< CompInfusion >().prefix = prefix;
            piece.TryGetComp< CompInfusion >().suffix = suffix;
        }

        public override string GetDescriptionPart()
        {
            return base.GetDescriptionPart() + parent.GetInfusedDescription() + "\n" +
                   parent.GetInfusedDescriptionITab();
        }
    }
}
