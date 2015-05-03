using RimWorld;
using Verse;

namespace Infusion
{
	class ThingWithInfusions : ThingWithComps
	{
		public override string Label
		{
			get
			{
				QualityCategory qc;
				if (!this.TryGetQuality(out qc) || qc < QualityCategory.Good || this.TryGetComp<CompInfusion>() == null)
					return base.Label;

				return this.GetInfusedLabel();
			}
		}
	}
}
