using System.Collections.Generic;
using Verse;

namespace Infusion
{
	public static class StaticSet
	{
		//Mote
		public static string StringInfused = "Infused".Translate();

		public static List<string> StringInfusionSuffixLabels = new List<string>();
		public static List<string> StringInfusionSuffixLabelsShort = new List<string>();
		public static List<string> StringInfusionSuffixDescriptions = new List<string>();

		public static List<string> StringInfusionPrefixLabels = new List<string>();
		public static List<string> StringInfusionPrefixLabelsShort = new List<string>();
		public static List<string> StringInfusionPrefixDescriptions = new List<string>();
		
		/**
		 * CompInfusion
		 */
		//Full name
		public static string StringInfusionFullName = "InfusionFullName".Translate();
		//This specific weapon has more potential than others.\nYour colonists had named it {0}.
		public static string StringInfusionInfo = "InfusionInfo";
		//This weapon is {0}.
		public static string StringInfusionInfoPrefix = "InfusionInfoPrefix";
		//This weapon has power of {0}.
		public static string StringInfusionInfoSuffix = "InfusionInfoSuffix";
		//Also, it is infused with power of {0}.
		public static string StringInfusionInfoPreSuffix = "InfusionInfoPreSuffix";
		//It will grant user {0}.
		public static string StringInfusionInfoPrefixBonus = "InfusionInfoPrefixBonus";
		//It will add {0}.
		public static string StringInfusionInfoSuffixBonus = "InfusionInfoSuffixBonus";

		/**
		 * InfusionUtility
		 */
		//{1: golden sword} of {2: stream}
		public static string StringInfusionInfusedLabelSuffix = "InfusionInfusedLabelSuffix";

		/**
		 * StatParts
		 */
		//Infusion bonuses
		public static string StringInfusionDescBonus = "InfusionDescBonus".Translate();

		static StaticSet()
		{
			for (var i = 0; i < (int)InfusionPrefix.End; i++)
			{
				//InfusionShock.translate();
				StringInfusionPrefixLabels.Add(("Infusion" + ((InfusionPrefix)i)).Translate());
				//InfusionShockLabel.translate();
				StringInfusionPrefixLabelsShort.Add(("Infusion" + ((InfusionPrefix)i) + "Short").Translate());
				//InfusionShockDescription.translate();
				StringInfusionPrefixDescriptions.Add(("Infusion" + ((InfusionPrefix)i) + "Desc").Translate());
			}
			for (var i = 0; i < (int) InfusionSuffix.End; i++)
			{
				//InfusionShock.translate();
				StringInfusionSuffixLabels.Add(("Infusion" + ((InfusionSuffix)i)).Translate());
				//InfusionShockLabel.translate();
				StringInfusionSuffixLabelsShort.Add(("Infusion" + ((InfusionSuffix)i) + "Short").Translate());
				//InfusionShockDescription.translate();
				StringInfusionSuffixDescriptions.Add(("Infusion" + ((InfusionSuffix)i) + "Desc").Translate());
			}
		}
	}
}
