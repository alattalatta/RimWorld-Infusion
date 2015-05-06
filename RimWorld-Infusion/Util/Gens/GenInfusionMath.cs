using UnityEngine;

namespace Infusion
{
	public static class GenInfusionMath
	{
		public static InfusionTier Max(InfusionTier a, InfusionTier b)
		{
			if (a == InfusionTier.Undefined) return b;
			if (b == InfusionTier.Undefined) return a;

			return a < b ? b : a;
		}

		public static float ToAbs(this float f)
		{
			return Mathf.Abs(f);
		}
	}
}
