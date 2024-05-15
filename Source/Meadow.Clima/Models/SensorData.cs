using Meadow.Units;
using System.Collections.Generic;

namespace Clima_Demo;

public record SensorData
{
    public Temperature? Temperature { get; set; }
    public Pressure? Pressure { get; set; }
    public RelativeHumidity? Humidity { get; set; }
    public Concentration? Co2Level { get; set; }
    public Speed? WindSpeed { get; set; }
    public Azimuth? WindDirection { get; set; }
    public Length? Rain { get; set; }
    public Illuminance? Light { get; set; }

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
