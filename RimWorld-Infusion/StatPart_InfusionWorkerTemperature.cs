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
			InfusionTypes infType;
			var pawn = req.Thing as Pawn;
			if (pawn == null)
				return null;
			if (!req.HasThing || !pawn.equipment.Primary.TryGetInfusion(out infType))
				return null;

			var result = new StringBuilder();
			result.AppendLine("Infusion bonuses");
			if (StatModOf(infType).offset != 0)
				result.AppendLine("    " + pawn.equipment.Primary.GetInfusedLabelShort() + ": " + (StatModOf(infType).offset > 0 ? "+" : "-") + StatModOf(infType).offset.ToAbs().ToStringTemperatureOffset());
			if (StatModOf(infType).multiplier != 1)
				result.AppendLine("    " + pawn.equipment.Primary.GetInfusedLabelShort() + ": x" + StatModOf(infType).multiplier.ToStringPercent());
			return result.ToString();
		}
	}
}
