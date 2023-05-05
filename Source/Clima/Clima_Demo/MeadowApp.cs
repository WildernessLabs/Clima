using Meadow;
using Meadow.Devices;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace Clima_Demo
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7CoreComputeV2> //   F7FeatherV2>
    {
        IClimaHardware clima;

        public override Task Initialize()
        {
            Resolver.Log.Loglevel = Meadow.Logging.LogLevel.Trace;

            Resolver.Log.Info("Initialize hardware...");

            clima = Clima.Create();

            Resolver.Log.Info($"Running on Clima Hardware {clima.RevisionString}");

            //---- BME688 Atmospheric sensor
            if (clima.AtmosphericSensor is { } bme688)
            {
                bme688.Updated += Bme688Updated;
            }

            //---- SCD40 Environmental sensor
            if (clima.EnvironmentalSensor is { } scd40)
            {
                scd40.Updated += Scd40Updated;
            }

            //---- Wind Vane sensor
            if (clima.WindVane is { } windVane)
            {
                windVane.Updated += WindvaneUpdated;
            }

            //---- Rain Gauge sensor
            if (clima.RainGauge is { } rainGuage)
            {
                rainGuage.Updated += RainGuageUpdated;
            }

            //---- Anemometer
            if (clima.Anemometer is { } anemometer)
            {
                anemometer.Updated += AnemometerUpdated;
            }

            //---- Solar Voltage Input sensor
            if (clima.SolarVoltageInput is { } solarVoltage)
            {
                solarVoltage.Updated += SolarVoltageUpdated;
            }

            //---- heartbeat
            Resolver.Log.Info("Initialization complete");

            return base.Initialize();
        }

        public override Task Run()
        {
            Resolver.Log.Info("Run...");

            //---- BME688 Atmospheric sensor
            if (clima.EnvironmentalSensor is { } bme688)
            {
                bme688.StartUpdating(TimeSpan.FromSeconds(5));
            }

            return base.Run();
        }

        private void Bme688Updated(object sender, IChangeResult<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance)> e)
        {
            Resolver.Log.Info($"BME688: {(int)e.New.Temperature?.Celsius}C - {(int)e.New.Humidity?.Percent}% - {(int)e.New.Pressure?.Millibar}mbar");
        }

        private void SolarVoltageUpdated(object sender, IChangeResult<Voltage> e)
        {
            Resolver.Log.Info($"SolarVoltage: {e.New.Volts} volts");
        }

        private void AnemometerUpdated(object sender, IChangeResult<Speed> e)
        {
            Resolver.Log.Info($"Anemometer: {e.New.MetersPerSecond} m/s");
        }

        private void RainGuageUpdated(object sender, IChangeResult<Length> e)
        {
            Resolver.Log.Info($"RainGauge: {e.New.Millimeters} mm");
        }

        private void WindvaneUpdated(object sender, IChangeResult<Azimuth> e)
        {
            Resolver.Log.Info($"$WineVane: {e.New.Compass16PointCardinalName} ({e.New.Radians:0} radians)");
        }

        private void Scd40Updated(object sender, IChangeResult<(Concentration? Concentration, Temperature? Temperature, RelativeHumidity? Humidity)> e)
        {
            Resolver.Log.Info($"SCD40: {e.New.Concentration.Value.PartsPerMillion}ppm, {e.New.Temperature.Value.Celsius}C, {e.New.Humidity.Value.Percent}%");
        }
    }
}