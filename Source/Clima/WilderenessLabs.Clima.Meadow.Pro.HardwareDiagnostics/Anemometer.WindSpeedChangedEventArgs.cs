using System;
using Meadow;

namespace WilderenessLabs.Clima.Meadow
{
	public partial class Anemometer
	{
		public class AnemometerChangeResult : INumericChangeResult<float>
		{
			public float New {
				get => this.newValue;
				set => this.newValue = value;
			}
			protected float newValue = 0f;
			public float Old {
				get => this.oldValue;
				set => this.oldValue = value;
			}
			protected float oldValue = 0f;

			public float DeltaPercent => (oldValue > 0) ? Delta / oldValue : 0f;

			public float Delta => newValue - oldValue;

			public AnemometerChangeResult()
			{
			}
		}
	}
}
