using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace Infusion
{
	class ITab_Infusion : ITab
	{
		private static readonly Vector2 WinSize = new Vector2(400, 200);

		private static CompInfusion SelectedCompInfusion
		{
			get
			{
				var thing = Find.Selector.SingleSelectedThing;
				return thing != null ? thing.TryGetComp<CompInfusion>() : null;
			}
		}
		public override bool IsVisible
		{
			get { return SelectedCompInfusion != null && SelectedCompInfusion.Infused; }
		}

		public ITab_Infusion()
		{
			size = WinSize;
			labelKey = "TabInfusion";
		}

		protected override void FillTab()
		{
			var rectBase = new Rect(0f, 0f, WinSize.x, WinSize.y).ContractedBy(10f);
			var rectLabel = rectBase;
			Text.Font = GameFont.Medium;
			Widgets.Label(rectLabel, GetRectLabel());

			var rectQuality = rectBase;
			rectQuality.yMin += 27;
			Text.Font = GameFont.Small;
			QualityCategory qc;
			SelectedCompInfusion.parent.TryGetQuality(out qc);
			Widgets.Label(rectQuality, qc.GetLabel().CapitalizeFirst() + " quality");

			var rectDesc = rectBase;
			rectDesc.yMin += 55f;
			Text.Font = GameFont.Small;
			Widgets.Label(rectDesc, SelectedCompInfusion.parent.GetInfusedDescriptionITab());
		}

		private static string GetRectLabel()
		{
			var infs = SelectedCompInfusion.Infusions;

			var result = new StringBuilder();
			if (!infs.PassPre)
				result.Append(infs.Prefix.ToInfusionDef().label.CapitalizeFirst());
			if (!infs.PassPre && !infs.PassSuf)
				result.Append(" / ");
			if (!infs.PassSuf)
				result.Append(infs.Suffix.ToInfusionDef().label.CapitalizeFirst());

			return result.ToString();
		}
	}
}
