using System.Collections.Generic;
using System.Text;
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

			InfuseEquipments();
		}
		public override void MapComponentOnGUI()
		{
			base.MapComponentOnGUI();
			Draw();
		}

		private static void InfuseEquipments()
		{
			var targetComps = new List<CompInfusion>();
			foreach (var current in Find.ListerPawns.AllPawns)
			{
				if (current.def != ThingDef.Named("Human"))
					continue;
				if (current.equipment.Primary == null)
					continue;
				var compInfusion = current.equipment.Primary.TryGetComp<CompInfusion>();
				if (compInfusion != null)
					targetComps.Add(compInfusion);
			}
			foreach (var current in targetComps)
			{
				if (current.Tried)
					continue;
				current.SetInfusion();
			}
		}
		private static void Draw()
		{
			if (Find.CameraMap.CurrentZoom != CameraZoomRange.Closest) return;
			if (InfusionLabelManager.Drawee.Count == 0)
				return;

			Log.Message(InfusionLabelManager.Drawee.Count.ToString());

			foreach (var current in InfusionLabelManager.Drawee)
			{
				//We don't have check if inf is Infused or not, as Drawee only has CompInfusion of at least one infusion.
				var inf = current.Infusions;
				var prefix = current.Infusions.Prefix.ToInfusionDef();
				var suffix = current.Infusions.Suffix.ToInfusionDef();

				var color = new Color();
				//When there is only suffix
				if (inf.PassPre)
				{
					switch (suffix.tier)
					{
						case InfusionTier.Tier1:
							color = StaticSet.ColorTier1;
							break;
						case InfusionTier.Tier2:
							color = StaticSet.ColorTier2;
							break;
						case InfusionTier.Tier3:
							color = StaticSet.ColorTier3;
							break;
					}
				}
				//When there is only prefix
				else if (inf.PassSuf)
				{
					switch (prefix.tier)
					{
						case InfusionTier.Tier1:
							color = StaticSet.ColorTier1;
							break;
						case InfusionTier.Tier2:
							color = StaticSet.ColorTier2;
							break;
						case InfusionTier.Tier3:
							color = StaticSet.ColorTier3;
							break;
					}
				}
				//When there are both prefix and suffix
				else
				{
					//Use color of higher tier
					if (prefix.tier == InfusionTier.Tier3 || suffix.tier == InfusionTier.Tier3)
						color = StaticSet.ColorTier3;
					else if (prefix.tier == InfusionTier.Tier2 || suffix.tier == InfusionTier.Tier2)
						color = StaticSet.ColorTier2;
					else
						color = StaticSet.ColorTier1;
				}

				var result = new StringBuilder();
				if (!inf.PassPre)
				{
					result.Append(prefix.labelShort);
					if (!inf.PassSuf)
						result.Append(" ");
				}
				if (!inf.PassSuf)
					result.Append(suffix.labelShort);

				GenWorldUI.DrawThingLabel(
					GenWorldUI.LabelDrawPosFor(current.parent, -0.66f), result.ToString(), color);
			}
		}
	}
}
