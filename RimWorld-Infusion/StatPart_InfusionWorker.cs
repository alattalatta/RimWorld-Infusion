using System;
using System.Text;
using RimWorld;
using Verse;

namespace Infusion
{
	public class StatPart_InfusionWorker : StatPart_InfusionModifier
	{
		public override void TransformValue(StatRequest req, ref float val)
		{
			if (!req.HasThing || req.Thing == null) return;
			if (req.Def != ThingDef.Named("Human"))
				return;

			//Return if the pawn has no weapon
			var pawn = req.Thing as Pawn;
			if (pawn == null || pawn.equipment.Primary == null) return;

			InfusionSuffix infSuffix;
			if (!pawn.equipment.Primary.TryGetInfusion(out infSuffix)) return;
			if (infSuffix == InfusionSuffix.None) return;
			
			val += StatModOf(infSuffix).offset;
			val *= StatModOf(infSuffix).multiplier;
		}
		public override string ExplanationPart(StatRequest req)
		{
			if (req.Def != ThingDef.Named("Human"))
				return null;
			InfusionSuffix infSuffix;
			var pawn = req.Thing as Pawn;
			if (pawn == null)
				return null;
			if (!req.HasThing || !pawn.equipment.Primary.TryGetInfusion(out infSuffix))
				return null;

			var result = new StringBuilder();
			result.AppendLine("Infusion bonuses");
			if (StatModOf(infSuffix).offset != 0)
				result.AppendLine("    " + pawn.equipment.Primary.GetInfusedLabelShort() + ": " + (StatModOf(infSuffix).offset > 0 ? "+" : "-") + StatModOf(infSuffix).offset.ToAbs().ToStringPercent());
			if (StatModOf(infSuffix).multiplier != 1)
				result.AppendLine("    " + pawn.equipment.Primary.GetInfusedLabelShort() + ": x" + StatModOf(infSuffix).multiplier.ToStringPercent());
			return result.ToString();
		}
	}
}
