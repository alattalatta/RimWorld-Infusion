using RimWorld;
using UnityEngine;

namespace Infusion.Util
{
	public static class GenInfusionColor
	{
		public static readonly Color Uncommon = new Color(0.12f, 1, 0);
		public static readonly Color Rare = new Color(0, 0.44f, 1);
		public static readonly Color Epic = new Color(0.64f, 0.21f, 0.93f);
		public static readonly Color Legendary = new Color(1, 0.5f, 0);
		public static readonly Color Artifact = new Color(0.92f, 0.84f, 0.56f);

		public static Color QualityColor(this QualityCategory qc)
		{
			switch ((int)qc)
			{
				case 4:
					return Uncommon;
				case 5:
					return Rare;
				case 6:
					return Epic;
				case 7:
					return Legendary;
				case 8:
					return Artifact;
				default:
					return Color.white;
			}
		}

		public static Color InfusionColor(this InfusionTier it)
		{
			switch (it)
			{
				case InfusionTier.Uncommon:
					return Uncommon;
				case InfusionTier.Rare:
					return Rare;
				case InfusionTier.Epic:
					return Epic;
				case InfusionTier.Legendary:
					return Legendary;
				case InfusionTier.Artifact:
					return Artifact;
				default:
					return Color.white;
			}
		}
	}
}
