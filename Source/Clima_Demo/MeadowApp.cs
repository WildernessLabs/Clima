using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using Meadow.Peripherals.Sensors.Location.Gnss;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clima_Demo;

public class MeadowApp : App<F7CoreComputeV2>
{
    private IClimaHardware clima;
    private NotificationController notificationController;
    private SensorController sensorController;
    private PowerController powerController;

    public MeadowApp()
    {
        Resolver.Services.Add(new CloudController());
    }

    public override void OnBootFromCrash(IEnumerable<string> crashReports)
    {
        Resolver.Services.Get<CloudController>()?.LogAppStartupAfterCrash();
    }

    public override Task Initialize()
    {
        Resolver.Log.LogLevel = Meadow.Logging.LogLevel.Information;

        Resolver.Log.Info("Initialize hardware...");

        clima = Clima.Create();

        notificationController = new NotificationController(clima.RgbLed);
        Resolver.Services.Add<NotificationController>(notificationController);

        notificationController.Starting();

        Resolver.Services.Get<CloudController>()?.LogAppStartup(clima.RevisionString);
        Resolver.Log.Info($"Running on Clima Hardware {clima.RevisionString}");

        sensorController = new SensorController(clima);
        powerController = new PowerController(clima);

        InitializeSensors();

        Resolver.Log.Info("Initialization complete");

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

        if (clima.Gnss is { } gnss)
        {
            gnss.StartUpdating();
        }

        return base.Run();
    }
}