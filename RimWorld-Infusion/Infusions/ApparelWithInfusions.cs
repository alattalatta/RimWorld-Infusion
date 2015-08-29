using Infusion;
using RimWorld;
using Verse;

namespace Infusion
{
    internal class ApparelWithInfusions : Apparel
    {
        public override string LabelBase
        {
            get
            {
                QualityCategory qc;
                if ( !this.TryGetQuality( out qc ) ||
                     qc < QualityCategory.Good ||
                     this.TryGetComp< CompInfusion >() == null ||
                     !this.TryGetComp< CompInfusion >().Infused )
                {
                    return base.LabelBase;
                }

                return this.GetInfusedLabel();
            }
        }
    }
}
