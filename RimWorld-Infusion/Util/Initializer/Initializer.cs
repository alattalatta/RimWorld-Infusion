using UnityEngine;
using Verse;

namespace Infusion
{
	public class Initializer : ITab
	{

		public Initializer()
		{
			var initter = GameObject.Find("IN_ModInitter");
			if (initter != null) return;

			initter = new GameObject("IN_ModInitter");
			initter.AddComponent<InitComponent>();
			Object.DontDestroyOnLoad(initter);
		}
		protected override void FillTab()
		{
		}
	}
}