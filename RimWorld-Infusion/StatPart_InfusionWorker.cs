using System;
using System.Text;
using RimWorld;
using Verse;

namespace Infusion
{
	public class StatPart_InfusionWorker : StatPart
	{
		protected StatMod shock = new StatMod { multiplier = 1, offset = 0 };
		protected StatMod impact = new StatMod { multiplier = 1, offset = 0 };
		protected StatMod needle = new StatMod { multiplier = 1, offset = 0 };
		protected StatMod charisma = new StatMod { multiplier = 1, offset = 0 };

		protected StatMod fire = new StatMod { multiplier = 1, offset = 0 };
		protected StatMod water = new StatMod { multiplier = 1, offset = 0 };
		protected StatMod plain = new StatMod { multiplier = 1, offset = 0 };
		protected StatMod rock = new StatMod { multiplier = 1, offset = 0 };
		protected StatMod creation = new StatMod { multiplier = 1, offset = 0 };
		protected StatMod stream = new StatMod { multiplier = 1, offset = 0 };

		protected StatMod sunlight = new StatMod { multiplier = 1, offset = 0 };
		protected StatMod starlight = new StatMod { multiplier = 1, offset = 0 };
		protected StatMod pain = new StatMod { multiplier = 1, offset = 0 };

		public override void TransformValue(StatRequest req, ref float val)
		{
			if (!req.HasThing || req.Thing == null) return;
			if (req.Def != ThingDef.Named("Human"))
				return;

			//Return if the pawn has no weapon
			var pawn = req.Thing as Pawn;
			if (pawn == null || pawn.equipment.Primary == null) return;

			InfusionTypes infType;
			if (!pawn.equipment.Primary.TryGetInfusion(out infType)) return;
			if (infType == InfusionTypes.None) return;
			
			val += StatModOf(infType).offset;
			val *= StatModOf(infType).multiplier;
		}
		public override string ExplanationPart(StatRequest req)
		{
			if (req.Def != ThingDef.Named("Human"))
				return null;
			InfusionTypes infType;
			var pawn = req.Thing as Pawn;
			if (pawn == null)
				return null;
			if (!req.HasThing || !pawn.equipment.Primary.TryGetInfusion(out infType))
				return null;

			var result = new StringBuilder();
			result.AppendLine("Infusion bonuses");
			if (StatModOf(infType).offset != 0)
				result.AppendLine("    " + pawn.equipment.Primary.GetInfusedLabelShort() + ": " + (StatModOf(infType).offset > 0 ? "+" : "-") + StatModOf(infType).offset.ToAbs().ToStringPercent());
			if (StatModOf(infType).multiplier != 1)
				result.AppendLine("    " + pawn.equipment.Primary.GetInfusedLabelShort() + ": x" + StatModOf(infType).multiplier.ToStringPercent());
			return result.ToString();
		}

		//Decide what modifier StatPart should use
		protected StatMod StatModOf(InfusionTypes infTypes)
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
