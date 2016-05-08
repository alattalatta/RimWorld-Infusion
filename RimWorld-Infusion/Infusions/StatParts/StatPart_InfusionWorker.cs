using System;
using System.Text;
using RimWorld;
using Verse;

namespace Infusion
{
    //Worker will change item user's stats.

    public class StatPart_InfusionWorker : StatPart_InfusionModifier
    {
        public override void TransformValue( StatRequest req, ref float val )
        {
            if ( !req.HasThing)
            {
                return;
            }

            if (!(req.Thing is Pawn)) { return; }

            var pawn = req.Thing as Pawn;

            //Just in case
            if (pawn == null)
            {
                return;
            }

            if (pawn.RaceProps == null) { return; }
            if (!pawn.RaceProps.Humanlike) { return; }


            //Pawn has a primary weapon
            if ( pawn.equipment.Primary != null )
            {
                InfusionSet inf;
                if (pawn.equipment.Primary.TryGetInfusions( out inf ))
                {
                    StatMod mod;
                    var stat = notifier.ToStatDef();
                    if (stat == null)
                    {
                        Log.ErrorOnce( "Could not find notifier's StatDef, which is " + notifier, 3388123 );
                        return;
                    }
                    var prefix = inf.Prefix.ToInfusionDef();
                    var suffix = inf.Suffix.ToInfusionDef();

                    if (!inf.PassPre && prefix.GetStatValue( stat, out mod ))
                    {
                        val += mod.offset;
                        val *= mod.multiplier;
                    }
                    if (!inf.PassSuf && suffix.GetStatValue( stat, out mod ))
                    {
                        val += mod.offset;
                        val *= mod.multiplier;
                    }
                }
            }

            //Pawn has apparels
            if (pawn.apparel.WornApparelCount == 0)
            {
                return;
            }
            foreach ( var current in pawn.apparel.WornApparel )
            {
                InfusionSet inf;
                if (current.TryGetInfusions( out inf ))
                {
                    StatMod mod;
                    var stat = notifier.ToStatDef();
                    if (stat == null)
                    {
                        Log.ErrorOnce( "Could not find notifier's StatDef, which is " + notifier, 3388123 );
                        continue;
                    }
                    var prefix = inf.Prefix.ToInfusionDef();
                    var suffix = inf.Suffix.ToInfusionDef();

                    if (!inf.PassPre && prefix.GetStatValue( stat, out mod ))
                    {
                        val += mod.offset;
                        val *= mod.multiplier;
                    }
                    if (!inf.PassSuf && suffix.GetStatValue( stat, out mod ))
                    {
                        val += mod.offset;
                        val *= mod.multiplier;
                    }
                }
            }
        }

        public override string ExplanationPart( StatRequest req )
        {
            if (!req.HasThing)
            {
                return null;
            }

            if (!(req.Thing is Pawn)) { return null; }

            var pawn = req.Thing as Pawn;

            //Just in case
            if (pawn == null)
            {
                return null;
            }

            if (pawn.RaceProps == null) { return null; }
            if (!pawn.RaceProps.Humanlike) { return null; }


            InfusionSet infusions;
            var result = new StringBuilder();

			result.AppendLine(ResourceBank.StringInfusionDescBonus);

			//Pawn has a primary weapon
			if ( pawn.equipment.Primary.TryGetInfusions( out infusions ) )
            {
                result.Append( WriteExplanation( pawn.equipment.Primary, infusions ) );
            }

            //Pawn has apparels
            if ( pawn.apparel.WornApparelCount != 0 )
            {
                foreach ( var current in pawn.apparel.WornApparel )
                {
                    if ( current.TryGetInfusions( out infusions ) )
                    {
                        result.Append( WriteExplanation( current, infusions ) );
                    }
                }
            }

            return result.ToString();
        }

        protected override string WriteExplanationDetail( Thing infusedThing, string val )
        {
            StatMod mod;
            var inf = val.ToInfusionDef();
            var result = new StringBuilder();

            //No mod for this stat
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
                result.Append( "    " + infusedThing.GetInfusedLabel().CapitalizeFirst() + ": " );
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
                result.Append( "    " + infusedThing.GetInfusedLabel().CapitalizeFirst() + ": x" );
                result.AppendLine( mod.multiplier.ToStringPercent() );
			}

			return result.ToString();
        }
    }
}
