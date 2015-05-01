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
			//Display only when zoomed
			if (Find.CameraMap.CurrentZoom != CameraZoomRange.Closest) return;

			foreach (var current in InfusionLabelManager.Drawee)
			{
				InfusionTypes infType;
				if (current.parent.TryGetInfusion(out infType))
				{
					GenWorldUI.DrawThingLabel(
						GenWorldUI.LabelDrawPosFor(current.parent, -0.64f),
						infType.GetInfusionLabel(),
						new Color(1f, 0.5f, 0));
				}
			}
		}
	}
}
