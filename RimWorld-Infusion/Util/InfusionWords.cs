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
		Forest,			// MeleeCooldown+, PlantWorkSpeed++
		Rock,			// MentalBreakThreshold++, PsychicSensitivity+, MeleeCooldown-, Social--, WorkSpeedGlobal-
		Creation,		// WorkSpeedGlobal+
		Stream,			// Attack-, MeleeCooldown+++, MeleeHitChance-
		Salt,			// CookSpeed+, FoodPoisonChance+, ComfyTemperatureMin/Max+
		
		Tier3,			//= Hack
		Sunlight,		// Attack++, MeleeHitChance++, ImmunityGainSpeed+
		Starlight,		// MeleeHitChance+, MeleeCooldown+++
		Pain,			// Attack++++, PsychicSensitivity--, MentalBreakThreshold-, ImmunityGainSpeed--
		Automaton,		// WorkSpeedGlobal++
		Disassembler,	// ButcheryMechanoidSpeed++, ButcheryMechanoidEfficiency+

		End				//= Hack
	}

	public enum InfusionPrefix
	{
		None,			//0 None

		Tier1,			//= Hack
		Lightweight,	// Attack-, MeleeCooldown+
		Heavyweight,	// Attack+, MeleeCooldown-
		Hardened,

		Tier2,			//= Hack
		Hot,			// ComfyTemperatureMin++, ComfyTemperatureMax-
		Cold,			// ComfyTemperatureMax++, ComfyTemperatureMin-
		Compressed,		// MeleeCooldown++
		Targeting,		// MeleeHitChance+
		Intimidating,	// Attack+, Social-
		Decorated,		// Social+
		Slaughterous,	// ButcherySpeed+, ButcheryEfficiency+
		Alcoholic,		// BrewingSpeed++, MeleeHitChance-

		Tier3,			//= Hack
		Telescoping,	// MeleeHitChance++, MeleeCooldown++
		Mechanized,		// Attack+, MeleeHitChance+, ConstructionSpeed+
		Pneumatic,		// Attack+, MeleeCooldown+, MiningSpeed+, StonecuttingSpeed+
		Charged,		// Attack++
		Antiviral,		// ImmunityGainSpeed++
		Holographic,	// Social+++, Attack--, MeleeHitChance-
		Contaminated,	// ImmunityGainSpeed--

		End				//= Hack
	}
}
