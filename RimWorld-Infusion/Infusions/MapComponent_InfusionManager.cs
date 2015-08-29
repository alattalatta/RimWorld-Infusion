using System;
using System.Text;
using UnityEngine;
using Verse;

namespace Infusion
{
    class MapComponent_InfusionManager : MapComponent
    {
        private int lastTick;
        private bool welfare = true;

        public override void MapComponentTick()
        {
            if ( welfare == false )
                return;

            //Execute every 12 ticks
            var curTick = Find.TickManager.TicksGame;
            if (curTick - lastTick < 12)
                return;

            try
            {
                lastTick = curTick;

                FindAndInfuseEquipments();
            }
            catch ( Exception e )
            {
                Log.Message( "LT-IN: InfusionManager met error. Hibernating." );
                Log.Error( e.ToString() );
                welfare = false;
            }
        }
        public override void MapComponentOnGUI()
        {
            Draw();
        }

        //Infuse items of raiders and visitors, etc
        private static void FindAndInfuseEquipments()
        {
            foreach (var pawn in Find.ListerPawns.AllPawns)
            {
                // No humanlike neither mechanoid, pass
                if (!pawn.def.race.Humanlike && !pawn.def.race.mechanoid)
                    continue;

                CompInfusion compInfusion;

                //Pawn has primary
                if (pawn.equipment.Primary != null)
                {
                    compInfusion = pawn.equipment.Primary.TryGetComp<CompInfusion>();
                    if (compInfusion != null && !compInfusion.Tried)
                    {
#if DEBUG
                        Log.Message("Infusing: " + pawn + " " + pawn.equipment.Primary);
#endif
                        compInfusion.SetInfusion();
                        compInfusion.Tried = true;
                    }
                }

                //Pawn has apparel
                if ( pawn.apparel.WornApparelCount != 0 )
                {
                    foreach ( var curApparel in pawn.apparel.WornApparel )
                    {
                        compInfusion = curApparel.TryGetComp< CompInfusion >();
                        if ( compInfusion != null && !compInfusion.Tried )
                        {
#if DEBUG
                            Log.Message("Infusing: " + pawn + " " + curApparel);
#endif
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
