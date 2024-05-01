using Meadow.Units;
using System.Collections.Generic;

namespace Clima_Demo;

public record PowerData
{
    public Voltage? SolarVoltage { get; set; }
    public Voltage? BatteryVoltage { get; set; }

    public Dictionary<string, object> AsTelemetryDictionary()
    {
        var d = new Dictionary<string, object>();
        if (SolarVoltage != null)
        {
            d.Add(nameof(SolarVoltage), SolarVoltage.Value.Volts);
        }
        if (BatteryVoltage != null)
        {
            d.Add(nameof(BatteryVoltage), BatteryVoltage.Value.Volts);
        }

        return d;
    }
}
