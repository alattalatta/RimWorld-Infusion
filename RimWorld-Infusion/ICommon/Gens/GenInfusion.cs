using RimWorld;
using Verse;

namespace Infusion
{
	public static class GenInfusion
	{
		public static float GetInfusionChance( QualityCategory qc )
		{
			float result;
			switch ( qc )
			{
				case QualityCategory.Awful:
				case QualityCategory.Shoddy:
				case QualityCategory.Poor:
					result = 0.05f;
					break;
				case QualityCategory.Normal:
					result = 0.1f;
					break;
				case QualityCategory.Good:
					result = 0.2f;
					break;
				case QualityCategory.Superior:
					result = 0.33f;
					break;
				case QualityCategory.Excellent:
					result = 0.45f;
					break;
				case QualityCategory.Masterwork:
					result = 0.67f;
					break;
				case QualityCategory.Legendary:
					result = 0.88f;
					break;
				default:
					result = 0;
					break;
			}

			return result;
		}

		public static InfusionTier GetTier( QualityCategory qc, float multiplier )
		{
			var rand = Rand.Value;
			if ( rand < 0.02*QualityMultiplier( qc )*multiplier )
			{
				return InfusionTier.Artifact;
			}
			if ( rand < 0.045*QualityMultiplier( qc )*multiplier )
			{
				return InfusionTier.Legendary;
			}
			if ( rand < 0.09*QualityMultiplier( qc )*multiplier )
			{
				return InfusionTier.Epic;
			}
			if ( rand < 0.18*QualityMultiplier( qc )*multiplier )
			{
				return InfusionTier.Rare;
			}
			if ( rand < 0.5*QualityMultiplier( qc )*multiplier )
			{
				return InfusionTier.Uncommon;
			}
			return InfusionTier.Common;
		}

		private static float QualityMultiplier( QualityCategory qc )
		{
			return (int) qc/3f;
		}

		/// <summary>
		/// Set parameter targInf to thing's CompInfusion's infusions. Set targInf to null when there is no CompInfusion, or the comp is not infused.
		/// </summary>
		/// <param name="thing"></param>
		/// <param name="targInf"></param>
		/// <returns></returns>
		public static bool TryGetInfusions( this Thing thing, out InfusionSet targInf )
		{
			var comp = thing.TryGetComp< CompInfusion >();
			if ( comp == null )
			{
				targInf = InfusionSet.Empty;
				return false;
			}
			targInf = comp.Infusions;
			return comp.Infused;
		}

		public static bool MatchItemType( this InfusionDef iDef, ThingDef tDef )
		{
			if ( tDef.IsMeleeWeapon )
			{
				return iDef.allowance.melee;
			}
			if ( tDef.IsRangedWeapon )
			{
				return iDef.allowance.ranged;
			}
			return tDef.IsApparel && iDef.allowance.apparel;
		}

		public static InfusionDef ToInfusionDef( this string defName )
		{
			return defName != null ? DefDatabase< InfusionDef >.GetNamed( defName ) : null;
		}

		public static StatDef ToStatDef( this string defName )
		{
			return defName != null ? DefDatabase< StatDef >.GetNamed( defName ) : null;
		}
	}
}
