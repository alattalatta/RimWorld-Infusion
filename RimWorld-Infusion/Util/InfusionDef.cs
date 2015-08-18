using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using RimWorld;
using Verse;

namespace Infusion
{
	public enum InfusionType
	{
		Undefined,
		Prefix,
		Suffix,
		Depiction //Not implemented!
	}

	public enum InfusionTier
	{
		Undefined,
		Uncommon,
		Rare,
		Epic,
		Legendary,
		Artifact
	}

	public class InfusionDef : Def
	{
		/**
		 * Inherited:
		 * public string defName;
		 * public string label;
		 * public string description;
		 */
		public string labelShort = "#NN";
		public Dictionary<StatDef, StatMod> stats = new Dictionary<StatDef, StatMod>();

		public InfusionType type = InfusionType.Undefined;
		public InfusionTier tier = InfusionTier.Undefined;

		public bool melee = true;
		public bool ranged = true;
		public bool apparel = false;
		//public bool furniture = false;

		/// <summary>
		/// Get matching StatMod for given stat, from this def. Returns false when none.
		/// </summary>
		/// <param name="stat"></param>
		/// <param name="mod"></param>
		/// <returns></returns>
		public bool GetStatValue(StatDef stat, out StatMod mod)
		{
			return stats.TryGetValue(stat, out mod);
		}

		public override void PostLoad()
		{
			base.PostLoad();
			foreach (KeyValuePair<StatDef, StatMod> current in stats)
			{
				if(current.Key == null)
					Log.Message("StatKey error in infusion " + label);
				if(current.Value == null)
					Log.Message("StatMod error in infuison " + label);
			}
			if(type == InfusionType.Undefined)
				Log.Error(defName + ": Infusion has no type! Available values: Preffix, Suffix");
			if(tier == InfusionTier.Undefined)
				Log.Error(defName + ": Infusion has no tier! Available values: Uncommon, Rare, Epic, Legendary, Artifact");
		}
	}
}
