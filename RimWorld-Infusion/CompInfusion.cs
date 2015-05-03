using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Infusion
{
    public class CompInfusion : ThingComp
    {
	    public bool IsTried
	    {
		    get { return isTried; }
	    }

	    public bool IsInfused
	    {
		    get { return prefix != InfusionPrefix.None || suffix != InfusionSuffix.None; }
	    }

	    private bool isTried;
		private InfusionPrefix prefix;
	    private InfusionSuffix suffix;
	    private static readonly SoundDef InfusionSound = SoundDef.Named("Infusion_Infused");

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

		public bool SetInfusion(bool shouldFireMote = false)
		{
			if (isTried)
				return false;
			var compQuality = parent.GetComp<CompQuality>();
			if (compQuality == null)
			{
				return false;
			}

			var qc = compQuality.Quality;
			if (qc > QualityCategory.Normal) return GenerateInfusion(qc, shouldFireMote);

			prefix = InfusionPrefix.None;
			suffix = InfusionSuffix.None;
			isTried = true;
			return false;
		}

	    private bool GenerateInfusion(QualityCategory qc, bool shouldFireMote)
		{
			bool passPrefix = false, passSuffix = false;

			/** PrefixTable
			 * Legendary	8 75
			 * Masterwork	7 58
			 * Excellent	6 41
			 * Superior		5 24
			 * Good			4 07
			 */
		    var chance = GetChance(qc, true);
			//Lower chance with ranged weapons
			if (parent.def.IsRangedWeapon)
				chance *= 0.88f;

			var rand = Rand.Range(0, 100);

			if (rand >= chance)
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
		    chance = GetChance(qc, false);
			//Lower chance with ranged weapons
			if (parent.def.IsRangedWeapon)
				chance *= 0.70f;

			rand = Rand.Range(0, 100);
			if (rand >= chance)
			{
				suffix = InfusionSuffix.None;
				passSuffix = true;
			}

			if (passPrefix && passSuffix)
			{
				isTried = true;
				return false;
			}

			if (!passPrefix)
			{
				/** PrefixTable
				 * Tier 1		45 - (qc - 4)
				 * Tier 2		32 - (qc - 4)
				 * Tier 3		23 + 2 * (qc - 4)
				 */
				rand = Rand.Range(0, 100);
				if (rand <= 23 + 2 * ((int)qc - 4))
					rand = MathInfusion.Rand(InfusionPrefix.Tier3, InfusionPrefix.End);
				else if (rand <= 32 - (int)qc + 4)
					rand = MathInfusion.Rand(InfusionPrefix.Tier2, InfusionPrefix.Tier3);
				else
					rand = MathInfusion.Rand(InfusionPrefix.Tier1, InfusionPrefix.Tier2);

				prefix = (InfusionPrefix)rand;
			}

			if (!passSuffix)
			{
				/** SuffixTable
				 * Tier 1		50 - (qc - 4)
				 * Tier 2		38 - (qc - 4)
				 * Tier 3		12 + 2 * (qc - 4)
				 */
				rand = Rand.Range(0, 100);
				if (rand <= 12 + 2 * ((int)qc - 4))
					rand = MathInfusion.Rand(InfusionSuffix.Tier3, InfusionSuffix.End);
				else if (rand <= 38 - (int)qc + 4)
					rand = MathInfusion.Rand(InfusionSuffix.Tier2, InfusionSuffix.Tier3);
				else
					rand = MathInfusion.Rand(InfusionSuffix.Tier1, InfusionSuffix.Tier2);

				suffix = (InfusionSuffix)rand;
			}

			//For added hit points
			parent.HitPoints = parent.MaxHitPoints;

		    if (shouldFireMote)
		    {
			    var msg = qc + " ";
			    if (parent.Stuff != null)
				    msg += parent.Stuff.LabelAsStuff + " ";
			    msg += parent.def.label;
				Messages.Message(StaticSet.StringInfusionMessage.Translate(msg));
				InfusionSound.PlayOneShotOnCamera();
				MoteThrower.ThrowText(parent.Position.ToVector3Shifted(), StaticSet.StringInfused, new Color(1f, 0.4f, 0f));
		    }
			isTried = true;
			return true;
	    }

	    public override void PostSpawnSetup()
	    {
		    base.PostSpawnSetup();
		    SetInfusion(true);

		    if (IsInfused)
			    InfusionLabelManager.Register(this);
	    }

	    public override void PostExposeData()
	    {
		    base.PostExposeData();
			Scribe_Values.LookValue(ref isTried, "isTried", true);
		    Scribe_Values.LookValue(ref prefix, "prefix", InfusionPrefix.None);
		    Scribe_Values.LookValue(ref suffix, "suffix", InfusionSuffix.None);
	    }

	    public override void PostDeSpawn()
	    {
		    base.PostDeSpawn();

			if(IsInfused)
				InfusionLabelManager.DeRegister(this);
	    }

	    public override bool AllowStackWith(Thing other)
	    {
		    if (other.TryGetComp<CompInfusion>() == null)
			    return false;

			InfusionSuffix infSuffix;
		    InfusionPrefix infPrefix;
		    other.TryGetInfusionPrefix(out infPrefix);
		    other.TryGetInfusionSuffix(out infSuffix);

		    var flag = infPrefix == prefix && infSuffix == suffix;

		    return flag;
	    }
		public override void PostSplitOff(Thing piece)
		{
			base.PostSplitOff(piece);
			piece.TryGetComp<CompInfusion>().isTried = IsTried;
			piece.TryGetComp<CompInfusion>().prefix = prefix;
			piece.TryGetComp<CompInfusion>().suffix = suffix;
		}

	    public override string GetDescriptionPart()
	    {
		    var prePass = prefix == InfusionPrefix.None;
		    var sufPass = suffix == InfusionSuffix.None;

		    if (prePass && sufPass)
			    return null;

			var result = new StringBuilder(null);
		    result.AppendLine(StaticSet.StringInfusionInfo.Translate(parent.GetInfusedLabel(true, false)));
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

	    private static float GetChance(QualityCategory qc, bool isPrefix)
	    {
			return isPrefix ? 
				(int)qc * 20 - 95 : (int)qc * 23 - 89;
	    }
    }
}
