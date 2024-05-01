using Meadow;
using Meadow.Devices;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace Clima_Demo;

public class SensorController
{
    private IClimaHardware hardware;

    public bool LogSensorData { get; set; } = false;
    public TimeSpan UpdateInterval { get; } = TimeSpan.FromSeconds(5);

    public SensorController(IClimaHardware clima)
    {
        hardware = clima;

        if (clima.TemperatureSensor is { } temperatureSensor)
        {
            temperatureSensor.Updated += TemperatureUpdated;
            temperatureSensor.StartUpdating(UpdateInterval);
        }

        if (clima.BarometricPressureSensor is { } pressureSensor)
        {
            pressureSensor.Updated += PressureUpdated;
            pressureSensor.StartUpdating(UpdateInterval);
        }

        if (clima.HumiditySensor is { } humiditySensor)
        {
            humiditySensor.Updated += HumidityUpdated;
            humiditySensor.StartUpdating(UpdateInterval);
        }

        if (clima.CO2ConcentrationSensor is { } co2Sensor)
        {
            co2Sensor.Updated += Co2Updated;
            co2Sensor.StartUpdating(UpdateInterval);
        }

        if (clima.WindVane is { } windVane)
        {
            windVane.Updated += WindvaneUpdated;
            windVane.StartUpdating(UpdateInterval);
        }

        if (clima.RainGauge is { } rainGuage)
        {
            rainGuage.Updated += RainGuageUpdated;
            rainGuage.StartUpdating(UpdateInterval);
        }

        if (clima.Anemometer is { } anemometer)
        {
            anemometer.Updated += AnemometerUpdated;
            anemometer.StartUpdating(UpdateInterval);
        }
    }

    public async Task<SensorData> GetSensorData()
    {
        return new SensorData
        {
            Temperature = hardware.TemperatureSensor?.Temperature ?? null,
            Pressure = hardware.BarometricPressureSensor?.Pressure ?? null,
            Humidity = hardware.HumiditySensor?.Humidity ?? null,
            Co2Level = hardware.CO2ConcentrationSensor?.CO2Concentration ?? null,
            WindSpeed = hardware.Anemometer?.WindSpeed ?? null,
            WindDirection = hardware.WindVane?.WindAzimuth ?? null,
            Rain = hardware.RainGauge?.RainDepth ?? null,
        };
    }

    private void TemperatureUpdated(object sender, IChangeResult<Temperature> e)
    {
        Resolver.Log.InfoIf(LogSensorData, $"Temperature:     {e.New.Celsius:0.#}C");
    }

    private void PressureUpdated(object sender, IChangeResult<Pressure> e)
    {
        Resolver.Log.InfoIf(LogSensorData, $"Pressure:        {e.New.Millibar:0.#}mbar");
    }

    private void HumidityUpdated(object sender, IChangeResult<RelativeHumidity> e)
    {
        Resolver.Log.InfoIf(LogSensorData, $"Humidity:        {e.New.Percent:0.#}%");
    }

    private void Co2Updated(object sender, IChangeResult<Concentration> e)
    {
        Resolver.Log.InfoIf(LogSensorData, $"CO2:             {e.New.PartsPerMillion:0.#}ppm");
    }

    private void AnemometerUpdated(object sender, IChangeResult<Speed> e)
    {
        Resolver.Log.InfoIf(LogSensorData, $"Anemometer:      {e.New.MetersPerSecond:0.#} m/s");
    }

    private void RainGuageUpdated(object sender, IChangeResult<Length> e)
    {
        Resolver.Log.InfoIf(LogSensorData, $"Rain Gauge:      {e.New.Millimeters:0.#} mm");
    }

    private void WindvaneUpdated(object sender, IChangeResult<Azimuth> e)
    {
        Resolver.Log.InfoIf(LogSensorData, $"Wind Vane:       {e.New.Compass16PointCardinalName} ({e.New.Radians:0.#} radians)");
    }
}
