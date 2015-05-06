using System.Linq;
using System.Text;
using Infusion.Util;
using RimWorld;
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
		    get { return prefix != null || suffix != null; }
	    }

		public InfusionSet Infusions
		{
			get
			{
				return new InfusionSet(prefix, suffix);
			}
		}

		private bool tried;
	    private string prefix, suffix;
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

			prefix = null;
			suffix = null;
		}

		private static float GetChance(QualityCategory qc, InfusionType type)
		{
			switch (qc)
			{
					//									Pre : Suf
				case QualityCategory.Good:
					return type == InfusionType.Prefix ? 40 : 44;
				case QualityCategory.Superior:
					return type == InfusionType.Prefix ? 47 : 50;
				case QualityCategory.Excellent:
					return type == InfusionType.Prefix ? 58 : 59;
				case QualityCategory.Masterwork:
					return 77;
				case QualityCategory.Legendary:
					return type == InfusionType.Prefix ? 100 : 88;
			}
			return 0;
		}

	    private static InfusionTier GetTier(QualityCategory qc, InfusionType type)
	    {
		    var rand = Rand.Range(0, 100);
			//												   Pre : Suf
		    if (rand > (type == InfusionType.Prefix ? 50 + (int)qc : 45 + (int)qc))
			    return InfusionTier.Uncommon;
		    if (rand > (type == InfusionType.Prefix ? 22 + (int)qc : 15 + (int)qc))
			    return InfusionTier.Rare;
			if (rand > (type == InfusionType.Prefix ?  9		   : 5))
			    return InfusionTier.Epic;
			if (rand > (type == InfusionType.Prefix ?  2		   : 1))
				return InfusionTier.Legendary;
			return InfusionTier.Artifact;
	    }

	    private void GenerateInfusion(QualityCategory qc, bool shouldThrowMote)
		{
			//Chance based pass indicators
			bool passPre = false, passSuf = false;

		    var chance = GetChance(qc, InfusionType.Prefix);
			//Lower chance with ranged weapons : 91%
			if (parent.def.IsRangedWeapon)
				chance *= 0.91f;

			var rand = Rand.Range(0, 100);

			if (rand >= chance)
				passPre = true;

		    chance = GetChance(qc, InfusionType.Suffix);
			//Lower chance with ranged weapons : 85%
			if (parent.def.IsRangedWeapon)
				chance *= 0.85f;

			rand = Rand.Range(0, 100);
			if (rand >= chance)
				passSuf = true;

			if (passPre && passSuf)
				return;

			if (!passPre)
			{
				InfusionDef preTemp;
				var tier = GetTier(qc, InfusionType.Prefix);
				if (!(from t in DefDatabase<InfusionDef>.AllDefs.ToList()
					where 
						t.tier == tier &&
						t.type == InfusionType.Prefix //&&
						//(parent.def.IsApparel && t.canInfuseApparel || (parent.def.IsMeleeWeapon || parent.def.IsRangedWeapon) && t.canInfuseWeapons)
					select t).TryRandomElement(out preTemp))
				{
					Log.Error("Couldn't find any prefixed InfusionDef! Tier: " + tier);
					return;
				}
				prefix = preTemp.defName;
			}

			if (!passSuf)
			{
				InfusionDef preTemp;
				var tier = GetTier(qc, InfusionType.Suffix);
				if (!(from t in DefDatabase<InfusionDef>.AllDefs.ToList()
					  where
						 t.tier == tier &&
						 t.type == InfusionType.Suffix //&&
						 //(parent.def.IsApparel && t.canInfuseApparel || (parent.def.IsMeleeWeapon || parent.def.IsRangedWeapon) && t.canInfuseWeapons)
					 select t).TryRandomElement(out preTemp))
				{
					Log.Error("Couldn't find any suffixed InfusionDef! Tier: " + tier);
					return;
				}
				suffix = preTemp.defName;
			}

			//For added hit points
			parent.HitPoints = parent.MaxHitPoints;

		    if (!shouldThrowMote) return;

		    var msg = new StringBuilder();
		    msg.Append(qc.ToString().ToLower() + " ");
		    if (parent.Stuff != null)
			    msg.Append(parent.Stuff.LabelAsStuff + " ");
		    msg.Append(parent.def.label);
		    Messages.Message(StaticSet.StringInfusionMessage.Translate(msg));
		    InfusionSound.PlayOneShotOnCamera();
		    MoteThrower.ThrowText(parent.Position.ToVector3Shifted(), StaticSet.StringInfused, GenInfusionColor.Legendary);
		}
		/*
	    public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
	    {
		    List<FloatMenuOption> list = base.CompFloatMenuOptions(selPawn).ToList();
			list.Add(new FloatMenuOption("Try reinfusion...", () => Find.LayerStack.Add(new Dialog_Infusion(selPawn, parent))));

		    return list;
	    }
		 */

	    public override void PostSpawnSetup()
	    {
		    base.PostSpawnSetup();
		    SetInfusion(true);
		    tried = true;
		    if (Infused)
			    InfusionLabelManager.Register(this);
	    }

	    public override void PostExposeData()
	    {
		    base.PostExposeData();
			Scribe_Values.LookValue(ref tried, "tried", false);
			Scribe_Values.LookValue(ref prefix, "prefix", null);
			Scribe_Values.LookValue(ref suffix, "suffix", null);
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

		    return Infusions.Equals(otherSet);
	    }
		public override void PostSplitOff(Thing piece)
		{
			base.PostSplitOff(piece);
			piece.TryGetComp<CompInfusion>().tried = Tried;
			piece.TryGetComp<CompInfusion>().prefix = prefix;
			piece.TryGetComp<CompInfusion>().suffix = suffix;
		}

	    public override string GetDescriptionPart()
	    {
		    return base.GetDescriptionPart() + parent.GetInfusedDescription() + "\n" + parent.GetInfusedDescriptionITab();
	    }
    }
}
