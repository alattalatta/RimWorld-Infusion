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
	public class InitComponent : MonoBehaviour
	{
		private readonly MapComponent_InfusionManager mapComp = new MapComponent_InfusionManager();

		private const string ModName = "LT_Infusion";
		private const string CompName = "Infusion.MapComponent_InfusionManager";
		private void OnLevelWasLoaded(int level)
		{
			if (level != 1) return;

			try
			{
				InfusionLabelManager.ReInit();
				if (!IsModLoaded())
					throw new Exception(ModName + " : Mod not loaded");

				InjectMapcomp();
				InjectVarious();
				Util.Find.Init();
				Log.Message("Initialized the " + ModName + "mod");
			}
			catch (Exception ex)
			{
				Log.Error(ModName + " : Error Initializing mod");
				Log.Error(ex.ToString());
			}
		}
		private static bool IsModLoaded()
		{
			foreach (var current in LoadedModManager.LoadedMods)
			{
				if (current.name == ModName)
				{
					return true;
				}
			}
			return false;
		}

		//Inject MapComponent_InfusionManager for various infusion handling.
		private void InjectMapcomp()
		{
			if (Find.Map == null || Find.Map.components == null) return;

			if (Find.Map.components.FindAll(x => x.GetType().ToString() == CompName).Count != 0) return;

			Find.Map.components.Add(mapComp);
			//Log.Message("Injected new MapComponent by " + ModName);
		}

		//Inject every prerequisites to defs.
		private void InjectVarious()
		{
			//Access ThingDef database with each def's defName.
			var typeFromHandle = typeof(DefDatabase<ThingDef>);
			var defsByName = typeFromHandle.GetField("defsByName", BindingFlags.Static | BindingFlags.NonPublic);
			if (defsByName == null)
			{
				throw new NullReferenceException(ModName + "defsByName is null");
			}
			var valDefsByName = defsByName.GetValue(null);
			var dictDefsByName = valDefsByName as Dictionary<string, ThingDef>;
			if (dictDefsByName == null)
			{
				throw new Exception(ModName + ": Could not access private members");
			}
			foreach (KeyValuePair<string, ThingDef> cur in dictDefsByName)
			{
				if (!cur.Value.IsMeleeWeapon && !cur.Value.IsRangedWeapon)
					continue;

				if (AddCompInfusion(cur.Value))
				{
					//We need ITab_Infusion only when the thing has CompInfusion already.
					InjectITab(cur.Value);
				}
			}
			//Log.Message("Injected new ThingComp by " + ModName);
		}
 
		//Inject new CompInfusion to given def. Only works when the def has CompQuality.
		private static bool AddCompInfusion(ThingDef def)
		{
			var qualityExist = false;
			foreach (var current in def.comps)
			{
				//We don't want to add a comp that is already there.
				if (current.compClass == typeof(CompInfusion))
				{
					return true;
				}
				//Only add when CompQuality exists. Pass when we already know that it has CompQuality.
				if (!qualityExist && current.compClass == typeof(CompQuality))
				{
					qualityExist = true;
				}
			}
			if (!qualityExist) return false;

			//As we are adding, not replacing, we need a fresh CompProperties.
			//We don't need anything except compClass as CompInfusion does not take anything.
			var compProperties = new CompProperties { compClass = typeof(CompInfusion) };
			def.comps.Add(compProperties);

			ReplaceClass(def);
			return true;
		}

		//Replace ThingWithComps with ThingWithInfusions.
		private static void ReplaceClass(ThingDef def)
		{
			if (def.thingClass == typeof (ThingWithComps))
				def.thingClass = typeof (ThingWithInfusions);
		}

		//Inject new ITab_Infusion to given def.
		private static void InjectITab(ThingDef def)
		{
			if (def.inspectorTabs == null || def.inspectorTabs.Count == 0)
			{
				def.inspectorTabs = new List<Type>();
				def.inspectorTabsResolved = new List<ITab>();
			}
			if (def.inspectorTabs.Contains(typeof(ITab_Infusion)))
				return;

			try
			{
				def.inspectorTabs.Add(typeof (ITab_Infusion));
				def.inspectorTabsResolved.Add(ITabManager.GetSharedInstance(typeof (ITab_Infusion)));
			}
			catch (Exception ex)
			{
				Log.Warning(ModName + " : Failed to inject an ITab to " + def.label);
				Log.Warning(ex.ToString());
			}
		}
	}
}
