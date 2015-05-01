using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Infusion
{
	public static class InfusionUtility
	{
		public static Dictionary<InfusionSuffix, Dictionary<StatDef, StatMod>> dictModifier;

		public static void Init()
		{
			dictModifier = new Dictionary<InfusionSuffix, Dictionary<StatDef, StatMod>>();
		}

		public static void AddModifier(this InfusionSuffix infSuffix, StatDef statDef, StatMod statMod)
		{
			var dict = new Dictionary<StatDef, StatMod> {{statDef, statMod}};
			dictModifier.Add(infSuffix, dict);
		}

		public static void GetModifier(this InfusionSuffix infSuffix, List<StatDef> statDef, out List<StatMod> statMod)
		{
			statMod = new List<StatMod>();

			foreach (var current in statDef)
			{
				if(dictModifier[infSuffix].ContainsKey(current))
					statMod.Add(dictModifier[infSuffix][current]);
			}
		}

		public static bool TryGetInfusion(this Thing thing, out InfusionSuffix infSuffix)
		{
			var compInfusion = thing.TryGetComp<CompInfusion>();
			if (compInfusion == null)
			{
				infSuffix = InfusionSuffix.None;
				return false;
			}

			infSuffix = compInfusion.Infusion;
			return infSuffix != InfusionSuffix.None;
		}

		public static string GetInfusedLabel(this Thing thing)
		{
			InfusionSuffix infSuffix;
			if (!TryGetInfusion(thing, out infSuffix))
				return null;

			QualityCategory qc;
			thing.TryGetQuality(out qc);
			var result =
				 thing.Stuff.stuffProps.stuffAdjective + " " + thing.def.label + " of " + GetInfusionLabel(infSuffix) + " (" + qc.GetLabel() + ")" ;
			return result.CapitalizeFirst();
		}

		public static string GetInfusedLabelShort(this Thing thing)
		{
			InfusionSuffix infSuffix;
			if (!TryGetInfusion(thing, out infSuffix))
				return null;

			QualityCategory qc;
			thing.TryGetQuality(out qc);
			var result =
				thing.def.label + " of " + GetInfusionLabel(infSuffix);
			return result.CapitalizeFirst();
		}

		public static string GetInfusionLabel(this InfusionSuffix infSuffix)
		{
			return infSuffix.ToString().ToLower();
		}

		public static string GetInfusionDescription(this InfusionSuffix infSuffix)
		{
			switch (infSuffix)
			{
				case InfusionSuffix.Shock:
					return "Small bonus to damage and cooldown.";
				case InfusionSuffix.Impact:
					return "Bonus to damage.";
				case InfusionSuffix.Needle:
					return "Bonus to hit chance.\nSmall penalty to damage.";
				case InfusionSuffix.Charisma:
					return "Small bonus to social skills.";

				case InfusionSuffix.Plain:
					return "Bonus to sowing and harvesting. Small bonus to cooldown.";
				case InfusionSuffix.Rock:
					return
						"Small bonus to mental break threshold and psychic sensitivity.\nPenalty to cooldown and social skills. Small penalty to global work speed.";
				case InfusionSuffix.Creation:
					return "Small bonus to global work speed.";
				case InfusionSuffix.Stream:
					return "Big bonus to cooldown.\nPenalty to hit chance.";

				case InfusionSuffix.Sunlight:
					return "Bonus to attack and hit chance.";
				case InfusionSuffix.Starlight:
					return "Bonus to cooldown. Small bonus to hit chance.";
				case InfusionSuffix.Pain:
					return
						"Huge bonus to damage.\nPenalty to psychic sensitivity. Small penalty to mental break threshold and immunity gain speed.";

				default:
					throw new ArgumentOutOfRangeException(infSuffix.ToString());
			}
		}
	}

	public static class MathInfusion
	{
		public static int Rand(InfusionSuffix begin, InfusionSuffix end)
		{
			return Verse.Rand.Range((int)begin + 1, (int)end);
		}

		public static float ToAbs(this float f)
		{
			return Mathf.Abs(f);
		}
	}
}
