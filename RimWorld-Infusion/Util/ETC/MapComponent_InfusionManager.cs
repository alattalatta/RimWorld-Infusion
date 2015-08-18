using System.Text;
using Infusion.Util;
using UnityEngine;
using Verse;
using Find = Verse.Find;

namespace Infusion
{
	class MapComponent_InfusionManager : MapComponent
	{
		private int lastTick;

		public override void MapComponentTick()
		{
			//Execute every 8 ticks
			var curTick = Find.TickManager.TicksGame;
			if (curTick - lastTick < 8)
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
			foreach (var current in Find.ListerPawns.AllPawns)
			{
				//If not tool equippable or primary equipment is null, pass
				if (!current.def.race.ToolUser || current.equipment.Primary == null)
					continue;

				var compInfusion = current.equipment.Primary.TryGetComp<CompInfusion>();
				if (compInfusion != null && !compInfusion.Tried)
				{
					compInfusion.SetInfusion();
					compInfusion.Tried = true;
				}
			}
		}
		private static void Draw()
		{
			if (Find.CameraMap.CurrentZoom != CameraZoomRange.Closest) return;
			if (InfusionLabelManager.Drawee.Count == 0)
				return;

			foreach (var current in InfusionLabelManager.Drawee)
			{
				var inf = current.Infusions;
				var prefix = current.Infusions.Prefix.ToInfusionDef();
				var suffix = current.Infusions.Suffix.ToInfusionDef();

				Color color;
				//When there is only suffix
				if (inf.PassPre)
				{
					color = suffix.tier.InfusionColor();
				}
				//When there is only prefix
				else if (inf.PassSuf)
				{
					color = prefix.tier.InfusionColor();
				}
				//When there are both prefix and suffix
				else
				{
					color = GenInfusionMath.Max(prefix.tier, suffix.tier).InfusionColor();
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
