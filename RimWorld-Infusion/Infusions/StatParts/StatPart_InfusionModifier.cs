using System.Diagnostics.CodeAnalysis;
using System.Text;
using RimWorld;
using Verse;

// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace Infusion
{
    //Modifier will change item's stats.

    [SuppressMessage( "ReSharper", "MemberCanBePrivate.Global" )]
    [SuppressMessage( "ReSharper", "InconsistentNaming" )]
    [SuppressMessage( "ReSharper", "FieldCanBeMadeReadOnly.Global" )]
    public class StatPart_InfusionModifier : StatPart
    {
        //As StatPart itself has no information about what it's adjusting, we will take a detour.
        //This string, notifier, has to be written in XML by mod maker.
        public string notifier; //Important: Must target its parent StatDef!
        public string offsetSuffix = null;
        public bool offsetUsePercentage = true;

        public override void TransformValue( StatRequest req, ref float val )
        {
            if ( !req.HasThing )
            {
                return;
            }

            InfusionSet inf;
            if ( !req.Thing.TryGetInfusions( out inf ) )
            {
                return;
            }

            StatMod mod;
            var stat = notifier.ToStatDef();
            if ( stat == null )
            {
                Log.ErrorOnce( "Could not find notifier's StatDef, which is " + notifier, 3388123 );
                return;
            }
            //"Notifier" will notify below lines about the actual StatDef it is adjusting, via XML-written notifier string.
            //InfusionSet is also string-based, so we have to find the InfusionDef behind it.
            var prefix = inf.Prefix.ToInfusionDef();
            var suffix = inf.Suffix.ToInfusionDef();
            //Return if the worker has no stat specified
            if ( !inf.PassPre && prefix.GetStatValue( stat, out mod ) )
            {
                val += mod.offset;
                val *= mod.multiplier;
            }
            if ( inf.PassSuf || !suffix.GetStatValue( stat, out mod ) )
            {
                return;
            }

            val += mod.offset;
            val *= mod.multiplier;
        }

        public override string ExplanationPart( StatRequest req )
        {
            if ( !req.HasThing )
            {
                return null;
            }

            InfusionSet infusions;
            return req.Thing.TryGetInfusions( out infusions )
                ? WriteExplanation( req.Thing, infusions )
                : null;
        }

        protected string WriteExplanation( Thing thing, InfusionSet infusions )
        {
            var result = new StringBuilder();

            if ( !infusions.PassPre )
            {
                result.Append( WriteExplanationDetail( thing, infusions.Prefix ) );
            }
            if ( !infusions.PassSuf )
            {
                result.Append( WriteExplanationDetail( thing, infusions.Suffix ) );
            }
            return result.ToString();
        }

        protected virtual string WriteExplanationDetail( Thing infusedThing, string val )
        {
            StatMod mod;
            var inf = val.ToInfusionDef();
            var result = new StringBuilder();
            if ( !inf.GetStatValue( notifier.ToStatDef(), out mod ) )
            {
                return null;
            }

            if ( mod.offset.FloatEqual(0) && mod.multiplier.FloatEqual(1) )
            {
	            return null;
            }

            if ( mod.offset.FloatNotEqual( 0 ) )
            {
                result.Append( "    " + inf.label.CapitalizeFirst() + ": " );
                result.Append( mod.offset > 0 ? "+" : "-" );
                string offsetValue;
                if ( offsetUsePercentage )
                {
                    offsetValue = mod.offset.ToAbs().ToStringPercent();
                }
                else
                {
                    offsetValue = mod.offset.ToAbs() + offsetSuffix;
                }
                result.AppendLine( offsetValue );
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
