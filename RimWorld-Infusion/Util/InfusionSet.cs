using System;
using Verse;

namespace Infusion
{
	public class InfusionSet : IEquatable<InfusionSet>, IExposable
	{
		public bool Equals(InfusionSet other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Prefix, other.Prefix) && string.Equals(Suffix, other.Suffix);
		}

		//As there was a problem with Scribe_Deep, I didn't have to use this interface.
		//No reference for now.
		public void ExposeData()
		{
			Scribe_Values.LookValue(ref Prefix, "Prefix", null);
			Scribe_Values.LookValue(ref Suffix, "Suffix", null);
		}

		public string Prefix;
		public string Suffix;

		public bool PassPre { get { return Prefix == null; } }
		public bool PassSuf { get { return Suffix == null; } }

		public InfusionSet(string pre, string suf)
		{
			Prefix = pre;
			Suffix = suf;
		}

		public static InfusionSet Empty
		{
			get
			{
				return new InfusionSet(null, null);
			}
		}
	}
}