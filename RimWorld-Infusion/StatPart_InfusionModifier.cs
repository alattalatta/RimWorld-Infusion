using System;
using System.Text;
using RimWorld;
using Verse;
// ReSharper disable FieldCanBeMadeReadOnly.Local


namespace Infusion
{
	public class StatPart_InfusionModifier : StatPart
	{
		private StatMod shock = new StatMod();
		private StatMod impact = new StatMod();
		private StatMod needle = new StatMod();
		private StatMod charisma = new StatMod();

		private StatMod plain = new StatMod();
		private StatMod rock = new StatMod();
		private StatMod creation = new StatMod();
		private StatMod stream = new StatMod();
		private StatMod salt = new StatMod();

		private StatMod sunlight = new StatMod();
		private StatMod starlight = new StatMod();
		private StatMod pain = new StatMod();
		private StatMod automaton = new StatMod();
		private StatMod disassembling = new StatMod();

		public override void TransformValue(StatRequest req, ref float val)
		{
			InfusionSuffix infSuffix;
			if (!req.HasThing || !req.Thing.TryGetInfusion(out infSuffix))
				return;

			val += StatModOf(infSuffix).offset;
			val *= StatModOf(infSuffix).multiplier;
		}
		public override string ExplanationPart(StatRequest req)
		{
			InfusionSuffix infSuffix;
			if (!req.HasThing || !req.Thing.TryGetInfusion(out infSuffix))
				return null;

			var result = new StringBuilder();
			result.AppendLine("Infusion bonuses");
			if (StatModOf(infSuffix).offset != 0)
				result.AppendLine("    " + infSuffix.GetInfusionLabel().CapitalizeFirst() + ": +" + StatModOf(infSuffix).offset.ToAbs().ToStringPercent());
			if (StatModOf(infSuffix).multiplier != 1)
				result.AppendLine("    " + infSuffix.GetInfusionLabel().CapitalizeFirst() + ": x" + StatModOf(infSuffix).multiplier.ToStringPercent());
			return result.ToString();
		}
		//Decide what modifier StatPart should use
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

				case InfusionSuffix.Plain:
					return plain;
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
				case InfusionSuffix.Disassembling:
					return disassembling;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
