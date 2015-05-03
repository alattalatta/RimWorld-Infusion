using RimWorld;
using Verse;

namespace Infusion
{
	class ThingWithInfusions : ThingWithComps
	{
		public override string LabelBase
		{
			get
			{
				QualityCategory qc;
				if (!this.TryGetQuality(out qc) || qc < QualityCategory.Good || this.TryGetComp<CompInfusion>() == null)
					return base.LabelBase;

				return this.GetInfusedLabel();
			}
		}
	}
}
