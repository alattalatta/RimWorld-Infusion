using UnityEngine;
using Verse;

namespace Infusion
{
    public class ModInitializer : ITab
    {
        public ModInitializer()
        {
            var initter = GameObject.Find( "IN_ModInitter" );
            if ( initter != null )
            {
                return;
            }

            initter = new GameObject( "IN_ModInitter" );
            initter.AddComponent< ModInitComponent >();
            Object.DontDestroyOnLoad( initter );
        }

        protected override void FillTab()
        {
        }
    }
}