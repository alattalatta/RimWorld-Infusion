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

		public bool Tried;
	    private string prefix, suffix;
	    private static readonly SoundDef InfusionSound = SoundDef.Named("Infusion_Infused");


		public void SetInfusion(bool _shouldFireMote = false)
		{
			if (Tried)
				return;
			var compQuality = parent.GetComp<CompQuality>();
			if (compQuality == null)
				return;

			var qc = compQuality.Quality;
			if (qc > QualityCategory.Normal)
			{
				GenerateInfusion(qc, _shouldFireMote);
				return;
			}

			prefix = null;
			suffix = null;
		}

		private static float GetChance(QualityCategory _qc, InfusionType _type)
		{
			switch (_qc)
			{
					//									Pre : Suf
				case QualityCategory.Good:
					return _type == InfusionType.Prefix ? 40 : 44;
				case QualityCategory.Superior:
					return _type == InfusionType.Prefix ? 47 : 50;
				case QualityCategory.Excellent:
					return _type == InfusionType.Prefix ? 58 : 59;
				case QualityCategory.Masterwork:
					return 77;
				case QualityCategory.Legendary:
					return _type == InfusionType.Prefix ? 100 : 88;
			}
			return 0;
		}

	    private static InfusionTier GetTier(QualityCategory _qc, InfusionType _type)
	    {
		    var rand = Rand.Range(0, 100);
			//												   Pre : Suf
		    if (rand > (_type == InfusionType.Prefix ? 48 + (int)_qc : 43 + (int)_qc))
			    return InfusionTier.Uncommon;
		    if (rand > (_type == InfusionType.Prefix ? 19 + (int)_qc : 13 + (int)_qc))
			    return InfusionTier.Rare;
			if (rand > (_type == InfusionType.Prefix ?  6		   : 4))
			    return InfusionTier.Epic;
			if (rand > (_type == InfusionType.Prefix ?  2		   : 1))
				return InfusionTier.Legendary;
			return InfusionTier.Artifact;
	    }

	    private void GenerateInfusion(QualityCategory _qc, bool _shouldThrowMote)
		{
			//Chance based pass indicators
			bool passPre = false, passSuf = false;

		    var chance = GetChance(_qc, InfusionType.Prefix);
			//Lower chance with ranged weapons : 91%
			if (parent.def.IsRangedWeapon)
				chance *= 0.91f;

			var rand = Rand.Range(0, 100);

			if (rand >= chance)
				passPre = true;

		    chance = GetChance(_qc, InfusionType.Suffix);
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
				var tier = GetTier(_qc, InfusionType.Prefix);
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
				var tier = GetTier(_qc, InfusionType.Suffix);
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

		    if (!_shouldThrowMote) return;

		    var msg = new StringBuilder();
		    msg.Append(_qc.ToString().ToLower() + " ");
		    if (parent.Stuff != null)
			    msg.Append(parent.Stuff.LabelAsStuff + " ");
		    msg.Append(parent.def.label);
		    Messages.Message(StaticSet.StringInfusionMessage.Translate(msg), MessageSound.Silent);
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
		    Tried = true;
		    if (Infused)
			    InfusionLabelManager.Register(this);
	    }

	    public override void PostExposeData()
	    {
		    base.PostExposeData();

			//Not using Scribe_Deep with a InfusionSet; instead, using individual strings: prefix, suffix.
			//See InfusionSet's ExposeDate() for a reason.
			Scribe_Values.LookValue(ref Tried, "tried", false);
			Scribe_Values.LookValue(ref prefix, "prefix", null);
			Scribe_Values.LookValue(ref suffix, "suffix", null);
	    }

	    public override void PostDeSpawn()
	    {
		    base.PostDeSpawn();

			if(Infused)
				InfusionLabelManager.DeRegister(this);
	    }

	    public override bool AllowStackWith(Thing _other)
	    {
		    if (_other.TryGetComp<CompInfusion>() == null)
			    return false;

		    InfusionSet otherSet;
		    _other.TryGetInfusions(out otherSet);

		    return Infusions.Equals(otherSet);
	    }
		public override void PostSplitOff(Thing _piece)
		{
			base.PostSplitOff(_piece);
			_piece.TryGetComp<CompInfusion>().Tried = Tried;
			_piece.TryGetComp<CompInfusion>().prefix = prefix;
			_piece.TryGetComp<CompInfusion>().suffix = suffix;
		}

	    public override string GetDescriptionPart()
	    {
		    return base.GetDescriptionPart() + parent.GetInfusedDescription() + "\n" + parent.GetInfusedDescriptionITab();
	    }
    }
}
