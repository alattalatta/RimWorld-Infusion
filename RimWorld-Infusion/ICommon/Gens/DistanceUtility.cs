using System;
using System.Collections.Generic;
using Verse;
// ReSharper disable MemberCanBePrivate.Global

namespace Infusion
{
	public static class DistanceUtility
	{
		public static double DistanceTo(this IntVec3 a, IntVec3 b)
		{
			var x = Math.Abs(a.x - b.x);
			var y = Math.Abs(a.y - b.y);
			var z = Math.Abs(a.z - b.z);

			return Math.Sqrt(x*x + y*y + z*z);
		}
		public static Thing FindNearestThing(IEnumerable<Thing> things, IntVec3 pos)
		{
			return things == null ? null : FindNearestThing(new List<Thing>(things), pos);
		}

		public static Thing FindNearestThing(List<Thing> things, IntVec3 pos)
		{
			var nearestDistance = 99999.0d;
			Thing foundThing = null;

			if (things == null)
				return null;

			//foreach (Thing t in things)
			foreach (var current in things)
			{
				var dist = current.Position.DistanceTo(pos);
				if (!(dist < nearestDistance)) continue;

				nearestDistance = dist;
				foundThing = current;
			}

			return foundThing;
		}
	}
}
