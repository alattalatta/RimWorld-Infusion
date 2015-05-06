using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using RimWorld;
using Verse;

namespace Infusion
{
	[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
	public static class GenInfusionText
	{
		private static Dictionary<int, string> infusedLabelDict = new Dictionary<int, string>();
		private static Dictionary<int, string> infusedITabDict = new Dictionary<int, string>();
		private const int LabelDictionaryMaxCount = 1000;
		private const int ITabDictionaryMaxCount = 1000;

		private struct InfusedLabelRequest
		{
			public Thing Thing;
			public EntityDef EntDef;
			public ThingDef StuffDef;
			public int HitPoints;
			public int MaxHitPoints;

			public override int GetHashCode()
			{
				var num1 = 7437233;
				if (Thing != null)
					num1 ^= Thing.GetHashCode()*712433;
				var num2 = num1 ^ EntDef.GetHashCode() * 345111;
				if (StuffDef != null)
					num2 ^= StuffDef.GetHashCode() * 666613;
				var thingDef = EntDef as ThingDef;
				if (thingDef == null) return num2;

				InfusionSet inf;
				if (Thing != null && Thing.TryGetInfusions(out inf))
				{
					if(!inf.PassPre)
						num2 ^= inf.Prefix.GetHashCode();
					if(!inf.PassSuf)
						num2 ^= inf.Suffix.GetHashCode();
				}

				if (thingDef.useHitPoints)
					num2 = num2 ^ HitPoints * 743273 ^ MaxHitPoints * 7437;
				return num2;
			}
		}

		private struct InfusedITabRequest
		{
			public string Prefix;
			public string Suffix;

			public override int GetHashCode()
			{
				var num1 = 17;
				if (Prefix != null)
					num1 = num1*29 + Prefix.GetHashCode();
				if (Suffix != null)
					num1 = num1*29 + Suffix.GetHashCode();
				return num1;
			}
		}

		public static string GetInfusedLabel(this Thing thing, bool isStuffed = true, bool isDetailed = true)
		{
			var request = new InfusedLabelRequest
			{
				EntDef = thing.def,
				Thing = thing
			};

			if (isStuffed)
				request.StuffDef = thing.Stuff;
			if (isDetailed)
			{
				request.MaxHitPoints = thing.MaxHitPoints;
				request.HitPoints = thing.HitPoints;
			}

			var hashCode = request.GetHashCode();
			string result;
			if (infusedLabelDict.TryGetValue(hashCode, out result)) return result;

			if (infusedLabelDict.Count > LabelDictionaryMaxCount)
				infusedLabelDict.Clear();
			result = NewInfusedThingLabel(thing, isStuffed, isDetailed);
			infusedLabelDict.Add(hashCode, result);
			return result;
		}

		private static string NewInfusedThingLabel(Thing thing, bool isStuffed, bool isDetailed)
		{
			var result = new StringBuilder();

			InfusionSet inf;
			thing.TryGetInfusions(out inf);

			if (!inf.PassPre)
				result.Append(inf.Prefix.ToInfusionDef().label + " ");

			string thingLabel;
			if (isStuffed && thing.Stuff != null)
				thingLabel = thing.Stuff.LabelAsStuff + " " + thing.def.label;
			else
				thingLabel = thing.def.label;

			result.Append(!inf.PassSuf
				? StaticSet.StringInfusionOf.Translate(thingLabel, inf.Suffix.ToInfusionDef().label)
				: thingLabel);

			if (!isDetailed)
				return result.ToString();

			result.Append(" (");
			QualityCategory qc;
			if (thing.TryGetQuality(out qc))
			{
				result.Append(qc.GetLabelShort());
			}

			if (!(thing.HitPoints < thing.MaxHitPoints)) return result + ")";

			result.Append(" " + ((float) thing.HitPoints/thing.MaxHitPoints).ToStringPercent() + ")");
			return result.ToString();
		}

		public static string GetInfusedDescriptionITab(this Thing thing)
		{
			InfusionSet infs;
			thing.TryGetInfusions(out infs);
			var request = new InfusedITabRequest
			{
				Prefix = infs.Prefix,
				Suffix = infs.Suffix
			};

			var hashCode = request.GetHashCode();
			string result;
			if (infusedITabDict.TryGetValue(hashCode, out result)) return result;

			if (infusedITabDict.Count > ITabDictionaryMaxCount)
				infusedITabDict.Clear();
			result = thing.NewInfusedDescriptionITab();
			infusedITabDict.Add(hashCode, result);
			return result;
		}

		private static string NewInfusedDescriptionITab(this Thing thing)
		{
			InfusionSet inf;
			if (!thing.TryGetInfusions(out inf))
				return null;

			var result = new StringBuilder(null);
			if (!inf.PassPre)
			{
				var prefix = inf.Prefix.ToInfusionDef();
				result.AppendLine("From " + prefix.LabelCap + ":");
				foreach (KeyValuePair<StatDef, StatMod> current in prefix.stats)
				{
					if (current.Value.offset != 0)
					{
						result.Append("     " + (current.Value.offset > 0 ? "+" : "-"));
						if (current.Key == StatDefOf.ComfyTemperatureMax || current.Key == StatDefOf.ComfyTemperatureMin)
							result.Append(current.Value.offset.ToAbs().ToStringTemperatureOffset());
						else if (current.Key == StatDefOf.MoveSpeed)
							result.Append(current.Value.offset.ToAbs() + "c/s");
						else if (current.Key == StatDefOf.MaxHitPoints)
							result.Append(current.Value.offset.ToAbs());
						else
							result.Append(current.Value.offset.ToAbs().ToStringPercent());
						result.AppendLine(" " + current.Key.LabelCap);
					}
					if (current.Value.multiplier == 1) continue;

					result.Append("     " + current.Value.multiplier.ToAbs().ToStringPercent());
					result.AppendLine(" " + current.Key.LabelCap);
				}
				result.AppendLine();
			}
			if (inf.PassSuf) return result.ToString();

			var suffix = inf.Suffix.ToInfusionDef();
			result.AppendLine("From " + suffix.LabelCap + ":");
			foreach (KeyValuePair<StatDef, StatMod> current in suffix.stats)
			{
				if (current.Value.offset != 0)
				{
					result.Append("     " + (current.Value.offset > 0 ? "+" : "-"));
					if (current.Key == StatDefOf.ComfyTemperatureMax || current.Key == StatDefOf.ComfyTemperatureMin)
						result.Append(current.Value.offset.ToAbs().ToStringTemperatureOffset());
					else if (current.Key == StatDefOf.MoveSpeed)
						result.Append(current.Value.offset.ToAbs() + "c/s");
					else if (current.Key == StatDefOf.MaxHitPoints)
						result.Append(current.Value.offset.ToAbs());
					else
						result.Append(current.Value.offset.ToAbs().ToStringPercent());
					result.AppendLine(" " + current.Key.LabelCap);
				}
				if (current.Value.multiplier == 1) continue;

				result.Append("     " + current.Value.multiplier.ToAbs().ToStringPercent());
				result.AppendLine(" " + current.Key.LabelCap);
			}
			return result.ToString();
		}

		public static string GetInfusedDescription(this Thing thing)
		{
			InfusionSet inf;
			if (!thing.TryGetInfusions(out inf))
				return null;

			var result = new StringBuilder(null);
			if (!inf.PassPre)
			{
				result.Append("This weapon " + inf.Prefix.ToInfusionDef().description + ".");
				if (!inf.PassSuf)
					result.Append(" ");
			}
			if (!inf.PassSuf)
				result.AppendLine(inf.Suffix.ToInfusionDef().description.CapitalizeFirst() + ".");
			else
				result.AppendLine();

			return result.ToString();
		}
	}
}
