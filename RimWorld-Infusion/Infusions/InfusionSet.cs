using System;

namespace Infusion
{
	public class InfusionSet : IEquatable<InfusionSet>//, IExposable
	{
		public bool Equals(InfusionSet other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Prefix, other.Prefix) && string.Equals(Suffix, other.Suffix);
		}

		public string Prefix;
		public string Suffix;

		public bool PassPre => Prefix == null;
		public bool PassSuf => Suffix == null;

		public InfusionSet(string pre, string suf)
		{
			Prefix = pre;
			Suffix = suf;
		}

		public static InfusionSet Empty => new InfusionSet(null, null);
	}
}