namespace Infusion
{
	public enum InfusionSuffix
	{
		None,			// None

		Tier1,			//= Hack
		Shock,			// MeleeCooldown+
		Impact,			// Attack+
		Needle,			// Attack-, MeleeHitChance+, MeleeCooldown+
		Charisma,		// Social+

		Tier2,			//= Hack
		Plain,			// MeleeCooldown+, PlantWorkSpeed++
		Rock,			// MentalBreakThreshold++, PsychicSensitivity+, MeleeCooldown-, Social--, WorkSpeedGlobal-
		Creation,		// WorkSpeedGlobal+
		Stream,			// Attack-, MeleeCooldown+++, MeleeHitChance-
		Salt,			// CookSpeed+, FoodPoisonChance+, ComfyTemperatureMin/Max+
		
		Tier3,			//= Hack
		Sunlight,		// Attack++, MeleeHitChance++
		Starlight,		// MeleeHitChance+, MeleeCooldown+++
		Pain,			// Attack++++, PsychicSensitivity--, MentalBreakThreshold-, ImmunityGainSpeed--
		Automaton,		// WorkSpeedGlobal++
		Disassembling,	// ButcheryMechanoidSpeed++, ButcheryMechanoidEfficiency+

		End				//= Hack
	}

	public enum InfusionPrefix
	{
		None,			//0 None

		Tier1,			//= Hack
		Lightweight,	// MeleeCooldown+
		Heavyweight,	// Attack+, MeleeCooldown-

		Tier2,			//= Hack
		Hot,			// Attack+, ComfyTemperatureMin++, ComfyTemperatureMax-
		Cold,			// Attack-, ComfyTemeratureMin/Max++
		Targeting,		// MeleeHitChance+
		Intimidating,	// Damage+, Social-
		Decorated,		// Social+
		Hardened,		// Attack+
		Butchering,		// ButcherySpeed+, ButcheryEfficiency+
		Alcoholic,		// BrewingSpeed++, MeleeHitChance-

		Tier3,			//= Hack
		Telescoping,	// MeleeHitChance++, MeleeCooldown++
		Mechanized,		// Attack+, MeleeCooldown+++, MeleeHitChance--
		Compressed,		// MeleeCooldown++
		Charged,		// Attack++
		Antiviral,		// ImmunityGainSpeed++
		Holographic,	// Social+++, Attack--, MeleeHitChance-
		Contaminated,	// ImmunityGainSpeed--

		End				//= Hack
	}
}
