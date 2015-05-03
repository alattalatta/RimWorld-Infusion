namespace Infusion
{
	public enum InfusionPrefix
	{
		None,			//0 None

		Tier1,			//= Hack
		Lightweight,	// Attack--, Cooldown+, MoveSpeed+
		Heavyweight,	// Attack+, Cooldown-, Hitpoints+, MoveSpeed-
		Hardened,		// Hitpoints+, DeteriorationRate+

		Tier2,			//= Hack
		Hot,			// ComfyTemperatureMin++, ComfyTemperatureMax-
		Cold,			// ComfyTemperatureMax++, ComfyTemperatureMin-
		Compressed,		// Cooldown++
		Targeting,		// Accuracy+
		Intimidating,	// Attack+, Social-
		Decorated,		// Social+
		Slaughterous,	// ButcherySpeed+, ButcheryEfficiency+
		Alcoholic,		// BrewingSpeed++, Accuracy-

		Tier3,			//= Hack
		Telescoping,	// Accuracy++, Cooldown++
		Mechanized,		// Attack+, Accuracy+, ConstructionSpeed+
		Pneumatic,		// Attack+, Cooldown+, MiningSpeed+, StonecuttingSpeed+
		Charged,		// Attack++
		Antiviral,		// ImmunityGainSpeed++
		Holographic,	// Social+++, Attack---, Accuracy-
		Gravitational,	// MoveSpeed++
		Contaminated,	// ImmunityGainSpeed--

		End				//= Hack
	}

	public enum InfusionSuffix
	{
		None,			// None

		Tier1,			//= Hack
		Shock,			// Cooldown+
		Impact,			// Attack+
		Needle,			// Attack-, Accuracy+, Cooldown+
		Charisma,		// Social+

		Tier2,			//= Hack
		Forest,			// Cooldown+, PlantWorkSpeed++
		Rock,			// MentalBreakThreshold++, PsychicSensitivity+, Cooldown-, Social--, WorkSpeedGlobal-
		Creation,		// WorkSpeedGlobal+
		Stream,			// Attack-, Cooldown+++, Accuracy-
		Salt,			// CookSpeed+, FoodPoisonChance+, ComfyTemperatureMin/Max+
		Art,			// SculptingSpeed++, Attack+
		
		Tier3,			//= Hack
		Sunlight,		// Attack++, Accuracy++, ImmunityGainSpeed+
		Starlight,		// Accuracy+, Cooldown+++
		Pain,			// Attack++++, PsychicSensitivity--, MentalBreakThreshold-, ImmunityGainSpeed--
		Automaton,		// WorkSpeedGlobal++
		Dismantle,		// ButcheryMechanoidSpeed++, ButcheryMechanoidEfficiency+
		Exhaust,		// MoveSpeed--

		End				//= Hack
	}
}
