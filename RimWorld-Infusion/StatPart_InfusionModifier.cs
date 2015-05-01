using System;
using System.Text;
using RimWorld;
using Verse;


namespace Infusion
{
	public class StatPart_InfusionModifier : StatPart
	{
		private StatMod shock = new StatMod { multiplier = 1, offset = 0 };
		private StatMod impact = new StatMod { multiplier = 1, offset = 0 };
		private StatMod needle = new StatMod { multiplier = 1, offset = 0 };
		private StatMod charisma = new StatMod { multiplier = 1, offset = 0 };

		private StatMod fire = new StatMod { multiplier = 1, offset = 0 };
		private StatMod water = new StatMod { multiplier = 1, offset = 0 };
		private StatMod plain = new StatMod { multiplier = 1, offset = 0 };
		private StatMod rock = new StatMod { multiplier = 1, offset = 0 };
		private StatMod creation = new StatMod { multiplier = 1, offset = 0 };
		private StatMod stream = new StatMod { multiplier = 1, offset = 0 };

		private StatMod sunlight = new StatMod { multiplier = 1, offset = 0 };
		private StatMod starlight = new StatMod { multiplier = 1, offset = 0 };
		private StatMod pain = new StatMod { multiplier = 1, offset = 0 };

		public override void TransformValue(StatRequest req, ref float val)
		{
			InfusionTypes infType;
			if (!req.HasThing || !req.Thing.TryGetInfusion(out infType))
				return;

			val += StatModOf(infType).offset;
			val *= StatModOf(infType).multiplier;
		}
		public override string ExplanationPart(StatRequest req)
		{
			InfusionTypes infType;
			if (!req.HasThing || !req.Thing.TryGetInfusion(out infType))
				return null;

			var result = new StringBuilder();
			result.AppendLine("Infusion bonuses");
			if (StatModOf(infType).offset != 0)
				result.AppendLine("    " + infType.GetInfusionLabel().CapitalizeFirst() + ": +" + StatModOf(infType).offset.ToAbs().ToStringPercent());
			if (StatModOf(infType).multiplier != 1)
				result.AppendLine("    " + infType.GetInfusionLabel().CapitalizeFirst() + ": x" + StatModOf(infType).multiplier.ToStringPercent());
			return result.ToString();
		}
		//Decide what modifier StatPart should use
		private StatMod StatModOf(InfusionTypes infTypes)
		{
			switch (infTypes)
			{
				case InfusionTypes.Shock:
					return shock;
				case InfusionTypes.Impact:
					return impact;
				case InfusionTypes.Needle:
					return needle;
				case InfusionTypes.Charisma:
					return charisma;

				case InfusionTypes.Fire:
					return fire;
				case InfusionTypes.Water:
					return water;
				case InfusionTypes.Plain:
					return plain;
				case InfusionTypes.Rock:
					return rock;
				case InfusionTypes.Creation:
					return creation;
				case InfusionTypes.Stream:
					return stream;

				case InfusionTypes.Sunlight:
					return sunlight;
				case InfusionTypes.Starlight:
					return starlight;
				case InfusionTypes.Pain:
					return pain;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
