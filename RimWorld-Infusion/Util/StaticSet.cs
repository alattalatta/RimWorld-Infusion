using System.Collections.Generic;
using Verse;

namespace Infusion
{
	public static class StaticSet
	{
		public static string StringInfused = "Infused".Translate();

		public static List<string> StringInfusionTypes = new List<string>();
		public static List<string> StringInfusionDescriptions = new List<string>();

		static StaticSet()
		{
			for (var i = 0; i < (int) InfusionSuffix.End; i++)
			{
				//InfusionShock.translate();
				StringInfusionTypes.Add(("Infusion" + ((InfusionSuffix)i)).Translate());
				//InfusionShockDescription.translate();
				StringInfusionDescriptions.Add(("Infusion" + ((InfusionSuffix)i) + "Description").Translate());
			}
		}
	}
}
