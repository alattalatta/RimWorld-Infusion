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
			//															Pre : Suf
		    if (rand > (type == InfusionType.Prefix ? 53 + (qc.Int() - 4)*4 : 60 + (qc.Int() - 4)*4))
			    return InfusionTier.Uncommon;
		    if (rand > (type == InfusionType.Prefix ? 27 + (qc.Int() - 4)*2 : 35 - (qc.Int() - 4)*2))
			    return InfusionTier.Rare;
			if (rand > (type == InfusionType.Prefix ?					 13 : 21))
			    return InfusionTier.Epic;
		    // ReSharper disable once ConvertIfStatementToReturnStatement
			if (rand > (type == InfusionType.Prefix ?  4 - (qc.Int() - 4)*2	: 9 + (qc.Int() - 4)*2))
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

		    InfusionTier infTier;
			if (!passPre)
			{
				infTier = GetTier(qc, InfusionType.Prefix);

				InfusionDef preTemp;
				var tier = infTier;
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
				infusions.Prefix = preTemp.defName;
			}

			if (!passSuf)
			{
				infTier = GetTier(qc, InfusionType.Suffix);

				InfusionDef preTemp;
				var tier = infTier;
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
				MoteThrower.ThrowText(parent.Position.ToVector3Shifted(), StaticSet.StringInfused, GenInfusionColor.Legendary);
		    }
			tried = true;
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
    }
}
