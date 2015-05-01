using System.Collections.Generic;

namespace Infusion
{
	public static class InfusionLabelManager
	{
		public static List<CompInfusion> Drawee { get; private set; }

		static InfusionLabelManager()
		{
			Drawee = new List<CompInfusion>();
		}

		public static void ReInit()
		{
			Drawee.Clear();
		}
		public static void Register(CompInfusion compInfusion)
		{
			Drawee.Add(compInfusion);
		}

		public static void DeRegister(CompInfusion compInfusion)
		{
			Drawee.Remove(compInfusion);
		}
	}
}
