using System.Linq;
using RimWorld;
using Verse;

namespace Infusion
{
    public static class ResourceBank
    {
        public static string QualityAsTranslated( QualityCategory qc )
        {
            return ("QualityCategory_" + qc).Translate();
        }

        //Mote
        public static readonly string StringInfused = "Infused".Translate();
        
        //Your weapon, {0}, is infused!
        public static readonly string StringInfusionMessage = "InfusionMessage";
        
        //{1: golden sword} of {2: stream}
        public static readonly string StringInfusionOf = "InfusionOf";
        
        //Infusion bonuses
        public static readonly string StringInfusionDescBonus = "InfusionDescBonus".Translate();
        public static readonly string StringInfusionDescFrom = "InfusionDescFrom";
        
        public static readonly string StringQuality = "Quality".Translate();


        public static readonly string StringThisApparel = "ThisApparel".Translate();
        public static readonly string StringThisWeapon = "ThisWeapon".Translate();
    }
}
