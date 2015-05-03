using System;
using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;

namespace Infusion
{
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
				InjectMapcomp();
				TryInjectComp();
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

		private void InjectMapcomp()
		{
			if (Find.Map == null || Find.Map.components == null) return;

			if (Find.Map.components.FindAll(x => x.GetType().ToString() == CompName).Count != 0) return;

			Find.Map.components.Add(mapComp);
			Log.Message(ModName + " : " + CompName + " Injected");
		}
		private void TryInjectComp()
		{
			if (!IsModLoaded())
			{
				Log.Warning(ModName + " : Mod not loaded");
				return;
			}

			//Access ThingDef database with each def's defName.
			var typeFromHandle = typeof(DefDatabase<ThingDef>);
			var defsByName = typeFromHandle.GetField("defsByName", BindingFlags.Static | BindingFlags.NonPublic);
			if (defsByName == null)
			{
				Log.Error(ModName + " : field == null");
				return;
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
				{
					continue;
				}

				AddCompInfusion(cur.Value);
			}
			Log.Message("Initialized " + ModName);
		}

		/// <summary>
		/// Inject new CompInfusion to existing defs. Only works when the def has CompQuality.
		/// </summary>
		/// <param name="def">ThingDef to be added.</param>
		private static void AddCompInfusion(ThingDef def)
		{
			var qualityExist = false;
			foreach (var current in def.comps)
			{
				//We don't want to add a comp that is already there.
				if (current.compClass == typeof(CompInfusion))
				{
					return;
				}
				//Only add when CompQuality exists. Pass when we already know that it has CompQuality.
				if (!qualityExist && current.compClass == typeof(CompQuality))
				{
					qualityExist = true;
				}
			}
			if (!qualityExist) return;

			//As we are adding, not replacing, we need a fresh CompProperties.
			//We don't need anything except compClass as CompInfusion does not take anything.
			var compProperties = new CompProperties { compClass = typeof(CompInfusion) };
			def.comps.Add(compProperties);

			TryReplaceClass(def);
		}

		/// <summary>
		/// Replace ThingWithComps with ThingWithInfusions.
		/// </summary>
		/// <param name="def">ThingDef to be replaced.</param>
		private static void TryReplaceClass(ThingDef def)
		{
			if(def.thingClass == typeof(ThingWithComps))
				def.thingClass = typeof (ThingWithInfusions);
		}
	}
}
