using System.Text;
using Verse;

namespace Infusion
{
    public class StatPart_InfusionModifierTemperature : StatPart_InfusionModifier
    {
        protected override string WriteExplanationDetail( Thing infusedThing, string val )
        {
            StatMod mod;
            var inf = val.ToInfusionDef();
            var result = new StringBuilder();
            if ( !inf.GetStatValue( notifier.ToStatDef(), out mod ) )
            {
                return null;
            }

            if ( mod.offset.FloatEqual( 0 ) && mod.multiplier.FloatEqual( 1 ) )
            {
	            return null;
            }

            if ( mod.offset.FloatNotEqual( 0 ) )
            {
                result.Append( "    " + inf.label.CapitalizeFirst() + ": " );
                result.Append( mod.offset > 0 ? "+" : "-" );
                result.AppendLine( mod.offset.ToAbs().ToStringTemperatureOffset() );
            }
            if ( mod.multiplier.FloatNotEqual( 1 ) )
            {
                result.Append( "    " + inf.label.CapitalizeFirst() + ": x" );
                result.AppendLine( mod.multiplier.ToStringPercent() );
                return result.ToString();
            }

            return result.ToString();
        }
    }
}
