using System;
using System.Collections.Generic;
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
            var typeFromHandle = typeof ( DefDatabase< ThingDef > );
            var defsByName = typeFromHandle.GetField( "defsByName", BindingFlags.Static | BindingFlags.NonPublic );
            if ( defsByName == null )
            {
                throw new NullReferenceException( "LT: defsByName is null" );
            }
            var valDefsByName = defsByName.GetValue( null );
            var dictDefsByName = valDefsByName as Dictionary< string, ThingDef >;
            if ( dictDefsByName == null )
            {
                throw new Exception( "LT: Could not access private members" );
            }
            foreach ( var current in dictDefsByName )
            {
                if ( !current.Value.IsMeleeWeapon && !current.Value.IsRangedWeapon && !current.Value.IsApparel )
                {
                    continue;
                }

                ReplaceClass( current.Value );

                if ( AddCompInfusion( current.Value ) )
                {
                    AddInfusionITab( current.Value );
                }
            }
            //Log.Message("Injected new ThingComp by " + ModName);
        }


        //Inject new ThingComp.
        private static bool AddCompInfusion( ThingDef def )
        {
            var qualityExist = false;
            foreach ( var current in def.comps )
            {
                //Only add when CompQuality exists. Pass when we already know that it has CompQuality.
                if ( !qualityExist && current.compClass == typeof ( CompQuality ) )
                {
                    qualityExist = true;
                }
            }
            if ( !qualityExist )
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
            else if ( def.IsApparel && ResourceBank.FindModActive( "LT_Infusion" ) &&
                      def.thingClass == typeof ( Apparel ) )
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

            try
            {
                def.inspectorTabs.Add( typeof ( ITab_Infusion ) );
                def.inspectorTabsResolved.Add( ITabManager.GetSharedInstance( typeof ( ITab_Infusion ) ) );
            }
            catch ( Exception ex )
            {
                Log.Warning( "LT: Failed to inject an ITab to " + def.label );
                Log.Warning( ex.ToString() );
            }
        }
    }
}
