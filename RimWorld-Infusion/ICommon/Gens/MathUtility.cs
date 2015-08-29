using UnityEngine;

namespace Infusion
{
    public static class MathUtility
    {
        public static InfusionTier Max( InfusionTier a, InfusionTier b )
        {
            if ( a == InfusionTier.Undefined )
            {
                return b;
            }
            if ( b == InfusionTier.Undefined )
            {
                return a;
            }

            return a < b ? b : a;
        }

        public static float ToAbs( this float f )
        {
            return Mathf.Abs( f );
        }

        public static bool FloatEqual( this float a, float b )
        {
            return a - b < 0.00001f;
        }

        public static bool FloatNotEqual( this float a, float b )
        {
            return !a.FloatEqual( b );
        }
    }
}
