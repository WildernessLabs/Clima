using Meadow.Devices.Clima.Hardware;
using Meadow.Devices.Clima.Models;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace Meadow.Devices.Clima.Controllers;

/// <summary>
/// Controller for managing sensor data from various sensors in the Clima hardware.
/// </summary>
public class SensorController
{
    private readonly CircularBuffer<Azimuth> windVaneBuffer = new CircularBuffer<Azimuth>(12);
    private readonly SensorData latestData;

    /// <summary>
    /// Gets or sets a value indicating whether to log sensor data.
    /// </summary>
    private bool LogSensorData { get; set; } = false;

    /// <summary>
    /// Gets the interval at which sensor data is updated.
    /// </summary>
    public TimeSpan UpdateInterval { get; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Initializes a new instance of the <see cref="SensorController"/> class.
    /// </summary>
    /// <param name="clima">The Clima hardware interface.</param>
    public SensorController(IClimaHardware clima)
    {
        latestData = new SensorData();

        if (clima.TemperatureSensor is { } temperatureSensor)
        {
            temperatureSensor.Updated += TemperatureUpdated;
            temperatureSensor.StartUpdating(TimeSpan.FromSeconds(15));
            temperatureSensor.StartUpdating(UpdateInterval);
        }

        if (clima.BarometricPressureSensor is { } pressureSensor)
        {
            pressureSensor.Updated += PressureUpdated;
            pressureSensor.StartUpdating(TimeSpan.FromMinutes(1));
        }

        if (clima.HumiditySensor is { } humiditySensor)
        {
            humiditySensor.Updated += HumidityUpdated;
            humiditySensor.StartUpdating(TimeSpan.FromMinutes(1));
        }

        if (clima.CO2ConcentrationSensor is { } co2Sensor)
        {
            co2Sensor.Updated += Co2Updated;
            co2Sensor.StartUpdating(TimeSpan.FromMinutes(5));
        }

        if (clima.WindVane is { } windVane)
        {
            windVane.Updated += WindvaneUpdated;
            windVane.StartUpdating(TimeSpan.FromSeconds(1));
        }

        if (clima.RainGauge is { } rainGuage)
        {
            rainGuage.Updated += RainGaugeUpdated;

            rainGuage.StartUpdating(TimeSpan.FromMinutes(5));
        }

        if (clima.Anemometer is { } anemometer)
        {
            anemometer.Updated += AnemometerUpdated;
            anemometer.StartUpdating(UpdateInterval);
        }

        if (clima.LightSensor is { } lightSensor)
        {
            lightSensor.Updated += LightSensorUpdated;
            lightSensor.StartUpdating(UpdateInterval);
        }
    }

    /// <summary>
    /// Gets the latest sensor data.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the latest sensor data.</returns>
    public Task<SensorData> GetSensorData()
    {
        lock (latestData)
        {
            var data = latestData.Copy();

            latestData.Clear();

            return Task.FromResult(data);
        }
    }

    private void LightSensorUpdated(object sender, IChangeResult<Illuminance> e)
    {
        lock (latestData)
        {
            latestData.Light = e.New;
        }

        Resolver.Log.InfoIf(LogSensorData, $"Light:     {e.New.Lux:0.#} lux");
    }

    private void TemperatureUpdated(object sender, IChangeResult<Temperature> e)
    {
        lock (latestData)
        {
            latestData.Temperature = e.New;
        }

        Resolver.Log.InfoIf(LogSensorData, $"Temperature:     {e.New.Celsius:0.#}C");
    }

    private void PressureUpdated(object sender, IChangeResult<Pressure> e)
    {
        lock (latestData)
        {
            latestData.Pressure = e.New;
        }

        Resolver.Log.InfoIf(LogSensorData, $"Pressure:        {e.New.Millibar:0.#}mbar");
    }

    private void HumidityUpdated(object sender, IChangeResult<RelativeHumidity> e)
    {
        lock (latestData)
        {
            latestData.Humidity = e.New;
        }

        Resolver.Log.InfoIf(LogSensorData, $"Humidity:        {e.New.Percent:0.#}%");
    }

    private void Co2Updated(object sender, IChangeResult<Concentration> e)
    {
        lock (latestData)
        {
            latestData.Co2Level = e.New;
        }
        Resolver.Log.InfoIf(LogSensorData, $"CO2:             {e.New.PartsPerMillion:0.#}ppm");
    }

    private void AnemometerUpdated(object sender, IChangeResult<Speed> e)
    {
        lock (latestData)
        {
            latestData.WindSpeed = e.New;
        }

        Resolver.Log.InfoIf(LogSensorData, $"Anemometer:      {e.New.MetersPerSecond:0.#} m/s");
    }

    private void RainGaugeUpdated(object sender, IChangeResult<Length> e)
    {
        lock (latestData)
        {
            latestData.Rain = e.New;
        }

        Resolver.Log.InfoIf(LogSensorData, $"Rain Gauge:      {e.New.Millimeters:0.#} mm");
    }

    private void WindvaneUpdated(object sender, IChangeResult<Azimuth> e)
    {
        windVaneBuffer.Append(e.New);

        lock (latestData)
        {
            latestData.WindDirection = windVaneBuffer.Mean();
        }

        Resolver.Log.InfoIf(LogSensorData, $"Wind Vane:       {e.New.DecimalDegrees} (mean: {windVaneBuffer.Mean().DecimalDegrees})");
    }
}