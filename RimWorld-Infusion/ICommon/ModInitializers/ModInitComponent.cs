using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;

namespace Infusion
{
    /// <summary>
    /// A handler for every injection/replacement.
    /// </summary>
    public class ModInitComponent : MonoBehaviour
    {
        private void OnLevelWasLoaded( int level )
        {
            if ( level != 1 )
            {
                return;
            }
            //if (!ResourceBank.FindModActive("LT_Infusion") && !ResourceBank.FindModActive("LT_Gunparts"))
            //return;

            try
            {
                InfusionLabelManager.ReInit();

                InjectVarious();
                Log.Message( "LT-IN: Initialized Infusion." );
            }
            catch ( Exception ex )
            {
                Log.Error( "LT-IN: Error Initializing Infusion. Exception:\n" + ex );
            }
        }

        //Inject every prerequisites to defs.
	    private void InjectVarious()
	    {
		    //Access ThingDef database with each def's defName.
		    var field = typeof ( DefDatabase< ThingDef > ).GetField( "defsByName",
		                                                                  BindingFlags.Static | BindingFlags.NonPublic );
		    if ( field == null )
		    {
			    throw new Exception( "LT-IN: field is null" );
		    }
		    var defsByName = field.GetValue( null ) as Dictionary< string, ThingDef >;
		    if ( defsByName == null )
		    {
			    throw new Exception( "LT-IN: Could not access private members" );
		    }
		    try
		    {
			    foreach (
				    var current in
					    defsByName.Values.Where( current => current.IsMeleeWeapon ||
					                                        current.IsRangedWeapon ||
					                                        current.IsApparel ) )
			    {
				    ReplaceClass( current );

				    if ( AddCompInfusion( current ) )
				    {
					    AddInfusionITab( current );
				    }
			    }
		    }
		    catch ( Exception e )
		    {
			    throw new Exception("LT-IN: Met error while injecting.\n" + e);
		    }
        }
        
        //Inject new ThingComp.
        private static bool AddCompInfusion( ThingDef def )
        {
            if ( def.comps.Exists( s => s.compClass == typeof ( CompInfusion ) ) )
            {
                return false;
            }

            if ( !def.comps.Exists( s => s.compClass == typeof ( CompQuality ) ) )
            {
                return false;
            }

            //As we are adding, not replacing, we need a fresh CompProperties.
            //We don't need anything except compClass as CompInfusion does not take anything.
            var compProperties = new CompProperties {compClass = typeof ( CompInfusion )};
            def.comps.Add( compProperties );

            return true;
        }

        //Replace ThingWithComps with ThingWithInfusions or ApparelWithInfusions.
        private static void ReplaceClass( ThingDef def )
        {
            if ( def.IsWeapon && def.thingClass == typeof ( ThingWithComps ) )
            {
                def.thingClass = typeof ( ThingWithInfusions );
            }
            else if ( def.IsApparel && def.thingClass == typeof ( Apparel ) )
            {
                def.thingClass = typeof ( ApparelWithInfusions );
            }
        }


        //Inject new ITab to given def.
        private static void AddInfusionITab( ThingDef def )
        {
	        if ( def.inspectorTabs == null || def.inspectorTabs.Count == 0 )
	        {
		        def.inspectorTabs = new List< Type >();
		        def.inspectorTabsResolved = new List< ITab >();
	        }
	        if ( def.inspectorTabs.Contains( typeof ( ITab_Infusion ) ) )
	        {
		        return;
	        }
	        def.inspectorTabs.Add( typeof ( ITab_Infusion ) );
	        def.inspectorTabsResolved.Add( ITabManager.GetSharedInstance( typeof ( ITab_Infusion ) ) );
        }
    }
}
