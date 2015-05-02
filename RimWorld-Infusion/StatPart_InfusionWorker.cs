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

			InfusionPrefix infPrefix;
			InfusionSuffix infSuffix;
			if (pawn.equipment.Primary.TryGetInfusionPrefix(out infPrefix))
			{
				val += StatModOf(infPrefix).offset;
				if(StatModOf(infPrefix).multiplier != 0)
					val *= StatModOf(infPrefix).multiplier;
			}
			if (pawn.equipment.Primary.TryGetInfusionSuffix(out infSuffix))
			{
				val += StatModOf(infSuffix).offset;
				if (StatModOf(infSuffix).multiplier != 0)
					val *= StatModOf(infSuffix).multiplier;
			}
			if (infSuffix == InfusionSuffix.None) return;
			
			val *= StatModOf(infSuffix).multiplier;
		}
		public override string ExplanationPart(StatRequest req)
		{
			if (!req.HasThing)
				return null;
			if (req.Def != ThingDef.Named("Human"))
				return null;

			var pawn = req.Thing as Pawn;
			if (pawn == null)
				return null;

			InfusionPrefix infPrefix;
			pawn.equipment.Primary.TryGetInfusionPrefix(out infPrefix);
			InfusionSuffix infSuffix;
			pawn.equipment.Primary.TryGetInfusionSuffix(out infSuffix);
			if (infPrefix == InfusionPrefix.None && infSuffix == InfusionSuffix.None)
				return null;

			return WriteExplanation(req, infPrefix, infSuffix);
		}
		protected override string WriteExplanation(StatRequest req, InfusionPrefix infPrefix, InfusionSuffix infSuffix)
		{
			var pawn = req.Thing as Pawn;
			if (pawn == null)
				return null;

			var result = new StringBuilder();
			result.AppendLine(StaticSet.StringInfusionDescBonus);

			if (infPrefix != InfusionPrefix.None)
			{
				if (StatModOf(infPrefix).offset != 0)
				{
					result.Append("    " + pawn.equipment.Primary.GetInfusedLabelShort().CapitalizeFirst() + ": ");
					result.AppendLine((StatModOf(infPrefix).offset > 0 ? "+" : "-") +
									  StatModOf(infPrefix).offset.ToAbs().ToStringPercent());
				}
				if (StatModOf(infPrefix).multiplier != 1)
				{
					result.AppendLine("    " + pawn.equipment.Primary.GetInfusedLabelShort().CapitalizeFirst() + ": x" +
									  StatModOf(infPrefix).multiplier.ToStringPercent());
				}
			}
			if (infSuffix != InfusionSuffix.None)
			{
				if (StatModOf(infSuffix).offset != 0)
				{
					result.Append("    " + pawn.equipment.Primary.GetInfusedLabelShort().CapitalizeFirst() + ": ");
					result.AppendLine((StatModOf(infSuffix).offset > 0 ? "+" : "-") +
									  StatModOf(infSuffix).offset.ToAbs().ToStringPercent());
				}
				if (StatModOf(infSuffix).multiplier != 1)
				{
					result.AppendLine("    " + pawn.equipment.Primary.GetInfusedLabelShort().CapitalizeFirst() + ": x" +
									  StatModOf(infSuffix).multiplier.ToStringPercent());
				}
			}
			return result.ToString();
		}
	}
}
