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
				TryAddComp();
			}
			catch (Exception ex)
			{
				Log.Error(ModName + " : Error Initializing mod");
				Log.Error(ex.ToString());
			}
		}
		/*
		public void Update()
		{
			if (isDone) return;

			try
			{
				InjectMapcomp();
				TryAddComp();
			}
			catch (Exception ex)
			{
				Log.Error(ModName + ": Error initializing mod");
				Log.Error(ex.ToString());
			}
			isDone = true;
		}*/
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
		private void TryAddComp()
		{
			if (!IsModLoaded())
			{
				Log.Warning(ModName + " : Mod not loaded");
				return;
			}
			var typeFromHandle = typeof(DefDatabase<ThingDef>);
			var defsList = typeFromHandle.GetField("defsList", BindingFlags.Static | BindingFlags.NonPublic);
			var defsByName = typeFromHandle.GetField("defsByName", BindingFlags.Static | BindingFlags.NonPublic);
			if (defsList == null)
			{
				Log.Error(ModName + " : field == null");
				return;
			}
			if (defsByName == null)
			{
				Log.Error(ModName + " : field2 == null");
				return;
			}
			var value = defsList.GetValue(null);
			var list = value as List<ThingDef>;
			var value2 = defsByName.GetValue(null);
			var dictionary = value2 as Dictionary<string, ThingDef>;
			if (list == null || dictionary == null)
			{
				throw new Exception(ModName + ": Could not access private members");
			}
			foreach (KeyValuePair<string, ThingDef> cur in dictionary)
			{
				if (!cur.Value.IsMeleeWeapon && !cur.Value.IsRangedWeapon)
				{
					continue;
				}

				AddComp(cur.Value, typeof(CompInfusion));
			}
			Log.Message("Initialized " + ModName);
		}
		private static void AddComp(ThingDef def, Type compToAdd)
		{
			var qualityExist = false;
			foreach (var current in def.comps)
			{
				if (current.compClass == compToAdd)
				{
					return;
				}
				if (current.compClass == typeof(CompQuality))
				{
					qualityExist = true;
				}
			}
			if (!qualityExist) return;

			var compProperties = new CompProperties { compClass = compToAdd };
			def.comps.Add(compProperties);
		}
	}
}
