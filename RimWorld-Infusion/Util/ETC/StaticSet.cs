using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Infusion
{
	public static class StaticSet
	{
		public static readonly Color ColorTier1 = new Color(1f, 0.75f, 0);
		public static readonly Color ColorTier2 = new Color(1f, 0.5f, 0);
		public static readonly Color ColorTier3 = new Color(1f, 0.25f, 0);

		//Mote
		public static string StringInfused = "Infused".Translate();
		
		/**
		 * CompInfusion
		 */
		//Your weapon, {0}, is infused!
		public static string StringInfusionMessage = "InfusionMessage";
		//This specific weapon has more potential than others.\nYour colonists had named it as {0}.
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
		 * GenInfusion
		 */
		//{1: golden sword} of {2: stream}
		public static string StringInfusionOf = "InfusionOf";

		/**
		 * StatParts
		 */
		//Infusion bonuses
		public static string StringInfusionDescBonus = "InfusionDescBonus".Translate();
	}
}
