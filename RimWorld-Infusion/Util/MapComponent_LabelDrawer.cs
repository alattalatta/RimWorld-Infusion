using UnityEngine;
using Verse;

namespace Infusion
{
	class MapComponent_LabelDrawer : MapComponent
	{
		public override void MapComponentOnGUI()
		{
			base.MapComponentOnGUI();
			Draw();
		}

		private static void Draw()
		{
			if (Find.CameraMap.CurrentZoom != CameraZoomRange.Closest) return;

			foreach (var current in InfusionLabelManager.Drawee)
			{
				InfusionSuffix infSuffix;
				if (!current.parent.TryGetInfusion(out infSuffix)) continue;

				Color color;
				if(infSuffix > InfusionSuffix.Tier3)
					color = new Color(1f, 0.5f, 0);
				else if(infSuffix > InfusionSuffix.Tier2)
					color = new Color(0.5f, 0, 0.5f);
				else
					color = new Color(0.2f, 0.2f, 0.8f);

				GenWorldUI.DrawThingLabel(
					GenWorldUI.LabelDrawPosFor(current.parent, -0.64f),
					infSuffix.GetInfusionLabel(),
					color);
			}
		}
	}
}
