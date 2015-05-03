using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Infusion
{
	class MapComponent_InfusionManager : MapComponent
	{
		private int lastTick;
		public override void MapComponentTick()
		{
			//Execute every 6 ticks
			var curTick = Find.TickManager.TicksGame;
			if (curTick - lastTick < 6)
				return;
			lastTick = curTick;

			var targetComps = new List<CompInfusion>();
			foreach (var current in Find.ListerPawns.AllPawns)
			{
				if (current.def != ThingDef.Named("Human"))
					continue;
				if (current.equipment.Primary == null)
					continue;
				var compInfusion = current.equipment.Primary.TryGetComp<CompInfusion>();
				if(compInfusion != null)
					targetComps.Add(compInfusion);
			}
			foreach (var current in targetComps)
			{
				if (current.IsTried)
					continue;
				current.SetInfusion();
			}
		}

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
				var prePass = false;
				InfusionPrefix infPrefix;
				if (!current.parent.TryGetInfusionPrefix(out infPrefix))
				{
					prePass = true;
				}
				var sufPass = false;
				InfusionSuffix infSuffix;
				if (!current.parent.TryGetInfusionSuffix(out infSuffix))
				{
					sufPass = true;
				}

				if (prePass && sufPass)
					continue;

				Color color;
				//When there is only suffix
				if (prePass)
				{
					if (infSuffix > InfusionSuffix.Tier3)
						color = new Color(1f, 0.25f, 0);
					else if (infSuffix > InfusionSuffix.Tier2)
						color = new Color(1f, 0.5f, 0);
					else
						color = new Color(1f, 0.75f, 0);
				}
				//When there is only prefix
				else if (sufPass)
				{
					if (infPrefix > InfusionPrefix.Tier3)
						color = new Color(1f, 0.25f, 0);
					else if (infPrefix > InfusionPrefix.Tier2)
						color = new Color(1f, 0.5f, 0);
					else
						color = new Color(1f, 0.75f, 0);
				}
				//When there is both prefix and suffix
				else
				{
					//Use color of higher tier
					if (infPrefix > InfusionPrefix.Tier3 || infSuffix > InfusionSuffix.Tier3)
						color = new Color(1f, 0.25f, 0);
					else if (infPrefix > InfusionPrefix.Tier2 || infSuffix > InfusionSuffix.Tier2)
						color = new Color(1f, 0.5f, 0);
					else
						color = new Color(1f, 0.75f, 0);
				}

				string label = null;
				if (!prePass)
				{
					label += infPrefix.GetInfusionLabelShort();
					if (!sufPass)
						label += " ";
				}
				if (!sufPass)
					label += infSuffix.GetInfusionLabelShort();

				GenWorldUI.DrawThingLabel(
					GenWorldUI.LabelDrawPosFor(current.parent, -0.66f), label, color);
			}
		}
	}
}
