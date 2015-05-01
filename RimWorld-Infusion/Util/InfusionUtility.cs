using System;
using System.Collections.Generic;
using System.Text;
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

		public static bool TryGetInfusionPrefix(this Thing thing, out InfusionPrefix infPrefix)
		{
			var compInfusion = thing.TryGetComp<CompInfusion>();
			if (compInfusion == null)
			{
				infPrefix = InfusionPrefix.None;
				return false;
			}

			infPrefix = compInfusion.Infusion.First;
			return infPrefix != InfusionPrefix.None;
		}
		public static bool TryGetInfusionSuffix(this Thing thing, out InfusionSuffix infSuffix)
		{
			var compInfusion = thing.TryGetComp<CompInfusion>();
			if (compInfusion == null)
			{
				infSuffix = InfusionSuffix.None;
				return false;
			}

			infSuffix = compInfusion.Infusion.Second;
			return infSuffix != InfusionSuffix.None;
		}

		public static string GetInfusedLabel(this Thing thing)
		{
			var result = new StringBuilder();

			InfusionPrefix infPrefix;
			if (TryGetInfusionPrefix(thing, out infPrefix))
			{
				result.Append(GetInfusionLabel(infPrefix) + " ");
			}
			result.Append(thing.Stuff.LabelAsStuff + " " + thing.def.label);

			InfusionSuffix infSuffix;
			if (TryGetInfusionSuffix(thing, out infSuffix))
				result.Append(" of " + GetInfusionLabel(infSuffix));

			return result.ToString().CapitalizeFirst();
		}

		public static string GetInfusedLabelShort(this Thing thing)
		{
			var result = new StringBuilder();

			InfusionPrefix infPrefix;
			if (TryGetInfusionPrefix(thing, out infPrefix))
				result.Append(GetInfusionLabel(infPrefix) + " ");
			result.Append(thing.def.label);

			InfusionSuffix infSuffix;
			if (TryGetInfusionSuffix(thing, out infSuffix))
				result.Append(" of " + GetInfusionLabel(infSuffix));

			return result.ToString().CapitalizeFirst();
		}
		public static string GetInfusionLabelShort(this InfusionPrefix infPrefix)
		{
			switch (infPrefix)
			{
				case InfusionPrefix.Lightweight:
					return "light";
				case InfusionPrefix.Heavyweight:
					return "heavy";

				case InfusionPrefix.Compressed:
					return "comp";
				case InfusionPrefix.Targeting:
					return "targ";
				case InfusionPrefix.Intimidating:
					return "inti";
				case InfusionPrefix.Decorated:
					return "deco";
				case InfusionPrefix.Slaughterous:
					return "slght";
				case InfusionPrefix.Alcoholic:
					return "alco";

				case InfusionPrefix.Telescoping:
					return "tele";
				case InfusionPrefix.Mechanized:
					return "mecha";
				case InfusionPrefix.Pneumatic:
					return "pneu";
				case InfusionPrefix.Antiviral:
					return "antiv";
				case InfusionPrefix.Holographic:
					return "holo";
				case InfusionPrefix.Contaminated:
					return "cont";

				default:
					return GetInfusionLabel(infPrefix);
			}
		}
		public static string GetInfusionLabel(this InfusionPrefix infPrefix)
		{
			return infPrefix.ToString().ToLower();
		}
		public static string GetInfusionLabelShort(this InfusionSuffix infSuffix)
		{
			switch (infSuffix)
			{
				case InfusionSuffix.Charisma:
					return "charm";
				case InfusionSuffix.Creation:
					return "creat";
				case InfusionSuffix.Automaton:
					return "auto";
				case InfusionSuffix.Disassembler:
					return "dassem";
				default:
					return GetInfusionLabel(infSuffix);
			}
		}
		public static string GetInfusionLabel(this InfusionSuffix infSuffix)
		{
			return infSuffix.ToString().ToLower();
		}

		public static string GetInfusionDescription(this InfusionPrefix infPrefix)
		{
			switch (infPrefix)
			{
				case InfusionPrefix.Lightweight:
					return "small bonus to cooldown but small penalty to damage.";
				case InfusionPrefix.Heavyweight:
					return "small bonus to damage but small penalty to cooldown.";

				case InfusionPrefix.Hot:
					return "bonus to minimum comfortable temperature but small penalty to maximum comfortable temperature.";
				case InfusionPrefix.Cold:
					return "bonus to maximum comfortable temperature but small penalty to minimum comfortable temperature.";
				case InfusionPrefix.Compressed:
					return "bonus to cooldown.";
				case InfusionPrefix.Targeting:
					return "small bonus to hit chance.";
				case InfusionPrefix.Intimidating:
					return "small bonus to damage but small penalty for social skills.";
				case InfusionPrefix.Decorated:
					return "small bonus to social skills.";
				case InfusionPrefix.Slaughterous:
					return "small bonus to butchery skills.";
				case InfusionPrefix.Alcoholic:
					return "bonus to brewing but small penalty to hit chance.";

				case InfusionPrefix.Telescoping:
					return "bonus to hit chance and cooldown.";
				case InfusionPrefix.Mechanized:
					return "small bonus to damage, hit chance and construction speed.";
				case InfusionPrefix.Pneumatic:
					return "small bonus to damage, hit chance, mining speed and stonecutting speed.";
				case InfusionPrefix.Charged:
					return "bonus to Attack.";
				case InfusionPrefix.Antiviral:
					return "bonus to immunity gain speed.";
				case InfusionPrefix.Holographic:
					return "big bonus to social but penalty to attack and small penalty to hit chance.";
				case InfusionPrefix.Contaminated:
					return "penalty to immunity gain speed.";

				default:
					throw new ArgumentOutOfRangeException(infPrefix.ToString());
			}
		}

		public static string GetInfusionDescription(this InfusionSuffix infSuffix)
		{
			switch (infSuffix)
			{
				case InfusionSuffix.Shock:
					return "small bonus to damage and cooldown.";
				case InfusionSuffix.Impact:
					return "bonus to damage.";
				case InfusionSuffix.Needle:
					return "bonus to hit chance but small penalty to damage.";
				case InfusionSuffix.Charisma:
					return "small bonus to social skills.";

				case InfusionSuffix.Forest:
					return "bonus to sowing and harvesting and small bonus to cooldown.";
				case InfusionSuffix.Rock:
					return
						"small bonus to mental break threshold and psychic sensitivity but penalty to cooldown and social skills. Also small penalty to global work speed.";
				case InfusionSuffix.Creation:
					return "small bonus to global work speed.";
				case InfusionSuffix.Stream:
					return "big bonus to cooldown but small penalty to damage and hit chance.";
				case InfusionSuffix.Salt:
					return "small bonus to cooking speed, food poison chance and minimum/maximum temperature.";

				case InfusionSuffix.Sunlight:
					return "bonus to damage, hit chance. Also small bonus to immunity gain speed.";
				case InfusionSuffix.Starlight:
					return "bonus to cooldown and small bonus to hit chance.";
				case InfusionSuffix.Pain:
					return
						"huge bonus to damage but penalty to psychic sensitivity. Also small penalty to mental break threshold and immunity gain speed.";
				case InfusionSuffix.Automaton:
					return "bonus to global work speed.";
				case InfusionSuffix.Disassembler:
					return "bonus to mechanoid disassembling speed and small bonus to efficiency.";

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
		public static int Rand(InfusionPrefix begin, InfusionPrefix end)
		{
			return Verse.Rand.Range((int)begin + 1, (int)end);
		}

		public static float ToAbs(this float f)
		{
			return Mathf.Abs(f);
		}
	}
}
