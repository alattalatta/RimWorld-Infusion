using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Infusion
{
	public enum InfusionType
	{
		Undefined,
		Prefix,
		Suffix
	}

	public enum InfusionTier
	{
		Undefined,
		Common,
		Uncommon,
		Rare,
		Epic,
		Legendary,
		Artifact
	}

	public class InfusionAllowance
	{
		public bool melee = true;
		public bool ranged = true;
		public bool apparel = false;
	}

	public class InfusionDef : Def
	{
		public string labelShort = "#NN";
		public Dictionary<StatDef, StatMod> stats = new Dictionary<StatDef, StatMod>();

		public InfusionType type = InfusionType.Undefined;
		public InfusionTier tier = InfusionTier.Undefined;

		public InfusionAllowance allowance = new InfusionAllowance();
		//public bool furniture = false;

		/// <summary>
		/// Get matching StatMod for given StatDef. Returns false when none.
		/// </summary>
		public bool GetStatValue(StatDef stat, out StatMod mod)
		{
			return stats.TryGetValue(stat, out mod);
		}
	}
}
