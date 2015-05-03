using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using RimWorld;
using Verse;
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace Infusion
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
	public class StatPart_InfusionModifier : StatPart
	{
		#region StatMod variables
		/* Prefixes */
		protected StatMod lightweight = new StatMod();
		protected StatMod heavyweight = new StatMod();
		protected StatMod hardened = new StatMod();

		protected StatMod hot = new StatMod();
		protected StatMod cold = new StatMod();
		protected StatMod compressed = new StatMod();
		protected StatMod targeting = new StatMod();
		protected StatMod intimidating = new StatMod();
		protected StatMod decorated = new StatMod();
		protected StatMod slaughterous = new StatMod();
		protected StatMod alcoholic = new StatMod();

		protected StatMod telescoping = new StatMod();
		protected StatMod mechanized = new StatMod();
		protected StatMod pneumatic = new StatMod();
		protected StatMod charged = new StatMod();
		protected StatMod antiviral = new StatMod();
		protected StatMod holographic = new StatMod();
		protected StatMod gravitational = new StatMod();
		protected StatMod contaminated = new StatMod();

		/* Suffixes */
		protected StatMod shock = new StatMod();
		protected StatMod impact = new StatMod();
		protected StatMod needle = new StatMod();
		protected StatMod charisma = new StatMod();

		protected StatMod forest = new StatMod();
		protected StatMod rock = new StatMod();
		protected StatMod creation = new StatMod();
		protected StatMod stream = new StatMod();
		protected StatMod salt = new StatMod();

		protected StatMod sunlight = new StatMod();
		protected StatMod starlight = new StatMod();
		protected StatMod pain = new StatMod();
		protected StatMod automaton = new StatMod();
		protected StatMod dismantle = new StatMod();
		protected StatMod exhaust = new StatMod();
		#endregion //StatMods

		public override void TransformValue(StatRequest req, ref float val)
		{
			if (!req.HasThing)
				return;

			InfusionPrefix infPrefix;
			if (req.Thing.TryGetInfusionPrefix(out infPrefix))
			{
				val += StatModOf(infPrefix).offset;
				if(StatModOf(infPrefix).multiplier != 0)
					val *= StatModOf(infPrefix).multiplier;
			}

			InfusionSuffix infSuffix;
			if (!req.Thing.TryGetInfusionSuffix(out infSuffix)) return;

			val += StatModOf(infSuffix).offset;
			if (StatModOf(infSuffix).multiplier != 0)
				val *= StatModOf(infSuffix).multiplier;
		}
		public override string ExplanationPart(StatRequest req)
		{
			if (!req.HasThing)
				return null;

			InfusionPrefix infPrefix;
			req.Thing.TryGetInfusionPrefix(out infPrefix);
			InfusionSuffix infSuffix;
			req.Thing.TryGetInfusionSuffix(out infSuffix);
			if (infPrefix == InfusionPrefix.None && infSuffix == InfusionSuffix.None)
				return null;

			return WriteExplanation(req, infPrefix, infSuffix);
		}

		protected virtual string WriteExplanation(StatRequest req, InfusionPrefix infPrefix, InfusionSuffix infSuffix)
		{
			var result = new StringBuilder();
			result.AppendLine(StaticSet.StringInfusionDescBonus);

			if (infPrefix != InfusionPrefix.None)
			{
				if (StatModOf(infPrefix).offset != 0)
				{
					result.Append("    " + infPrefix.GetInfusionLabel().CapitalizeFirst() + ": ");
					result.AppendLine((StatModOf(infPrefix).offset > 0 ? "+" : "-") +
									  StatModOf(infPrefix).offset.ToAbs().ToStringPercent());
				}
				if (StatModOf(infPrefix).multiplier != 1)
				{
					result.AppendLine("    " + infPrefix.GetInfusionLabel().CapitalizeFirst() + ": x" +
									  StatModOf(infPrefix).multiplier.ToStringPercent());
				}
			}
			if (infSuffix != InfusionSuffix.None)
			{
				if (StatModOf(infSuffix).offset != 0)
				{
					result.Append("    " + infSuffix.GetInfusionLabel().CapitalizeFirst() + ": ");
					result.AppendLine((StatModOf(infSuffix).offset > 0 ? "+" : "-") +
									  StatModOf(infSuffix).offset.ToAbs().ToStringPercent());
				}
				if (StatModOf(infSuffix).multiplier != 1)
				{
					result.AppendLine("    " + infSuffix.GetInfusionLabel().CapitalizeFirst() + ": x" +
									  StatModOf(infSuffix).multiplier.ToStringPercent());
				}
			}
			return result.ToString();
		}

		#region StatModOf switches
		//Decide what modifier StatPart should use
		protected StatMod StatModOf(InfusionPrefix infPrefix)
		{
			switch (infPrefix)
			{
				case InfusionPrefix.Lightweight:
					return lightweight;
				case InfusionPrefix.Heavyweight:
					return heavyweight;
				case InfusionPrefix.Hardened:
					return hardened;

				case InfusionPrefix.Hot:
					return hot;
				case InfusionPrefix.Cold:
					return cold;
				case InfusionPrefix.Compressed:
					return compressed;
				case InfusionPrefix.Targeting:
					return targeting;
				case InfusionPrefix.Intimidating:
					return intimidating;
				case InfusionPrefix.Decorated:
					return decorated;
				case InfusionPrefix.Slaughterous:
					return slaughterous;
				case InfusionPrefix.Alcoholic:
					return alcoholic;

				case InfusionPrefix.Telescoping:
					return telescoping;
				case InfusionPrefix.Mechanized:
					return mechanized;
				case InfusionPrefix.Pneumatic:
					return pneumatic;
				case InfusionPrefix.Charged:
					return charged;
				case InfusionPrefix.Antiviral:
					return antiviral;
				case InfusionPrefix.Holographic:
					return holographic;
				case InfusionPrefix.Gravitational:
					return gravitational;
				case InfusionPrefix.Contaminated:
					return contaminated;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		protected StatMod StatModOf(InfusionSuffix infSuffix)
		{
			switch (infSuffix)
			{
				case InfusionSuffix.Shock:
					return shock;
				case InfusionSuffix.Impact:
					return impact;
				case InfusionSuffix.Needle:
					return needle;
				case InfusionSuffix.Charisma:
					return charisma;

				case InfusionSuffix.Forest:
					return forest;
				case InfusionSuffix.Rock:
					return rock;
				case InfusionSuffix.Creation:
					return creation;
				case InfusionSuffix.Stream:
					return stream;
				case InfusionSuffix.Salt:
					return salt;

				case InfusionSuffix.Sunlight:
					return sunlight;
				case InfusionSuffix.Starlight:
					return starlight;
				case InfusionSuffix.Pain:
					return pain;
				case InfusionSuffix.Automaton:
					return automaton;
				case InfusionSuffix.Dismantle:
					return dismantle;
				case InfusionSuffix.Exhaust:
					return exhaust;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		#endregion
	}
}
