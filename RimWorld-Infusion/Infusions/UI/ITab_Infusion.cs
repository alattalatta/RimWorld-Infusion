using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace Infusion
{
    internal class ITab_Infusion : ITab
    {
        private static readonly Vector2 WinSize = new Vector2( 400, 550 );

        private static CompInfusion SelectedCompInfusion
        {
            get
            {
                var thing = Find.Selector.SingleSelectedThing;
                return thing?.TryGetComp< CompInfusion >();
            }
        }

        public override bool IsVisible => SelectedCompInfusion != null && SelectedCompInfusion.Infused;

        public ITab_Infusion()
        {
            size = WinSize;
            labelKey = "TabInfusion";
        }

        protected override void FillTab()
        {
            //Label
            var rectBase = new Rect( 0f, 0f, WinSize.x, WinSize.y ).ContractedBy( 10f );
            var rectLabel = rectBase;
            Text.Font = GameFont.Medium;
            var label = GetRectLabel();
            Widgets.Label( rectLabel, label );

            //Quality
            var rectQuality = rectBase;
            rectQuality.yMin += 26;
            Text.Font = GameFont.Small;
            QualityCategory qc;
            SelectedCompInfusion.parent.TryGetQuality( out qc );

			var subLabel = new StringBuilder();
	        subLabel.Append( qc.GetLabel().CapitalizeFirst() )
	                    .Append( " " )
	                    .Append( ResourceBank.StringQuality )
	                    .Append( " " );
            if ( SelectedCompInfusion.parent.Stuff != null )
            {
	            subLabel.Append( SelectedCompInfusion.parent.Stuff.LabelAsStuff ).Append( " " );
            }
	        subLabel.Append( SelectedCompInfusion.parent.def.label );

            Widgets.Label( rectQuality, subLabel.ToString() );
            GUI.color = Color.white;
			
			//Infusion descriptions
			Text.Anchor = TextAnchor.UpperLeft;
			var rectDesc = rectBase;
            rectDesc.yMin += 56;
            Text.Font = GameFont.Small;
            Widgets.Label( rectDesc, SelectedCompInfusion.parent.GetInfusionDesc() );
        }

        private static string GetRectLabel()
        {
            var infs = SelectedCompInfusion.Infusions;
            var preDef = infs.Prefix.ToInfusionDef();
            var sufDef = infs.Suffix.ToInfusionDef();

            var result = new StringBuilder();
            if ( !infs.PassPre )
            {
                result.Append( preDef.LabelCap );
                if ( !infs.PassSuf )
                {
                    result.Append( " " );
                }
            }

            if ( !infs.PassSuf )
            {
                result.Append( sufDef.LabelCap );
            }

            Color color;
            if ( !infs.PassPre && infs.PassSuf )
            {
                color = preDef.tier.InfusionColor();
            }
            else if ( !infs.PassSuf && infs.PassPre )
            {
                color = sufDef.tier.InfusionColor();
            }
            else
            {
                color = MathUtility.Max( preDef.tier, sufDef.tier ).InfusionColor();
            }

            GUI.color = color;
            return result.ToString();
        }
    }
}
