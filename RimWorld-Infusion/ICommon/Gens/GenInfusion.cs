using RimWorld;
using Verse;

namespace Infusion
{
	public static class GenInfusion
	{
		public static float GetInfusionChance(QualityCategory qc)
		{
			float result;
			switch (qc)
			{
				case QualityCategory.Awful:
				case QualityCategory.Shoddy:
				case QualityCategory.Poor:
					result = 1;
					break;
				case QualityCategory.Normal:
					result = 3;
					break;
				case QualityCategory.Good:
					result = 10;
					break;
				case QualityCategory.Superior:
					result = 20;
					break;
				case QualityCategory.Excellent:
					result = 35;
					break;
				case QualityCategory.Masterwork:
					result = 55;
					break;
				case QualityCategory.Legendary:
					result = 80;
					break;
				default:
					result = 0;
					break;
			}

			return result;
		}

		public static InfusionTier GetTier(QualityCategory qc)
		{
			var rand = Rand.Value;
			if (rand < 0.3 * QualityMultiplier(qc))
			{
				return InfusionTier.Artifact;
			}
			if (rand < 1.5 * QualityMultiplier(qc))
			{
				return InfusionTier.Legendary;
			}
			if (rand < 5 * QualityMultiplier(qc))
			{
				return InfusionTier.Epic;
			}
			if (rand < 10 * QualityMultiplier(qc))
			{
				return InfusionTier.Rare;
			}
			if (rand < 40 * QualityMultiplier(qc))
			{
				return InfusionTier.Uncommon;
			}
			return InfusionTier.Common;
		}

		private static float QualityMultiplier( QualityCategory qc )
		{
			return (int) qc/2f;
		}

		/// <summary>
		/// Set parameter targInf to thing's CompInfusion's infusions. Set targInf to null when there is no CompInfusion, or the comp is not infused.
		/// </summary>
		/// <param name="thing"></param>
		/// <param name="targInf"></param>
		/// <returns></returns>
		public static bool TryGetInfusions(this Thing thing, out InfusionSet targInf)
		{
			var comp = thing.TryGetComp<CompInfusion>();
			if (comp == null)
			{
				targInf = InfusionSet.Empty;
				return false;
			}
			targInf = comp.Infusions;
			return comp.Infused;
		}

		public static bool MatchItemType(this InfusionDef iDef, ThingDef tDef)
		{
			if (tDef.IsMeleeWeapon)
				return iDef.allowance.melee;
			if (tDef.IsRangedWeapon)
				return iDef.allowance.ranged;
			return tDef.IsApparel && iDef.allowance.apparel;
		}

		public static InfusionDef ToInfusionDef(this string defName)
		{
			return defName != null ? DefDatabase< InfusionDef >.GetNamed(defName) : null;
		}

		public static StatDef ToStatDef(this string defName)
		{
			return defName != null ? DefDatabase< StatDef >.GetNamed(defName) : null;
		}
	}
}
