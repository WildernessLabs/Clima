using System;
using Meadow;

namespace WilderenessLabs.Clima.Meadow
{
	public partial class Anemometer
	{
		public class AnemometerChangeResult : INumericChangeResult<float>
		{
			public float New { get; set; }
			public float Old { get; set; }

			public float DeltaPercent => (Old > 0) ? Delta / Old : 0f;

			public float Delta => New - Old;
			//public float Delta {
			//	get {
			//		Console.WriteLine($"n:{New},o:{Old}");
			//		float delta = New - Old;
			//		Console.WriteLine($"delta: {delta}");
			//		return delta;
			//	}
			//}

			public AnemometerChangeResult()
			{
			}
		}
	}
}
