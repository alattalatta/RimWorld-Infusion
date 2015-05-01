using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace Infusion
{
    public class CompInfusion : ThingComp
    {
	    private bool isInfused;
	    private InfusionSuffix infusion;

	    public InfusionSuffix Infusion
	    {
		    get { return infusion; }
	    }

		public void SetInfusion()
		{
			QualityCategory qc;
			if (!parent.TryGetQuality(out qc) || qc <= QualityCategory.Good)
			{
				infusion = InfusionSuffix.None;
				return;
			}

			/**		Table
			 * Legendary	80
			 * Masterwork	65
			 * Excellent	50
			 * Superior		35
			 */
			var chance = (int)qc * 15 - 40;
			var rand = Rand.Range(0, 100);
			if (rand >= chance)
			{
				infusion = InfusionSuffix.None;
				return;
			}

			/**		Table
			 * Tier 1		65
			 * Tier 2		35
			 * Tier 3		10
			 */
			rand = Rand.Range(0, 100);
			if (rand >= 35)
			{
				rand = MathInfusion.Rand(InfusionSuffix.Tier1, InfusionSuffix.Tier2);
			}
			else if (rand >= 10)
			{
				rand = MathInfusion.Rand(InfusionSuffix.Tier2, InfusionSuffix.Tier3);
			}
			else
			{
				rand = MathInfusion.Rand(InfusionSuffix.Tier3, InfusionSuffix.End);
			}

			infusion = (InfusionSuffix)rand;
			isInfused = true;

			//For added hit points
			parent.HitPoints = parent.MaxHitPoints;

			MoteThrower.ThrowText(parent.Position.ToVector3Shifted(), "Infused!", new Color(1f, 0.4f, 0f));
		}
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.LookValue(ref isInfused, "isInfused", false);
			Scribe_Values.LookValue(ref infusion, "infusion", InfusionSuffix.None);
		}
	    public override void PostSpawnSetup()
	    {
		    base.PostSpawnSetup();

			if(!isInfused)
				SetInfusion();

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

			InfusionSuffix infSuffix;
			other.TryGetInfusion(out infSuffix);
			return infusion == infSuffix;
		}

		public override string CompInspectStringExtra()
		{
			return Infusion == InfusionSuffix.None ? null : "Full name: " + parent.GetInfusedLabel();
		}

	    public override string GetDescriptionPart()
	    {
		    if (Infusion == InfusionSuffix.None)
			    return null;
			var result = new StringBuilder();
		    result.AppendLine("This weapon is infused with a power of " + Infusion.GetInfusionLabel() + ".");
		    result.AppendLine(Infusion.GetInfusionDescription());
		    return base.GetDescriptionPart() + result;
	    }
    }
}
