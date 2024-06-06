using Meadow;
using Meadow.Devices;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace Clima_Demo;

public class SensorController
{
    private IClimaHardware hardware;
    private CircularBuffer<Azimuth> windVaneBuffer = new CircularBuffer<Azimuth>(12);
    private Length? startupRainValue;
    private SensorData latestData;

    private bool LogSensorData { get; set; } = false;
    public TimeSpan UpdateInterval { get; } = TimeSpan.FromSeconds(5);

    public SensorController(IClimaHardware clima)
    {
        latestData = new SensorData();
        hardware = clima;

        if (clima.TemperatureSensor is { } temperatureSensor)
        {
            temperatureSensor.Updated += TemperatureUpdated;
            // atmospheric temp is slow to change
            temperatureSensor.StartUpdating(TimeSpan.FromSeconds(15));
            temperatureSensor.StartUpdating(UpdateInterval);
        }

        if (clima.BarometricPressureSensor is { } pressureSensor)
        {
            pressureSensor.Updated += PressureUpdated;
            // barometric pressure is slow to change
            pressureSensor.StartUpdating(TimeSpan.FromMinutes(1));
        }

        if (clima.HumiditySensor is { } humiditySensor)
        {
            humiditySensor.Updated += HumidityUpdated;
            // humidity is slow to change
            humiditySensor.StartUpdating(TimeSpan.FromMinutes(1));
        }

        if (clima.CO2ConcentrationSensor is { } co2Sensor)
        {
            co2Sensor.Updated += Co2Updated;
            // CO2 levels are slow to change
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

            // TODO: if we're restarting, we need to rehydrate today's totals already collected
            //            startupRainValue = rainGuage.Read().Result;
            //            Resolver.Log.Info($"Startup rain value: {startupRainValue}");

            // rain does not change frequently
            rainGuage.StartUpdating(TimeSpan.FromMinutes(5));
        }

        if (clima.Anemometer is { } anemometer)
        {
            anemometer.Updated += AnemometerUpdated;
            anemometer.StartUpdating(UpdateInterval);
        }

        if (clima.LightSensor is { } lightSensor)
        {
            lightSensor.StartUpdating(UpdateInterval);
        }
    }

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
