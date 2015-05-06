using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Infusion.Util
{
	static class Find
	{
		public static List<InfusionDef> AllPossiblePrefixes { get; private set; }
		public static List<InfusionDef> AllPossibleSuffixes { get; private set; }

		public static List<Thing> AllInfusers
		{
			get { return Verse.Find.ListerThings.AllThings.FindAll(s => s.def.defName == "PlantPot"); }
		} 

		public static void Init()
		{
			if (!DefDatabase<InfusionDef>.AllDefs.Any())
			{
				throw new Exception("No infusions found!");
			}

			AllPossiblePrefixes = (
				from d in DefDatabase<InfusionDef>.AllDefs
				where d.type == InfusionType.Prefix
				select d).ToList();
			AllPossibleSuffixes = (
				from d in DefDatabase<InfusionDef>.AllDefs
				where d.type == InfusionType.Suffix
				select d).ToList();
		}
	}
}
