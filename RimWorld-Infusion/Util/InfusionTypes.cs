namespace Infusion
{
	public enum InfusionTypes
	{
		None,		//0 None

		Tier1,		//= Hack
		Shock,		//1 Attack+, MeleeCooldown+
		Impact,		//2 Attack++
		Needle,		//3 Attack-, MeleeHitChance++
		Charisma,	//4 Social+

		Tier2,		//= Hack
		Fire,		//5 Attack+, ComfyTemperatureMin+, ComfyTemperatureMax-
		Water,		//6 Attack-, ComfyTemeratureMin/Max+
		Plain,		//7 MeleeCooldown+, PlantWorkSpeed++
		Rock,		//8 MentalBreakThreshold+, PsychicSensitivity+, MeleeCooldown-, Social--, WorkSpeedGlobal-
		Creation,	//9 GlobalWorkSpeed+
		Stream,		//10 MeleeCooldown+++, MeleeHitChance--
		
		Tier3,		//= Hack
		Sunlight,	//11 Attack++, MeleeHitChance++
		Starlight,	//12 MeleeHitChance+, MeleeCooldown++
		Pain,		//13 Attack++++, PsychicSensitivity--, MentalBreakThreshold-, ImmunityGainSpeed--

		End			//= Hack
	}
}
