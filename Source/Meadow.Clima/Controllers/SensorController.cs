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
    private readonly IClimaHardware clima;
    private readonly CircularBuffer<Azimuth> windVaneBuffer = new CircularBuffer<Azimuth>(12);
    private readonly SensorData latestData;

    /// <summary>
    /// Gets or sets a value indicating whether to log sensor data.
    /// </summary>
    private bool LogSensorData { get; set; } = false;

    /// <summary>
    /// Gets the interval at which sensor data is updated.
    /// </summary>
    public TimeSpan UpdateInterval { get; private set; } = TimeSpan.FromSeconds(15);

    /// <summary>
    /// Initializes a new instance of the <see cref="SensorController"/> class.
    /// </summary>
    /// <param name="clima">The Clima hardware interface.</param>
    public SensorController(IClimaHardware clima)
    {
        latestData = new SensorData();
        this.clima = clima;
    }

    /// <summary>
    /// Stop the update events and remove event handler.
    /// </summary>
    public void StopUpdating()
    {
        if (clima.TemperatureSensor is { } temperatureSensor)
        {
            temperatureSensor.Updated -= TemperatureUpdated;
            temperatureSensor.StopUpdating();
        }
        if (clima.BarometricPressureSensor is { } pressureSensor)
        {
            pressureSensor.Updated -= PressureUpdated;
            // barometric pressure is slow to change
            pressureSensor.StopUpdating();
        }

        if (clima.HumiditySensor is { } humiditySensor)
        {
            humiditySensor.Updated -= HumidityUpdated;
            // humidity is slow to change
            humiditySensor.StopUpdating();
        }

        if (clima.CO2ConcentrationSensor is { } co2Sensor)
        {
            co2Sensor.Updated -= Co2Updated;
            // CO2 levels are slow to change
            co2Sensor.StopUpdating();
        }

        if (clima.WindVane is { } windVane)
        {
            windVane.Updated -= WindvaneUpdated;
            windVane.StopUpdating();
        }

        if (clima.RainGauge is { } rainGuage)
        {
            rainGuage.Updated -= RainGaugeUpdated;
            // rain does not change frequently
            rainGuage.StopUpdating();
        }

        if (clima.Anemometer is { } anemometer)
        {
            anemometer.Updated -= AnemometerUpdated;
            anemometer.StopUpdating();
        }
    }

    /// <summary>
    /// Add event handlers and start updating
    /// </summary>
    /// <param name="updateInterval"></param>
    public void StartUpdating(TimeSpan updateInterval)
    {
        UpdateInterval = updateInterval;

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
            rainGuage.Updated += RainGaugeUpdated;

            rainGuage.StartUpdating(UpdateInterval);
        }

        if (clima.Anemometer is { } anemometer)
        {
            anemometer.Updated += AnemometerUpdated;
            anemometer.StartUpdating(UpdateInterval);
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