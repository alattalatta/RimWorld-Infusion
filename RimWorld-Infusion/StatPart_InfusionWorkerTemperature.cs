using System;
using System.Text;
using RimWorld;
using Verse;

namespace Infusion
{
	public class StatPart_InfusionWorkerTemperature : StatPart_InfusionWorker
	{
		
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
				result.AppendLine("    " + pawn.equipment.Primary.GetInfusedLabelShort() + ": " + (StatModOf(infSuffix).offset > 0 ? "+" : "-") + StatModOf(infSuffix).offset.ToAbs().ToStringTemperatureOffset());
			if (StatModOf(infSuffix).multiplier != 1)
				result.AppendLine("    " + pawn.equipment.Primary.GetInfusedLabelShort() + ": x" + StatModOf(infSuffix).multiplier.ToStringPercent());
			return result.ToString();
		}
	}
}
