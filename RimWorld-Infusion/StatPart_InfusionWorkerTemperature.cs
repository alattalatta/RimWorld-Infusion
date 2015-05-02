using System.Text;
using RimWorld;
using Verse;

namespace Infusion
{
	public class StatPart_InfusionWorkerTemperature : StatPart_InfusionWorker
	{
		protected override string WriteExplanation(StatRequest req, InfusionPrefix infPrefix, InfusionSuffix infSuffix)
		{
			var result = new StringBuilder();
			result.AppendLine(StaticSet.StringInfusionDescBonus);

			if (infPrefix != InfusionPrefix.None)
			{
				if (StatModOf(infPrefix).offset != 0)
				{
					result.Append("    " + req.Thing.GetInfusedLabelShort().CapitalizeFirst() + ": ");
					result.AppendLine((StatModOf(infPrefix).offset > 0 ? "+" : "-") +
									  StatModOf(infPrefix).offset.ToAbs().ToStringTemperatureOffset());
				}
				if (StatModOf(infPrefix).multiplier != 1)
				{
					result.AppendLine("    " + req.Thing.GetInfusedLabelShort().CapitalizeFirst() + ": x" +
									  StatModOf(infPrefix).multiplier.ToStringPercent());
				}
			}
			if (infSuffix == InfusionSuffix.None) return result.ToString();

			if (StatModOf(infSuffix).offset != 0)
			{
				result.Append("    " + req.Thing.GetInfusedLabelShort().CapitalizeFirst() + ": ");
				result.AppendLine((StatModOf(infSuffix).offset > 0 ? "+" : "-") +
				                  StatModOf(infSuffix).offset.ToAbs().ToStringTemperatureOffset());
			}
			if (StatModOf(infSuffix).multiplier != 1)
			{
				result.AppendLine("    " + req.Thing.GetInfusedLabelShort().CapitalizeFirst() + ": x" +
				                  StatModOf(infSuffix).multiplier.ToStringPercent());
			}
			return result.ToString();
		}
	}
}
