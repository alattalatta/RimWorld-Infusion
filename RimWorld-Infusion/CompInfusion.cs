using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Infusion
{
    public class CompInfusion : ThingComp
    {
	    public bool Tried
	    {
		    get { return tried; }
	    }

	    public bool Infused
	    {
		    get { return !infusions.Equals(InfusionSet.Empty); }
	    }

		public InfusionSet Infusions
		{
			get
			{
				return infusions;
			}
		}

		private bool tried;
		private InfusionSet infusions = new InfusionSet(null, null);
	    private static readonly SoundDef InfusionSound = SoundDef.Named("Infusion_Infused");


		public void SetInfusion(bool shouldFireMote = false)
		{
			if (tried)
				return;
			var compQuality = parent.GetComp<CompQuality>();
			if (compQuality == null)
				return;

			var qc = compQuality.Quality;
			if (qc > QualityCategory.Normal)
			{
				GenerateInfusion(qc, shouldFireMote);
				return;
			}

			infusions = InfusionSet.Empty;
			tried = true;
		}

	    private void GenerateInfusion(QualityCategory qc, bool shouldThrowMote)
		{
			//Chance based pass indicators
			bool passPrefix = false, passSuffix = false;

			/** PrefixTable
			 * Legendary	8 75
			 * Masterwork	7 58
			 * Excellent	6 41
			 * Superior		5 24
			 * Good			4 07
			 */
		    var chance = GetChance(qc, true);
			//Lower chance with ranged weapons : 88%
			if (parent.def.IsRangedWeapon)
				chance *= 0.88f;

			var rand = Rand.Range(0, 100);

			if (rand >= chance)
				passPrefix = true;

			/**	SuffixTable
			 * Legendary	8 95
			 * Masterwork	7 72
			 * Excellent	6 49
			 * Superior		5 27
			 * Good			4 05
			 */
		    chance = GetChance(qc, false);
			//Lower chance with ranged weapons : 70%
			if (parent.def.IsRangedWeapon)
				chance *= 0.70f;

			rand = Rand.Range(0, 100);
			if (rand >= chance)
				passSuffix = true;

			if (passPrefix && passSuffix)
				return;

		    InfusionTier infTier;
			if (!passPrefix)
			{
				/** PrefixTable
				 * Tier 1		45 - (qc - 4)
				 * Tier 2		32 - (qc - 4)
				 * Tier 3		23 + 2 * (qc - 4)
				 */
				rand = Rand.Range(0, 100);
				if (rand <= 23 + 2*((int) qc - 4))
					infTier = InfusionTier.Tier3;
				else if (rand <= 32 - (int) qc + 4)
					infTier = InfusionTier.Tier2;
				else
					infTier = InfusionTier.Tier1;

				InfusionDef preTemp;
				var tier = infTier;
				if (!(from t in DefDatabase<InfusionDef>.AllDefs.ToList()
					where 
						t.tier == tier &&
						t.type == InfusionType.Prefix //&&
						//(parent.def.IsApparel && t.canInfuseApparel || (parent.def.IsMeleeWeapon || parent.def.IsRangedWeapon) && t.canInfuseWeapons)
					select t).TryRandomElement(out preTemp))
				{
					Log.Error("Couldn't find any prefix InfusionDef!");
					return;
				}
				infusions.Prefix = preTemp.defName;
			}

			if (!passSuffix)
			{
				/** SuffixTable
				 * Tier 1		50 - (qc - 4)
				 * Tier 2		38 - (qc - 4)
				 * Tier 3		12 + 2 * (qc - 4)
				 */
				rand = Rand.Range(0, 100);
				if (rand <= 23 + 2 * ((int)qc - 4))
					infTier = InfusionTier.Tier3;
				else if (rand <= 32 - (int)qc + 4)
					infTier = InfusionTier.Tier2;
				else
					infTier = InfusionTier.Tier1;

				InfusionDef preTemp;
				var tier = infTier;
				if (!(from t in DefDatabase<InfusionDef>.AllDefs.ToList()
					  where
						 t.tier == tier &&
						 t.type == InfusionType.Suffix //&&
						 //(parent.def.IsApparel && t.canInfuseApparel || (parent.def.IsMeleeWeapon || parent.def.IsRangedWeapon) && t.canInfuseWeapons)
					 select t).TryRandomElement(out preTemp))
				{
					Log.Error("Couldn't find any suffix InfusionDef!");
					return;
				}
				infusions.Suffix = preTemp.defName;
			}

			//For added hit points
			parent.HitPoints = parent.MaxHitPoints;

		    if (shouldThrowMote)
		    {
			    var msg = new StringBuilder();
			    msg.Append(qc.ToString().ToLower() + " ");
			    if (parent.Stuff != null)
				    msg.Append(parent.Stuff.LabelAsStuff + " ");
			    msg.Append(parent.def.label);
				Messages.Message(StaticSet.StringInfusionMessage.Translate(msg));
				InfusionSound.PlayOneShotOnCamera();
				MoteThrower.ThrowText(parent.Position.ToVector3Shifted(), StaticSet.StringInfused, StaticSet.ColorTier2);
		    }
			tried = true;
		}

	    public override void PostSpawnSetup()
	    {
		    base.PostSpawnSetup();
		    SetInfusion(true);

		    if (Infused)
			    InfusionLabelManager.Register(this);
	    }

	    public override void PostExposeData()
	    {
		    base.PostExposeData();
			Scribe_Values.LookValue(ref tried, "tried", true);
		    Scribe_Values.LookValue(ref infusions, "infusions", InfusionSet.Empty);
	    }

	    public override void PostDeSpawn()
	    {
		    base.PostDeSpawn();

			if(Infused)
				InfusionLabelManager.DeRegister(this);
	    }

	    public override bool AllowStackWith(Thing other)
	    {
		    if (other.TryGetComp<CompInfusion>() == null)
			    return false;

		    InfusionSet otherSet;
		    other.TryGetInfusions(out otherSet);

		    return infusions.Equals(otherSet);
	    }
		public override void PostSplitOff(Thing piece)
		{
			base.PostSplitOff(piece);
			piece.TryGetComp<CompInfusion>().tried = Tried;
			piece.TryGetComp<CompInfusion>().infusions = Infusions;
		}

	    public override string GetDescriptionPart()
	    {
		    return base.GetDescriptionPart() + parent.GetInfusedDescription();
	    }

	    private static float GetChance(QualityCategory qc, bool isPrefix)
	    {
			return isPrefix ? 
				(int)qc * 20 - 95 : (int)qc * 23 - 89;
	    }
    }
}
