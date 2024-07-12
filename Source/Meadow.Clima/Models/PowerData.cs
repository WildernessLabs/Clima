using Meadow.Units;
using System.Collections.Generic;

namespace Meadow.Devices.Clima.Models;

/// <summary>
/// Represents power data including solar voltage and battery voltage.
/// </summary>
public record PowerData
{
    /// <summary>
    /// Gets or sets the solar voltage.
    /// </summary>
    public Voltage? SolarVoltage { get; set; }

    /// <summary>
    /// Gets or sets the battery voltage.
    /// </summary>
    public Voltage? BatteryVoltage { get; set; }

    /// <summary>
    /// Converts the power data to a telemetry dictionary.
    /// </summary>
    /// <returns>The telemetry dictionary.</returns>
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