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
			if (!req.HasThing || req.Def.defName != "Human")
				return;
			
			//Return if the pawn has no weapon
			var pawn = req.Thing as Pawn;
			if (pawn == null || pawn.equipment.Primary == null)
				return;

			InfusionSet inf;
			if (!pawn.equipment.Primary.TryGetInfusions(out inf))
				return;

			StatMod mod;
			var stat = notifier.ToStatDef();
			if (stat == null)
			{
				Log.ErrorOnce("Could not find notifier's StatDef, which is " + notifier, 3388123);
				return;
			}
			var prefix = inf.Prefix.ToInfusionDef();
			var suffix = inf.Suffix.ToInfusionDef();

			if (!inf.PassPre && prefix.GetStatValue(stat, out mod))
			{
				val += mod.offset;
				val *= mod.multiplier;
			}
			if (inf.PassSuf || !suffix.GetStatValue(stat, out mod))
				return;

			val += mod.offset;
			val *= mod.multiplier;
		}
		public override string ExplanationPart(StatRequest req)
		{
			if (!req.HasThing)
				return null;

			//Return if the pawn has no weapon
			var pawn = req.Thing as Pawn;
			if (pawn == null || pawn.def.defName != "Human")
				return null;

			InfusionSet infusions;
			return pawn.equipment.Primary.TryGetInfusions(out infusions) ?
				WriteExplanation(pawn, infusions) :
				null;
		}

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
				result.Append("    " + pawn.equipment.Primary.GetInfusedLabel().CapitalizeFirst() + ": ");
				result.Append(mod.offset > 0 ? "+" : "-");
				result.AppendLine(mod.offset.ToAbs().ToStringPercent());
			}
			if (mod.multiplier == 1) return result.ToString();

			result.Append("    " + pawn.equipment.Primary.GetInfusedLabel().CapitalizeFirst() + ": x");
			result.AppendLine(mod.multiplier.ToStringPercent());
			return result.ToString();
		}
	}
}
