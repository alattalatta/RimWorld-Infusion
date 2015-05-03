using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using RimWorld;
using Verse;

namespace Infusion
{
	[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
	public static class GenInfusionLabel
	{
		private static Dictionary<int, string> labelDictionary = new Dictionary<int, string>();
		private const int LabelDictionaryMaxCount = 2000;

		private struct LabelRequest
		{
			public Thing Thing;
			public EntityDef EntDef;
			public ThingDef StuffDef;
			public QualityCategory Quality;
			public InfusionPrefix Prefix;
			public InfusionSuffix Suffix;
			public int HitPoints;
			public int MaxHitPoints;

			public override int GetHashCode()
			{
				var num1 = 7437233;
				if (Thing != null)
					num1 ^= Thing.GetHashCode() * 712431;
				var num2 = num1 ^ EntDef.GetHashCode() * 345111;
				if (StuffDef != null)
					num2 ^= StuffDef.GetHashCode() * 666611;
				var thingDef = EntDef as ThingDef;
				if (thingDef == null) return num2;

				QualityCategory qc;
				if (Thing != null && Thing.TryGetQuality(out qc))
					num2 ^= (int)Quality * 391;
				InfusionSuffix infSuffix;
				if (Thing != null && Thing.TryGetInfusionSuffix(out infSuffix))
					num2 ^= (int)Prefix * 677;
				InfusionPrefix infPrefix;
				if (Thing != null && Thing.TryGetInfusionPrefix(out infPrefix))
					num2 ^= (int)Suffix * 1183;

				if (thingDef.useHitPoints)
					num2 = num2 ^ HitPoints * 743273 ^ MaxHitPoints * 7437;
				return num2;
			}
		}

		public static string GetInfusedLabel(this Thing thing, bool isStuffed = true, bool isDetailed = true)
		{
			var request = new LabelRequest()
			{
				EntDef = thing.def,
				Thing = thing
			};
			thing.TryGetQuality(out request.Quality);
			thing.TryGetInfusionPrefix(out request.Prefix);
			thing.TryGetInfusionSuffix(out request.Suffix);

			if (isStuffed)
				request.StuffDef = thing.Stuff;
			if (isDetailed)
			{
				request.MaxHitPoints = thing.MaxHitPoints;
				request.HitPoints = thing.HitPoints;
			}

			var hashCode = request.GetHashCode();
			string result;
			if (labelDictionary.TryGetValue(hashCode, out result)) return result;

			if (labelDictionary.Count > LabelDictionaryMaxCount)
				labelDictionary.Clear();
			result = NewInfusedThingLabel(thing, isStuffed, isDetailed);
			labelDictionary.Add(hashCode, result);
			return result;
		}

		private static string NewInfusedThingLabel(Thing thing, bool isStuffed, bool isDetailed)
		{
			string result = null;

			InfusionPrefix infPrefix;
			if (thing.TryGetInfusionPrefix(out infPrefix))
				result += infPrefix.GetInfusionLabel() + " ";

			string label;
			if (isStuffed && thing.Stuff != null)
				label = thing.Stuff.LabelAsStuff + " " + thing.def.label;
			else
				label = thing.def.label;

			InfusionSuffix infSuffix;
			result += thing.TryGetInfusionSuffix(out infSuffix)
				? StaticSet.StringInfusionInfusedLabelSuffix.Translate(label, infSuffix.GetInfusionLabel())
				: thing.def.label;

			if (!isDetailed)
				return result;

			result += " (";
			QualityCategory qc;
			if (thing.TryGetQuality(out qc))
				result += qc.GetLabelShort();

			if (!(thing.HitPoints < thing.MaxHitPoints)) return result + ")";

			result += " " + ((float)thing.HitPoints / thing.MaxHitPoints).ToStringPercent() + ")";
			return result;
		}
	}
}
