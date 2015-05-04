using System.Text;
using Verse;

namespace Infusion
{
	public class StatPart_InfusionModifierTemperature : StatPart_InfusionModifier
	{
		protected override string WriteExplanationDetail(Thing thing, string val)
		{
			StatMod mod;
			var inf = val.ToInfusionDef();
			var result = new StringBuilder();
			if (!inf.GetStatValue(notifier.ToStatDef(), out mod)) return null;

			if (mod.offset != 0)
			{
				result.Append("    " + inf.label.CapitalizeFirst() + ": ");
				result.Append(mod.offset > 0 ? "+" : "-");
				result.AppendLine(mod.offset.ToAbs().ToStringTemperatureOffset());
			}
			if (mod.multiplier == 1) return result.ToString();

			result.Append("    " + inf.label.CapitalizeFirst() + ": x");
			result.AppendLine(mod.multiplier.ToStringPercent());
			return result.ToString();
		}
	}
}
