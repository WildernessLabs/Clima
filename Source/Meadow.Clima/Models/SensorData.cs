using Meadow.Units;
using System.Collections.Generic;

namespace Meadow.Devices.Clima.Models;

/// <summary>
/// Represents the clima sensor data 
/// </summary>
public class SensorData
{
    /// <summary>
    /// Gets or sets the temperature.
    /// </summary>
    public Temperature? Temperature { get; set; }

    /// <summary>
    /// Gets or sets the pressure.
    /// </summary>
    public Pressure? Pressure { get; set; }

    /// <summary>
    /// Gets or sets the relative humidity.
    /// </summary>
    public RelativeHumidity? Humidity { get; set; }

    /// <summary>
    /// Gets or sets the CO2 level.
    /// </summary>
    public Concentration? Co2Level { get; set; }

    /// <summary>
    /// Gets or sets the wind speed.
    /// </summary>
    public Speed? WindSpeed { get; set; }

    /// <summary>
    /// Gets or sets the wind direction.
    /// </summary>
    public Azimuth? WindDirection { get; set; }

    /// <summary>
    /// Gets or sets the rain length.
    /// </summary>
    public Length? Rain { get; set; }

    /// <summary>
    /// Gets or sets the illuminance.
    /// </summary>
    public Illuminance? Light { get; set; }

    /// <summary>
    /// Clears all the sensor data.
    /// </summary>
    public void Clear()
    {
        Co2Level = null;
        Temperature = null;
        Pressure = null;
        WindSpeed = null;
        WindDirection = null;
        Rain = null;
        Light = null;
    }

    /// <summary>
    /// Creates a copy of the SensorData object.
    /// </summary>
    public SensorData Copy()
    {
        return new SensorData
        {
            Co2Level = Co2Level,
            Temperature = Temperature,
            Pressure = Pressure,
            WindSpeed = WindSpeed,
            WindDirection = WindDirection,
            Rain = Rain,
            Light = Light,
        };
    }


    /// <summary>
    /// Converts the SensorData object to a dictionary suitable for telemetry.
    /// </summary>
    /// <returns>A dictionary containing the telemetry data.</returns>
    public Dictionary<string, object> AsTelemetryDictionary()
    {
        var d = new Dictionary<string, object>();
        if (Temperature != null)
        {
            d.Add(nameof(Temperature), Temperature.Value.Celsius);
        }
        if (Pressure != null)
        {
            d.Add(nameof(Pressure), Pressure.Value.Bar);
        }
        if (Humidity != null)
        {
            d.Add(nameof(Humidity), Humidity.Value.Percent);
        }
        if (Co2Level != null)
        {
            d.Add(nameof(Co2Level), Co2Level.Value.PartsPerMillion);
        }
        if (WindSpeed != null)
        {
            d.Add(nameof(WindSpeed), WindSpeed.Value.KilometersPerHour);
        }
        if (WindDirection != null)
        {
            d.Add(nameof(WindDirection), WindDirection.Value.DecimalDegrees);
        }
        if (Rain != null)
        {
            d.Add(nameof(Rain), Rain.Value.Centimeters);
        }
        if (Light != null)
        {
            d.Add(nameof(Light), Light.Value.Lux);
        }

        return d;
    }
}