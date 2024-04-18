using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using Meadow.Peripherals.Sensors.Location.Gnss;
using Meadow.Units;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clima_Demo;

public class MeadowApp : App<F7CoreComputeV2>
{
    private IClimaHardware clima;
    private NotificationController notificationController;

    public MeadowApp()
    {
        Resolver.Services.Add(new CloudController());
    }

    public override void OnBootFromCrash(IEnumerable<string> crashReports)
    {
        Resolver.Services.Get<CloudController>()?.LogEvent(CloudEventIds.BootFromCrash, "Device restarted after a crash");
    }

    public override Task Initialize()
    {
        Resolver.Log.LogLevel = Meadow.Logging.LogLevel.Information;

        Resolver.Log.Info("Initialize hardware...");

        clima = Clima.Create();

        notificationController = new NotificationController(clima.RgbLed);
        Resolver.Services.Add<NotificationController>(notificationController);

        notificationController.Starting();

        Resolver.Services.Get<CloudController>()?.LogEvent(CloudEventIds.DeviceStarted, $"Device started (hardware {clima.RevisionString})");
        Resolver.Log.Info($"Running on Clima Hardware {clima.RevisionString}");

        InitializeSensors();

        Resolver.Log.Info("Initialization complete");

        clima.RgbLed.SetColor(Color.Yellow);

        var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
        wifi.NetworkConnected += OnNetworkConnected;
        wifi.NetworkDisconnected += OnNetworkDisconnected;

        if (wifi.IsConnected)
        {
            notificationController.NetworkConnected();
        }
        else
        {
            notificationController.NetworkDisconnected();
        }

        return Task.CompletedTask;
    }

    private void OnNetworkDisconnected(INetworkAdapter sender, NetworkDisconnectionEventArgs args)
    {
        notificationController.NetworkDisconnected();
    }

    private void OnNetworkConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
    {
        notificationController.NetworkConnected();
    }

    private void InitializeSensors()
    {
        if (clima.TemperatureSensor is { } temperatureSensor)
        {
            temperatureSensor.Updated += TemperatureUpdated;
        }

        if (clima.BarometricPressureSensor is { } pressureSensor)
        {
            pressureSensor.Updated += PressureUpdated;
        }

        if (clima.HumiditySensor is { } humiditySensor)
        {
            humiditySensor.Updated += HumidityUpdated;
        }

        if (clima.CO2ConcentrationSensor is { } co2Sensor)
        {
            co2Sensor.Updated += Co2Updated;
        }

        if (clima.WindVane is { } windVane)
        {
            windVane.Updated += WindvaneUpdated;
        }

        if (clima.RainGauge is { } rainGuage)
        {
            rainGuage.Updated += RainGuageUpdated;
        }

        if (clima.Anemometer is { } anemometer)
        {
            anemometer.Updated += AnemometerUpdated;
        }

        if (clima.SolarVoltageInput is { } solarVoltage)
        {
            solarVoltage.Updated += SolarVoltageUpdated;
        }

        if (clima.BatteryVoltageInput is { } batteryVoltage)
        {
            batteryVoltage.Updated += BatteryVoltageUpdated;
        }

        if (clima.Gnss is { } gnss)
        {
            //gnss.GsaReceived += GnssGsaReceived;
            //gnss.GsvReceived += GnssGsvReceived;
            //gnss.VtgReceived += GnssVtgReceived;
            gnss.RmcReceived += GnssRmcReceived;
            gnss.GllReceived += GnssGllReceived;
        }
    }

    private void GnssGsaReceived(object _, ActiveSatellites e)
    {
        if (e.SatellitesUsedForFix is { } sats)
        {
            Resolver.Log.Info($"Number of active satellites: {sats.Length}");
        }
    }

    private void GnssGsvReceived(object _, SatellitesInView e)
    {
        Resolver.Log.Info($"Satellites in view: {e.Satellites.Length}");
    }

    private void GnssVtgReceived(object _, CourseOverGround e)
    {
        if (e is { } cv)
        {
            Resolver.Log.Info($"{cv}");
        };
    }

    private void GnssRmcReceived(object _, GnssPositionInfo e)
    {
        if (e.Valid)
        {
            Resolver.Log.Info($"GNSS Position: lat: [{e.Position.Latitude}], long: [{e.Position.Longitude}]");
        }
    }

    private void GnssGllReceived(object _, GnssPositionInfo e)
    {
        if (e.Valid)
        {
            Resolver.Log.Info($"GNSS Position: lat: [{e.Position.Latitude}], long: [{e.Position.Longitude}]");
        }
    }

    public override Task Run()
    {
        Resolver.Log.Info("Run...");

        var updateInterval = TimeSpan.FromSeconds(5);

        if (clima.TemperatureSensor is { } temp)
        {
            temp.StartUpdating(updateInterval);
        }

        if (clima.HumiditySensor is { } humidity)
        {
            humidity.StartUpdating(updateInterval);
        }

        if (clima.BarometricPressureSensor is { } pressure)
        {
            pressure.StartUpdating(updateInterval);
        }

        if (clima.CO2ConcentrationSensor is { } co2)
        {
            co2.StartUpdating(updateInterval);
        }

        if (clima.WindVane is { } windVane)
        {
            windVane.StartUpdating(updateInterval);
        }

        if (clima.RainGauge is { } rainGuage)
        {
            rainGuage.StartUpdating(updateInterval);
        }

        if (clima.Anemometer is { } anemometer)
        {
            anemometer.StartUpdating(updateInterval);
        }

        if (clima.SolarVoltageInput is { } solarVoltage)
        {
            solarVoltage.StartUpdating(updateInterval);
        }

        if (clima.BatteryVoltageInput is { } batteryVoltage)
        {
            batteryVoltage.StartUpdating(updateInterval);
        }

        if (clima.Gnss is { } gnss)
        {
            gnss.StartUpdating();
        }

        return base.Run();
    }

    private void TemperatureUpdated(object sender, IChangeResult<Temperature> e)
    {
        Resolver.Log.Info($"Temperature:     {e.New.Celsius:0.#}C");
    }

    private void PressureUpdated(object sender, IChangeResult<Pressure> e)
    {
        Resolver.Log.Info($"Pressure:        {e.New.Millibar:0.#}mbar");
    }

    private void HumidityUpdated(object sender, IChangeResult<RelativeHumidity> e)
    {
        Resolver.Log.Info($"Humidity:        {e.New.Percent:0.#}%");
    }

    private void Co2Updated(object sender, IChangeResult<Concentration> e)
    {
        Resolver.Log.Info($"CO2:             {e.New.PartsPerMillion:0.#}ppm");
    }

    private void SolarVoltageUpdated(object sender, IChangeResult<Voltage> e)
    {
        Resolver.Log.Info($"Solar Voltage:   {e.New.Volts:0.#} volts");
    }

    private void BatteryVoltageUpdated(object sender, IChangeResult<Voltage> e)
    {
        Resolver.Log.Info($"Battery Voltage: {e.New.Volts:0.#} volts");
    }

    private void AnemometerUpdated(object sender, IChangeResult<Speed> e)
    {
        Resolver.Log.Info($"Anemometer:      {e.New.MetersPerSecond:0.#} m/s");
    }

    private void RainGuageUpdated(object sender, IChangeResult<Length> e)
    {
        Resolver.Log.Info($"Rain Gauge:      {e.New.Millimeters:0.#} mm");
    }

    private void WindvaneUpdated(object sender, IChangeResult<Azimuth> e)
    {
        Resolver.Log.Info($"Wind Vane:       {e.New.Compass16PointCardinalName} ({e.New.Radians:0.#} radians)");
    }
}