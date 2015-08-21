using System.Linq;
using Verse;

namespace Infusion
{
	public static class StaticSet
	{
		public static bool FindModActive(string modName)
		{
			return LoadedModManager.LoadedMods.ToList().Exists(s => s.name == modName);
		}

		//Mote
		public static string StringInfused = "Infused".Translate();
		
		/**
		 * CompInfusion
		 */
		//Your weapon, {0}, is infused!
		public static string StringInfusionMessage = "InfusionMessage";

		/**
		 * GenInfusion
		 */
		//{1: golden sword} of {2: stream}
		public static string StringInfusionOf = "InfusionOf";

		/**
		 * StatParts
		 */
		//Infusion bonuses
		public static string StringInfusionDescBonus = "InfusionDescBonus".Translate();
		public static string StringInfusionDescFrom = "InfusionDescFrom";

		/**
		 * ITab
		 */
		public static string StringQuality = "Quality".Translate();


		public static string StringThisApparel = "ThisApparel".Translate();
		public static string StringThisWeapon = "ThisWeapon".Translate();
	}
}
