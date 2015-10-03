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

		public void SetInfusion( bool shouldThrowMote = false )
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

			GenerateInfusion( compQuality.Quality, shouldThrowMote );
			tried = true;
		}

		private void GenerateInfusion( QualityCategory qc, bool shouldThrowMote )
		{
			prefix = null;
			suffix = null;

			var passPre = true;
			var passSuf = true;
			var lowTech = parent.def.techLevel < TechLevel.Midworld;

			var chance = GenInfusion.GetInfusionChance( qc );
			var rand = Rand.Value;
			if ( lowTech )
			{
				rand /= 3;
			}
			if ( rand <= chance )
			{
				passPre = false;
			}

			chance = GenInfusion.GetInfusionChance( qc );
			rand = Rand.Value;
			if ( lowTech )
			{
				rand /= 3;
			}
			if ( rand <= chance )
			{
				passSuf = false;
			}

			if ( passPre && passSuf )
			{
				//None has made this far
				return;
			}

			var tierMult = lowTech ? 3 : 1;

			if ( !passPre )
			{
				InfusionDef preTemp;
				var tier = GenInfusion.GetTier( qc, tierMult );
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
					prefix = null;
				}
				else
				{
					prefix = preTemp.defName;
				}
			}

			if ( !passSuf )
			{
				InfusionDef preTemp;
				var tier = GenInfusion.GetTier( qc, tierMult );
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
					suffix = null;
				}
				else
				{
					suffix = preTemp.defName;
				}
			}

			//For additional hit points
			parent.HitPoints = parent.MaxHitPoints;

			if ( !shouldThrowMote )
			{
				return;
			}

			var msg = new StringBuilder();
			msg.Append( qc.GetLabel() + " " );
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

			if ( (prefix != null && prefix.ToInfusionDef() == null) || (suffix != null && suffix.ToInfusionDef() == null) )
			{
#if DEBUG
				Log.Warning( "LT-IN: Could not find some of InfusionDef." + prefix + "/" + suffix );
#endif
				tried = false;
			}
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
			return base.GetDescriptionPart() + "\n" + parent.GetInfusionDesc();
		}
	}
}