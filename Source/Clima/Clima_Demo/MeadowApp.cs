using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Peripherals.Sensors.Location.Gnss;
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

            //---- GNSS
            if (clima.Gnss is { } gnss)
            {
                //    gnss.GllReceived += GnssGllReceived;
                //    gnss.RmcReceived += GnssRmcReceived;
                //    gnss.VtgReceived += GnssVtgReceived;
                //    gnss.GsvReceived += GnssGsvReceived;
                //    gnss.GsaReceived += GnssGsaReceived;
            }

            //---- heartbeat
            Resolver.Log.Info("Initialization complete");

            //---- set LED green
            clima.ColorLed.SetColor(Color.Green);

            return base.Initialize();
        }

        private void GnssGsaReceived(object sender, ActiveSatellites e)
        {
            if (e.SatellitesUsedForFix is { } sats)
            {
                Resolver.Log.Info($"Number of active satellites: {sats.Length}");
            }
        }

        private void GnssGsvReceived(object sender, SatellitesInView e)
        {
            Resolver.Log.Info($"Satellites in view: {e.Satellites.Length}");
        }

        private void GnssVtgReceived(object sender, CourseOverGround e)
        {
            if (e is { } cv)
            {
                Resolver.Log.Info($"{cv}");
            };
        }

        private void GnssRmcReceived(object sender, GnssPositionInfo e)
        {
            if (e.Valid)
            {
                Resolver.Log.Info($"GNSS Position: lat: [{e.Position.Latitude}], long: [{e.Position.Longitude}]");
            }
        }

        private void GnssGllReceived(object sender, GnssPositionInfo e)
        {
            if (e.Valid)
            {
                Resolver.Log.Info($"GNSS Position: lat: [{e.Position.Latitude}], long: [{e.Position.Longitude}]");
            }
        }

        public override Task Run()
        {
            Resolver.Log.Info("Run...");

            var updateInterval = TimeSpan.FromSeconds(2);

            //---- BME688 Atmospheric sensor
            if (clima.EnvironmentalSensor is { } bme688)
            {
                bme688.StartUpdating(updateInterval);
            }

            //---- SCD40 Environmental sensor
            if (clima.EnvironmentalSensor is { } scd40)
            {
                //    scd40.StartUpdating(updateInterval);
            }

            //---- Wind Vane sensor
            if (clima.WindVane is { } windVane)
            {
                windVane.StartUpdating(updateInterval);
            }

            //---- Rain Gauge sensor
            if (clima.RainGauge is { } rainGuage)
            {
                rainGuage.StartUpdating(updateInterval);
            }

            //---- Anemometer
            if (clima.Anemometer is { } anemometer)
            {
                anemometer.StartUpdating(updateInterval);
            }

            //---- Solar Voltage Input sensor
            if (clima.SolarVoltageInput is { } solarVoltage)
            {
                solarVoltage.StartUpdating(updateInterval);
            }

            //---- Neo8 GNSS
            if (clima.Gnss is { } gnss)
            {
                gnss.StartUpdating();
            }

            return base.Run();
        }

        private void Bme688Updated(object sender, IChangeResult<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance)> e)
        {
            Resolver.Log.Info($"BME688: {(int)e.New.Temperature?.Celsius:0.#}C - {(int)e.New.Humidity?.Percent:0.#}% - {(int)e.New.Pressure?.Millibar:0.#}mbar");
        }

        private void SolarVoltageUpdated(object sender, IChangeResult<Voltage> e)
        {
            Resolver.Log.Info($"SolarVoltage: {e.New.Volts:0.#} volts");
        }

        private void AnemometerUpdated(object sender, IChangeResult<Speed> e)
        {
            Resolver.Log.Info($"Anemometer: {e.New.MetersPerSecond:0.#} m/s");
        }

        private void RainGuageUpdated(object sender, IChangeResult<Length> e)
        {
            Resolver.Log.Info($"RainGauge: {e.New.Millimeters:0.#} mm");
        }

        private void WindvaneUpdated(object sender, IChangeResult<Azimuth> e)
        {
            Resolver.Log.Info($"$WineVane: {e.New.Compass16PointCardinalName} ({e.New.Radians::0.#} radians)");
        }

        private void Scd40Updated(object sender, IChangeResult<(Concentration? Concentration, Temperature? Temperature, RelativeHumidity? Humidity)> e)
        {
            Resolver.Log.Info($"SCD40: {e.New.Concentration.Value.PartsPerMillion:0.#}ppm, {e.New.Temperature.Value.Celsius:0.0}C, {e.New.Humidity.Value.Percent:0.0}%");
        }
    }
}