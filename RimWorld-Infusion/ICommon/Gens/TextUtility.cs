using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using RimWorld;
using Verse;

namespace Infusion
{
    [SuppressMessage( "ReSharper", "NonReadonlyMemberInGetHashCode" )]
    public static class GenInfusionText
    {
        private static Dictionary< int, string > infusedLabelDict = new Dictionary< int, string >();
        private const int LabelDictionaryMaxCount = 1000;

        /** Hash things. Taken from RimWorld base code. **/

        private struct InfusedLabelRequest
        {
            public Thing Thing;
            public BuildableDef BuildableDef;
            public ThingDef StuffDef;
            public int HitPoints;
            public int MaxHitPoints;

            public override int GetHashCode()
            {
                var num1 = 7437233;
                if ( Thing != null )
                {
                    num1 ^= Thing.GetHashCode()*712433;
                }
                var num2 = num1 ^ BuildableDef.GetHashCode()*345111;
                if ( StuffDef != null )
                {
                    num2 ^= StuffDef.GetHashCode()*666613;
                }
                var thingDef = BuildableDef as ThingDef;
                if ( thingDef == null )
                {
                    return num2;
                }

                InfusionSet inf;
                if ( Thing != null && Thing.TryGetInfusions( out inf ) )
                {
                    if ( !inf.PassPre )
                    {
                        num2 ^= inf.Prefix.GetHashCode();
                    }
                    if ( !inf.PassSuf )
                    {
                        num2 ^= inf.Suffix.GetHashCode();
                    }
                }

                if ( thingDef.useHitPoints )
                {
                    num2 = num2 ^ HitPoints*743273 ^ MaxHitPoints*7437;
                }
                return num2;
            }
        }

        /** End of the hash things. **/

        //Get one of existing infused labels from dictionary.
        public static string GetInfusedLabel( this Thing thing, bool isStuffed = true, bool isDetailed = true )
        {
            var request = new InfusedLabelRequest
            {
                BuildableDef = thing.def,
                Thing = thing
            };

            if ( isStuffed )
            {
                request.StuffDef = thing.Stuff;
            }
            if ( isDetailed )
            {
                request.MaxHitPoints = thing.MaxHitPoints;
                request.HitPoints = thing.HitPoints;
            }

            var hashCode = request.GetHashCode();
            string result;
            if ( infusedLabelDict.TryGetValue( hashCode, out result ) )
            {
                return result;
            }

            //Make a new label if there is none that matches.
            if ( infusedLabelDict.Count > LabelDictionaryMaxCount )
            {
                infusedLabelDict.Clear();
            }
            result = NewInfusedThingLabel( thing, isStuffed, isDetailed );
            //Save it to the dictionary.
            infusedLabelDict.Add( hashCode, result );
            return result;
        }

        //Make a new infused label.
        private static string NewInfusedThingLabel( Thing thing, bool isStuffed, bool isDetailed )
        {
            var result = new StringBuilder();

            InfusionSet inf;
            thing.TryGetInfusions( out inf );

            if ( !inf.PassPre )
            {
                result.Append( inf.Prefix.ToInfusionDef().label + " " );
            }

            string thingLabel;
            if ( isStuffed && thing.Stuff != null )
            {
                thingLabel = thing.Stuff.LabelAsStuff + " " + thing.def.label;
            }
            else
            {
                thingLabel = thing.def.label;
            }

            result.Append( !inf.PassSuf
                ? ResourceBank.StringInfusionOf.Translate( thingLabel, inf.Suffix.ToInfusionDef().label.CapitalizeFirst() )
                : thingLabel );

            if ( !isDetailed )
            {
                return result.ToString();
            }

            result.Append( " (" );
            QualityCategory qc;
            if ( thing.TryGetQuality( out qc ) )
            {
                result.Append( qc.GetLabelShort() );
            }

            if ( !(thing.HitPoints < thing.MaxHitPoints) )
            {
                return result + ")";
            }

            result.Append( " " + ((float) thing.HitPoints/thing.MaxHitPoints).ToStringPercent() + ")" );
            return result.ToString();
        }

        //Make a new infusion stat information.
        public static string GetInfusionDesc( this Thing thing )
        {
            InfusionSet inf;
            if ( !thing.TryGetInfusions( out inf ) )
            {
                return null;
            }

            var result = new StringBuilder( null );
            if ( !inf.PassPre )
            {
                var prefix = inf.Prefix.ToInfusionDef();
	            result.Append( ResourceBank.StringInfusionDescFrom.Translate( prefix.LabelCap ) )
	                  .Append( " (" )
	                  .Append( prefix.tier.Translate() )
	                  .AppendLine( ")" );

                foreach ( var current in prefix.stats )
                {
	                if ( current.Value.offset.FloatNotEqual( 0 ) )
	                {
		                result.Append( "     " + (current.Value.offset > 0 ? "+" : "-") );
		                if ( current.Key == StatDefOf.ComfyTemperatureMax ||
		                     current.Key == StatDefOf.ComfyTemperatureMin )
		                {
			                result.Append( current.Value.offset.ToAbs().ToStringTemperatureOffset() );
		                }
		                else
		                {
			                var modifier =
				                current.Key.parts.Find( s => s is StatPart_InfusionModifier ) as
					                StatPart_InfusionModifier;
			                if ( modifier != null )
			                {
				                if ( modifier.offsetUsePercentage )
				                {
					                result.Append( current.Value.offset.ToAbs().ToStringPercent() );
				                }
				                else
				                {
					                result.Append( current.Value.offset.ToAbs() + modifier.offsetSuffix );
				                }
			                }
		                }
		                result.AppendLine( " " + current.Key.LabelCap );
	                }
	                if ( current.Value.multiplier.FloatNotEqual( 1 ) )
	                {
		                result.Append( "     " + current.Value.multiplier.ToAbs().ToStringPercent() );
		                result.AppendLine( " " + current.Key.LabelCap );
					}
				}
                result.AppendLine();
            }
            if ( inf.PassSuf )
            {
                return result.ToString();
            }

            var suffix = inf.Suffix.ToInfusionDef();
			result.Append(ResourceBank.StringInfusionDescFrom.Translate(suffix.LabelCap))
				  .Append(" (")
				  .Append(suffix.tier.Translate())
				  .AppendLine(")");

			foreach ( var current in suffix.stats )
            {
                if ( current.Value.offset.FloatNotEqual( 0 ) )
                {
                    result.Append( "     " + (current.Value.offset > 0 ? "+" : "-") );
                    if ( current.Key == StatDefOf.ComfyTemperatureMax || current.Key == StatDefOf.ComfyTemperatureMin )
                    {
                        result.Append( current.Value.offset.ToAbs().ToStringTemperatureOffset() );
                    }
                    else
                    {
                        var modifier =
                            current.Key.parts.Find( s => s is StatPart_InfusionModifier ) as StatPart_InfusionModifier;
                        if ( modifier != null )
                        {
                            if ( modifier.offsetUsePercentage )
                            {
                                result.Append( current.Value.offset.ToAbs().ToStringPercent() );
                            }
                            else
                            {
                                result.Append( current.Value.offset.ToAbs() + modifier.offsetSuffix );
                            }
                        }
                    }
                    result.AppendLine( " " + current.Key.LabelCap );
                }
	            if ( current.Value.multiplier.FloatNotEqual( 1 ) )
	            {
		            result.Append( "     " + current.Value.multiplier.ToAbs().ToStringPercent() );
		            result.AppendLine( " " + current.Key.LabelCap );
	            }
            }
            return result.ToString();
        }
    }
}
