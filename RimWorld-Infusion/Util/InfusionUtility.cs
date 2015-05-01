using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Infusion
{
	public static class InfusionUtility
	{
		public static Dictionary<InfusionTypes, Dictionary<StatDef, StatMod>> dictModifier;

		public static void Init()
		{
			dictModifier = new Dictionary<InfusionTypes, Dictionary<StatDef, StatMod>>();
		}

		public static void AddModifier(this InfusionTypes infType, StatDef statDef, StatMod statMod)
		{
			var dict = new Dictionary<StatDef, StatMod> {{statDef, statMod}};
			dictModifier.Add(infType, dict);
		}

		public static void GetModifier(this InfusionTypes infType, List<StatDef> statDef, out List<StatMod> statMod)
		{
			statMod = new List<StatMod>();

			foreach (var current in statDef)
			{
				if(dictModifier[infType].ContainsKey(current))
					statMod.Add(dictModifier[infType][current]);
			}
		}

		public static bool TryGetInfusion(this Thing thing, out InfusionTypes infType)
		{
			var compInfusion = thing.TryGetComp<CompInfusion>();
			if (compInfusion == null)
			{
				infType = InfusionTypes.None;
				return false;
			}

			infType = compInfusion.Infusion;
			return infType != InfusionTypes.None;
		}

		public static string GetInfusedLabel(this Thing thing)
		{
			InfusionTypes infType;
			if (!TryGetInfusion(thing, out infType))
				return null;

			QualityCategory qc;
			thing.TryGetQuality(out qc);
			var result =
				 thing.Stuff.stuffProps.stuffAdjective + " " + thing.def.label + " of " + GetInfusionLabel(infType) + " (" + qc.GetLabel() + ")" ;
			return result.CapitalizeFirst();
		}

		public static string GetInfusedLabelShort(this Thing thing)
		{
			InfusionTypes infType;
			if (!TryGetInfusion(thing, out infType))
				return null;

			QualityCategory qc;
			thing.TryGetQuality(out qc);
			var result =
				thing.def.label + " of " + GetInfusionLabel(infType);
			return result.CapitalizeFirst();
		}

		public static string GetInfusionLabel(this InfusionTypes infType)
		{
			return infType.ToString().ToLower();
		}

		public static string GetInfusionDescription(this InfusionTypes infType)
		{
			switch (infType)
			{
				case InfusionTypes.Shock:
					return "Small bonus to damage and cooldown.";
				case InfusionTypes.Impact:
					return "Bonus to damage.";
				case InfusionTypes.Needle:
					return "Bonus to hit chance.\nSmall penalty to damage.";
				case InfusionTypes.Charisma:
					return "Small bonus to social skills.";

				case InfusionTypes.Fire:
					return "Small bonus to damage and minimum comfortable temperature.\nSmall penalty to maximum temperature.";
				case InfusionTypes.Water:
					return "Small bonus to minimum and maximum comfortable temperature.\nSmall penalty to damage.";
				case InfusionTypes.Plain:
					return "Bonus to sowing and harvesting. Small bonus to cooldown.";
				case InfusionTypes.Rock:
					return
						"Small bonus to mental break threshold and psychic sensitivity.\nPenalty to cooldown and social skills. Small penalty to global work speed.";
				case InfusionTypes.Creation:
					return "Small bonus to global work speed.";
				case InfusionTypes.Stream:
					return "Big bonus to cooldown.\nPenalty to hit chance.";

				case InfusionTypes.Sunlight:
					return "Bonus to attack and hit chance.";
				case InfusionTypes.Starlight:
					return "Bonus to cooldown. Small bonus to hit chance.";
				case InfusionTypes.Pain:
					return
						"Huge bonus to damage.\nPenalty to psychic sensitivity. Small penalty to mental break threshold and immunity gain speed.";

				default:
					throw new ArgumentOutOfRangeException(infType.ToString());
			}
		}
	}

	public static class MathInfusion
	{
		public static int Rand(InfusionTypes begin, InfusionTypes end)
		{
			return Verse.Rand.Range((int)begin + 1, (int)end);
		}

		public static float ToAbs(this float f)
		{
			return Mathf.Abs(f);
		}
	}
}
