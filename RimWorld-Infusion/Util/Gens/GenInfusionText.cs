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

		/** Hash things. Taken from RimWorld base code. **/
		private struct InfusedLabelRequest
		{
			public Thing Thing;
			public BuildableDef BuildableDef;
			public ThingDef StuffDef;
			public int HitPoints;
			public int MaxHitPoints;

			public override int GetHashCode()
			{
				var num1 = 7437233;
				if (Thing != null)
					num1 ^= Thing.GetHashCode()*712433;
				var num2 = num1 ^ BuildableDef.GetHashCode() * 345111;
				if (StuffDef != null)
					num2 ^= StuffDef.GetHashCode() * 666613;
				var thingDef = BuildableDef as ThingDef;
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
		/** End of the hash things. **/

		//Get one of existing infused labels from dictionary.
		public static string GetInfusedLabel(this Thing _thing, bool _isStuffed = true, bool _isDetailed = true)
		{
			var request = new InfusedLabelRequest
			{
				BuildableDef = _thing.def,
				Thing = _thing
			};

			if (_isStuffed)
				request.StuffDef = _thing.Stuff;
			if (_isDetailed)
			{
				request.MaxHitPoints = _thing.MaxHitPoints;
				request.HitPoints = _thing.HitPoints;
			}

			var hashCode = request.GetHashCode();
			string result;
			if (infusedLabelDict.TryGetValue(hashCode, out result)) return result;

			//Make a new label if there is none that matches.
			if (infusedLabelDict.Count > LabelDictionaryMaxCount)
				infusedLabelDict.Clear();
			result = NewInfusedThingLabel(_thing, _isStuffed, _isDetailed);
			//Save it to the dictionary.
			infusedLabelDict.Add(hashCode, result);
			return result;
		}

		//Make a new infused label.
		private static string NewInfusedThingLabel(Thing _thing, bool _isStuffed, bool _isDetailed)
		{
			var result = new StringBuilder();

			InfusionSet inf;
			_thing.TryGetInfusions(out inf);

			if (!inf.PassPre)
				result.Append(inf.Prefix.ToInfusionDef().label + " ");

			string thingLabel;
			if (_isStuffed && _thing.Stuff != null)
				thingLabel = _thing.Stuff.LabelAsStuff + " " + _thing.def.label;
			else
				thingLabel = _thing.def.label;

			result.Append(!inf.PassSuf
				? StaticSet.StringInfusionOf.Translate(thingLabel, inf.Suffix.ToInfusionDef().label)
				: thingLabel);

			if (!_isDetailed)
				return result.ToString();

			result.Append(" (");
			QualityCategory qc;
			if (_thing.TryGetQuality(out qc))
			{
				result.Append(qc.GetLabelShort());
			}

			if (!(_thing.HitPoints < _thing.MaxHitPoints)) return result + ")";

			result.Append(" " + ((float) _thing.HitPoints/_thing.MaxHitPoints).ToStringPercent() + ")");
			return result.ToString();
		}

		//Get one of infusion stat information from dictionary.
		public static string GetInfusedDescriptionITab(this Thing _thing)
		{
			InfusionSet infs;
			_thing.TryGetInfusions(out infs);
			var request = new InfusedITabRequest
			{
				Prefix = infs.Prefix,
				Suffix = infs.Suffix
			};

			var hashCode = request.GetHashCode();
			string result;
			if (infusedITabDict.TryGetValue(hashCode, out result)) return result;

			//Make a new label if there is none that matches.
			if (infusedITabDict.Count > ITabDictionaryMaxCount)
				infusedITabDict.Clear();
			result = _thing.NewInfusedDescriptionITab();
			//Save it to the dictionary.
			infusedITabDict.Add(hashCode, result);
			return result;
		}

		//Make a new infusion stat information.
		private static string NewInfusedDescriptionITab(this Thing _thing)
		{
			InfusionSet inf;
			if (!_thing.TryGetInfusions(out inf))
				return null;

			var result = new StringBuilder(null);
			if (!inf.PassPre)
			{
				var prefix = inf.Prefix.ToInfusionDef();
				result.AppendLine(StaticSet.StringInfusionDescFrom.Translate(prefix.LabelCap));
				foreach (KeyValuePair<StatDef, StatMod> current in prefix.stats)
				{
					if (current.Value.offset != 0)
					{
						result.Append("     " + (current.Value.offset > 0 ? "+" : "-"));
						if (current.Key == StatDefOf.ComfyTemperatureMax || current.Key == StatDefOf.ComfyTemperatureMin)
							result.Append(current.Value.offset.ToAbs().ToStringTemperatureOffset());
						else
						{
							var modifier = current.Key.parts.Find(_s => _s is StatPart_InfusionModifier) as StatPart_InfusionModifier;
							if (modifier != null)
							{
								if (modifier.offsetUsePercentage)
									result.Append(current.Value.offset.ToAbs().ToStringPercent());
								else
									result.Append(current.Value.offset.ToAbs() + modifier.offsetSuffix);
							}
						}
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
			result.AppendLine(StaticSet.StringInfusionDescFrom.Translate(suffix.LabelCap));
			foreach (var current in suffix.stats)
			{
				if (current.Value.offset != 0)
				{
					result.Append("     " + (current.Value.offset > 0 ? "+" : "-"));
					if (current.Key == StatDefOf.ComfyTemperatureMax || current.Key == StatDefOf.ComfyTemperatureMin)
						result.Append(current.Value.offset.ToAbs().ToStringTemperatureOffset());
					else
					{
						var modifier = current.Key.parts.Find(_s => _s is StatPart_InfusionModifier) as StatPart_InfusionModifier;
						if (modifier != null)
						{
							if (modifier.offsetUsePercentage)
								result.Append(current.Value.offset.ToAbs().ToStringPercent());
							else
								result.Append(current.Value.offset.ToAbs() + modifier.offsetSuffix);
						}
					}
					result.AppendLine(" " + current.Key.LabelCap);
				}
				if (current.Value.multiplier == 1) continue;

				result.Append("     " + current.Value.multiplier.ToAbs().ToStringPercent());
				result.AppendLine(" " + current.Key.LabelCap);
			}
			return result.ToString();
		}

		//Get infusion's defined(XML) description.
		public static string GetInfusedDescription(this Thing thing)
		{
			InfusionSet inf;
			if (!thing.TryGetInfusions(out inf))
				return null;

			var result = new StringBuilder(null);
			if (!inf.PassPre)
			{
				var str = thing.def.IsApparel ? StaticSet.StringThisApparel : StaticSet.StringThisWeapon;
				result.Append(str + " " + inf.Prefix.ToInfusionDef().description + ".");
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
