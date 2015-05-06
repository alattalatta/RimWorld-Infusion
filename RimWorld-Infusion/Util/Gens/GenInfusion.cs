using RimWorld;
using Verse;

namespace Infusion
{
	public static class GenInfusion
	{
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

		public static InfusionDef ToInfusionDef(this string defName)
		{
			return defName != null ? DefDatabase<InfusionDef>.GetNamed(defName) : null;
		}

		public static StatDef ToStatDef(this string defName)
		{
			return defName != null ? DefDatabase<StatDef>.GetNamed(defName) : null;
		}

		public static int Int(this QualityCategory it)
		{
			return (int) it;
		}
	}
}
