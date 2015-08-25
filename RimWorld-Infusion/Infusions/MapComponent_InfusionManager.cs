using System.Text;
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
            //Execute every 12 ticks
            var curTick = Find.TickManager.TicksGame;
            if (curTick - lastTick < 12)
                return;
            lastTick = curTick;

            InfuseEquipments();
        }
        public override void MapComponentOnGUI()
        {
            base.MapComponentOnGUI();
            Draw();
        }

        //Infuse items of raiders and visitors, etc
        private static void InfuseEquipments()
        {
            foreach (var current in Find.ListerPawns.AllPawns)
            {
                //If not tool equippable, pass
                if (!current.def.race.ToolUser)
                    continue;

                CompInfusion compInfusion;

                //Pawn has primary
                if (current.equipment.Primary != null)
                {
                    compInfusion = current.equipment.Primary.TryGetComp<CompInfusion>();
                    if (compInfusion != null && !compInfusion.Tried)
                    {
                        compInfusion.SetInfusion();
                        compInfusion.Tried = true;
                    }
                }

                //Pawn has apparel
                if ( current.apparel.WornApparelCount != 0 )
                {
                    foreach ( var curApparel in current.apparel.WornApparel )
                    {
                        compInfusion = curApparel.TryGetComp< CompInfusion >();
                        if ( compInfusion != null && !compInfusion.Tried )
                        {
                            compInfusion.SetInfusion();
                            compInfusion.Tried = true;
                        }
                    }
                }
            }
        }
        //Draw infusion label on map
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
                    color = MathUtility.Max(prefix.tier, suffix.tier).InfusionColor();
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
