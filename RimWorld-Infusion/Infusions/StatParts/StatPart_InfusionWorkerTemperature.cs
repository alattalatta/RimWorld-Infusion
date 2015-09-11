using System.Text;
using Verse;

namespace Infusion
{
    public class StatPart_InfusionWorkerTemperature : StatPart_InfusionWorker
    {
        protected override string WriteExplanationDetail( Thing infusedThing, string val )
        {
            //Apparel's temperature stat only uses InfusionModifier. Skipping.

            var pawn = infusedThing as Pawn;
            if ( pawn == null )
            {
                return null;
            }

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
                result.Append( "    " + pawn.equipment.Primary.GetInfusedLabel().CapitalizeFirst() + ": " );
                result.Append( mod.offset > 0 ? "+" : "-" );
                result.AppendLine( mod.offset.ToAbs().ToStringTemperatureOffset() );
            }
            if ( mod.multiplier.FloatNotEqual( 1 ) )
            {
                result.Append( "    " + pawn.equipment.Primary.GetInfusedLabel().CapitalizeFirst() + ": x" );
                result.AppendLine( mod.multiplier.ToStringPercent() );
                return result.ToString();
            }

            return result.ToString();
        }
    }
}
