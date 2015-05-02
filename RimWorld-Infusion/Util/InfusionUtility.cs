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
		/// <summary>
		/// Set prefix enum parameter's value to the thing's prefix infusion if exists.
		/// </summary>
		/// <param name="thing">The thing to get infusion.</param>
		/// <param name="infPrefix">The value to set infusion.</param>
		/// <returns>True if exists, false if not.</returns>
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
		/// <summary>
		/// Set suffix enum parameter's value to the thing's suffix infusion if exists.
		/// </summary>
		/// <param name="thing">The thing to get infusion.</param>
		/// <param name="infSuffix">The value to set infusion.</param>
		/// <returns>True if exists, false if not.</returns>
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
				result.Append(GetInfusionLabel(infPrefix) + " ");

			result.Append(thing.Stuff.LabelAsStuff + " ");

			InfusionSuffix infSuffix;
			result.Append(TryGetInfusionSuffix(thing, out infSuffix)
				? StaticSet.StringInfusionInfusedLabelSuffix.Translate(thing.def.label, GetInfusionLabel(infSuffix))
				: thing.def.label);

			return result.ToString().CapitalizeFirst();
		}

		public static string GetInfusedLabelShort(this Thing thing)
		{
			var result = new StringBuilder();

			InfusionPrefix infPrefix;
			if (TryGetInfusionPrefix(thing, out infPrefix))
				result.Append(GetInfusionLabel(infPrefix) + " ");

			InfusionSuffix infSuffix;
			result.Append(TryGetInfusionSuffix(thing, out infSuffix)
				? StaticSet.StringInfusionInfusedLabelSuffix.Translate(thing.def.label, GetInfusionLabel(infSuffix))
				: thing.def.label);

			return result.ToString().CapitalizeFirst();
		}
		public static string GetInfusionLabelShort(this InfusionPrefix infPrefix)
		{
			return StaticSet.StringInfusionPrefixLabelsShort[(int) infPrefix];
		}
		public static string GetInfusionLabelShort(this InfusionSuffix infSuffix)
		{
			return StaticSet.StringInfusionSuffixLabelsShort[(int) infSuffix];
		}
		public static string GetInfusionLabel(this InfusionPrefix infPrefix)
		{
			return StaticSet.StringInfusionPrefixLabels[(int) infPrefix];
		}
		public static string GetInfusionLabel(this InfusionSuffix infSuffix)
		{
			return StaticSet.StringInfusionSuffixLabels[(int) infSuffix];
		}

		public static string GetInfusionDescription(this InfusionPrefix infPrefix)
		{
			return StaticSet.StringInfusionPrefixDescriptions[(int) infPrefix];
		}

		public static string GetInfusionDescription(this InfusionSuffix infSuffix)
		{
			return StaticSet.StringInfusionSuffixDescriptions[(int) infSuffix];
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
