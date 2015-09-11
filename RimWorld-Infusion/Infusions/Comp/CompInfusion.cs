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
            }
            else
            {
                GenerateInfusion( qc, shouldFireMote );
            }
            tried = true;
        }

        private void GenerateInfusion( QualityCategory qc, bool shouldThrowMote )
        {
            bool passPre = false, passSuf = false;

            var chance = GenInfusion.GetInfusionChance( qc );
            var rand = Rand.Range( 0, 100 );

	        if ( parent.def.techLevel < TechLevel.Midworld )
	        {
		        rand /= 10;
	        }
            if ( rand >= chance )
            {
                passPre = true;
            }

            chance = GenInfusion.GetInfusionChance( qc );
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
                var tier = GenInfusion.GetTier( qc );
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
                var tier = GenInfusion.GetTier( qc );
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
			return false;
        }

        public override string GetDescriptionPart()
        {
            return base.GetDescriptionPart() + parent.GetInfusedDescription() + "\n" +
                   parent.GetInfusedDescriptionITab();
        }
    }
}
