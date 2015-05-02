using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace Infusion
{
    public class CompInfusion : ThingComp
    {
		private bool isInfused;
		private InfusionPrefix prefix;
	    private InfusionSuffix suffix;

	    public Pair<InfusionPrefix, InfusionSuffix> Infusion
	    {
		    get
		    {
			    return new Pair<InfusionPrefix, InfusionSuffix>
			    {
				    First = prefix,
				    Second = suffix
			    };
		    }
	    }

		public bool SetInfusion()
		{
			bool passPrefix = false, passSuffix = false;
			QualityCategory qc;
			if (!parent.TryGetQuality(out qc) || qc <= QualityCategory.Normal)
			{
				prefix = InfusionPrefix.None;
				suffix = InfusionSuffix.None;
				return false;
			}

			/** PrefixTable
			 * Legendary	8 75
			 * Masterwork	7 58
			 * Excellent	6 41
			 * Superior		5 24
			 * Good			4 07
			 */
			float chance = (int)qc * 20 - 95;
			//Lower chance with ranged weapons
			if (parent.def.IsRangedWeapon)
				chance *= 0.75f;

			var rand1 = Rand.Range(0, 100);

			if (rand1 >= chance)
			{
				prefix = InfusionPrefix.None;
				passPrefix = true;
			}

			/**	SuffixTable
			 * Legendary	8 95
			 * Masterwork	7 72
			 * Excellent	6 49
			 * Superior		5 27
			 * Good			4 05
			 */
			chance = (int)qc * 23 - 89;
			//Lower chance with ranged weapons
			if (parent.def.IsRangedWeapon)
				chance *= 0.75f;

			rand1 = Rand.Range(0, 100);
			if (rand1 >= chance)
			{
				suffix = InfusionSuffix.None;
				passSuffix = true;
			}

			if (passPrefix && passSuffix)
				return false;

			/**		Table
			 * Tier 1		50
			 * Tier 2		38
			 * Tier 3		12
			 */
			int rand2;
			rand1 = Rand.Range(0, 100);
			if (rand1 >= 50)
			{
				rand1 = MathInfusion.Rand(InfusionPrefix.Tier1, InfusionPrefix.Tier2);
				rand2 = MathInfusion.Rand(InfusionSuffix.Tier1, InfusionSuffix.Tier2);
			}
			else if (rand1 >= 38)
			{
				rand1 = MathInfusion.Rand(InfusionPrefix.Tier2, InfusionPrefix.Tier3);
				rand2 = MathInfusion.Rand(InfusionSuffix.Tier2, InfusionSuffix.Tier3);
			}
			else
			{
				rand1 = MathInfusion.Rand(InfusionPrefix.Tier3, InfusionPrefix.End);
				rand2 = MathInfusion.Rand(InfusionSuffix.Tier3, InfusionSuffix.End);
			}

			if(!passPrefix)
				prefix = (InfusionPrefix) rand1;
			if(!passSuffix)
				suffix = (InfusionSuffix) rand2;

			//For added hit points
			parent.HitPoints = parent.MaxHitPoints;

			MoteThrower.ThrowText(parent.Position.ToVector3Shifted(), StaticSet.StringInfused, new Color(1f, 0.4f, 0f));
			return true;
		}
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.LookValue(ref isInfused, "isInfused", false);
			Scribe_Values.LookValue(ref prefix, "prefix", InfusionPrefix.None);
			Scribe_Values.LookValue(ref suffix, "suffix", InfusionSuffix.None);
		}
	    public override void PostSpawnSetup()
	    {
		    base.PostSpawnSetup();

			if(!isInfused)
				SetInfusion();

			isInfused = true;
			InfusionLabelManager.Register(this);
	    }

	    public override void PostDeSpawn()
	    {
		    base.PostDeSpawn();
			if(isInfused)
				InfusionLabelManager.DeRegister(this);
	    }

	    public override bool AllowStackWith(Thing other)
	    {
			//Same weapon, same stuff, same pre&suffix? Not possible
			//Weapons are not stackable anyway
		    return false;
	    }

		public override string CompInspectStringExtra()
		{
			if (prefix == InfusionPrefix.None && suffix == InfusionSuffix.None)
				return null;

			QualityCategory qc;
			parent.TryGetQuality(out qc);
			return StaticSet.StringInfusionFullName + ": " + parent.GetInfusedLabel() + " (" + qc.GetLabel() + ")";
		}

	    public override string GetDescriptionPart()
	    {
		    var prePass = prefix == InfusionPrefix.None;
		    var sufPass = suffix == InfusionSuffix.None;

		    if (prePass && sufPass)
			    return null;

			var result = new StringBuilder(null);
		    result.AppendLine(StaticSet.StringInfusionInfo.Translate(parent.GetInfusedLabel()));
		    result.AppendLine();

		    if (!prePass)
		    {
				result.Append(StaticSet.StringInfusionInfoPrefix.Translate(prefix.GetInfusionLabel()) + " ");
			    result.AppendLine(StaticSet.StringInfusionInfoPrefixBonus.Translate(prefix.GetInfusionDescription()));
			}
		    if (!prePass && !sufPass)
		    {
			    result.AppendLine();
			    result.Append(StaticSet.StringInfusionInfoPreSuffix.Translate(suffix.GetInfusionLabel()) + " ");
		    }
		    else if(!sufPass)
			    result.Append(StaticSet.StringInfusionInfoSuffix.Translate(suffix.GetInfusionLabel()) + " ");

		    if (!sufPass)
				result.AppendLine(StaticSet.StringInfusionInfoSuffixBonus.Translate(suffix.GetInfusionDescription()));

		    return base.GetDescriptionPart() + result;
	    }
    }
}
