using System.Text;
using Verse;

namespace Infusion
{
	public class StatPart_InfusionWorkerCps : StatPart_InfusionWorker
	{
		protected override string WriteExplanationDetail(Thing thing, string val)
		{
			var pawn = thing as Pawn;
			if (pawn == null)
				return null;

			StatMod mod;
			var inf = val.ToInfusionDef();
			var result = new StringBuilder();
			if (!inf.GetStatValue(notifier.ToStatDef(), out mod)) return null;

			if (mod.offset != 0)
			{
				result.Append("    " + pawn.equipment.Primary.GetInfusedLabel() + ": ");
				result.Append(mod.offset > 0 ? "+" : "-");
				result.AppendLine(mod.offset.ToAbs() + "c/s");
			}
			if (mod.multiplier == 1) return result.ToString();

			result.Append("    " + pawn.equipment.Primary.GetInfusedLabel() + ": x");
			result.AppendLine(mod.multiplier.ToStringPercent());
			return result.ToString();
		}
	}
}
