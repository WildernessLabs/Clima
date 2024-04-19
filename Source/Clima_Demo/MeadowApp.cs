using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clima_Demo;

public class MeadowApp : App<F7CoreComputeV2>
{
    private IClimaHardware clima;
    private NotificationController notificationController;
    private SensorController sensorController;
    private PowerController powerController;
    private LocationController locationController;

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
        locationController = new LocationController(clima);

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

        Resolver.Log.Info("Initialization complete");

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
}