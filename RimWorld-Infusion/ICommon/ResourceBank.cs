using System.Linq;
using RimWorld;
using Verse;

namespace Infusion
{
    public static class ResourceBank
    {
        public static bool FindModActive( string modName )
        {
            return LoadedModManager.LoadedMods.ToList().Exists( s => s.name == modName );
        }

        public static string QualityAsTranslated( QualityCategory qc )
        {
            return ("QualityCategory_" + qc).Translate();
        }

        //Mote
        public static string StringInfused = "Infused".Translate();
        
        //Your weapon, {0}, is infused!
        public static string StringInfusionMessage = "InfusionMessage";
        
        //{1: golden sword} of {2: stream}
        public static string StringInfusionOf = "InfusionOf";
        
        //Infusion bonuses
        public static string StringInfusionDescBonus = "InfusionDescBonus".Translate();
        public static string StringInfusionDescFrom = "InfusionDescFrom";
        
        public static string StringQuality = "Quality".Translate();


        public static string StringThisApparel = "ThisApparel".Translate();
        public static string StringThisWeapon = "ThisWeapon".Translate();
    }
}
