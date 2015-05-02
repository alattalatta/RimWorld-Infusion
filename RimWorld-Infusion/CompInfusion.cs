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

		public void SetInfusion()
		{
			bool passPrefix = false, passSuffix = false;
			QualityCategory qc;
			if (!parent.TryGetQuality(out qc) || qc <= QualityCategory.Normal)
			{
				prefix = InfusionPrefix.None;
				suffix = InfusionSuffix.None;
				return;
			}

			/** PrefixTable
			 * Legendary	8 75
			 * Masterwork	7 58
			 * Excellent	6 41
			 * Superior		5 24
			 * Good			4 07
			 */
			var chance = (int)qc * 20 - 95;
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
			chance = (int) qc*23 - 89;
			rand1 = Rand.Range(0, 100);
			if (rand1 >= chance)
			{
				suffix = InfusionSuffix.None;
				passSuffix = true;
			}

			if (passPrefix && passSuffix)
				return;

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
			else if (rand1 >= 10)
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

			MoteThrower.ThrowText(parent.Position.ToVector3Shifted(), "Infused!", new Color(1f, 0.4f, 0f));
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
			var compInfusion = other.TryGetComp<CompInfusion>();
			if (compInfusion == null)
				return false;

		    InfusionPrefix infPrefix;
		    other.TryGetInfusionPrefix(out infPrefix);
			InfusionSuffix infSuffix;
			other.TryGetInfusionSuffix(out infSuffix);
			return prefix == infPrefix && suffix == infSuffix;
		}

		public override string CompInspectStringExtra()
		{
			if (prefix == InfusionPrefix.None && suffix == InfusionSuffix.None)
				return null;

			QualityCategory qc;
			parent.TryGetQuality(out qc);
			return "Full name: " + parent.GetInfusedLabel() + " (" + qc.GetLabel() + ")";
		}

	    public override string GetDescriptionPart()
	    {
		    var prePass = prefix == InfusionPrefix.None;
		    var sufPass = suffix == InfusionSuffix.None;

		    if (prePass && sufPass)
			    return null;

			var result = new StringBuilder(null);
		    result.AppendLine("This specific weapon has more potential than others.");
		    result.AppendLine("Your colonists had named it " + parent.GetInfusedLabel());
		    result.AppendLine();

		    if (!prePass)
		    {
			    result.Append("This weapon is " + prefix.GetInfusionLabel() + ". ");
			    result.AppendLine("It will grant user " + prefix.GetInfusionDescription());
			}
		    if (!prePass && !sufPass)
		    {
			    result.AppendLine();
			    result.Append("Also, it is infused with power of ");
		    }
		    else if(!sufPass)
			    result.Append("This weapon has power of ");

		    if (!sufPass)
		    {
			    result.Append(suffix.GetInfusionLabel() + ". ");
				result.AppendLine("It will add " + suffix.GetInfusionDescription());
		    }

		    return base.GetDescriptionPart() + result;
	    }
    }
}
