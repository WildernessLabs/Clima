using Clima_Demo;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow.Devices;

public class MainController
{
    private IClimaHardware hardware;
    private NotificationController notificationController;
    private SensorController sensorController;
    private PowerController powerController;
    private LocationController locationController;
    private NetworkController? networkController;
    private CloudController cloudController;
    private Timer TelemetryTimer;

    public TimeSpan TelemetryPublicationPeriod { get; } = TimeSpan.FromMinutes(1);

    public Task Initialize(IClimaHardware hardware, INetworkAdapter? networkAdapter)
    {
        this.hardware = hardware;

        Resolver.Log.Info("Initialize hardware...");

        notificationController = new NotificationController(hardware.RgbLed);
        Resolver.Services.Add(notificationController);

        notificationController.SetSystemStatus(NotificationController.SystemStatus.Starting);

        cloudController = new CloudController();

        Resolver.Services.Get<CloudController>()?.LogAppStartup(hardware.RevisionString);
        Resolver.Log.Info($"Running on Clima Hardware {hardware.RevisionString}");

        sensorController = new SensorController(hardware);

        powerController = new PowerController(hardware);
        powerController.SolarVoltageWarning += OnSolarVoltageWarning;
        powerController.BatteryVoltageWarning += OnBatteryVoltageWarning;

        locationController = new LocationController(hardware);

        if (networkAdapter == null)
        {
            Resolver.Log.Error("No network adapter found!");
        }
        else
        {
            networkController = new NetworkController(networkAdapter);
            networkController.ConnectionStateChanged += OnNetworkConnectionStateChanged;
            networkController.NetworkDown += OnNetworkStillDown;

            if (!networkController.IsConnected)
            {
                notificationController.SetSystemStatus(NotificationController.SystemStatus.SearchingForNetwork);
                Resolver.Log.Info("Network is down");
            }
            else
            {
                notificationController.SetSystemStatus(NotificationController.SystemStatus.NetworkConnected);
                if (Resolver.MeadowCloudService.ConnectionState == CloudConnectionState.Connecting)
                {
                    notificationController.SetSystemStatus(NotificationController.SystemStatus.ConnectingToCloud);
                }
            }
        }

        Resolver.MeadowCloudService.ConnectionStateChanged += OnMeadowCloudServiceConnectionStateChanged;
        cloudController.LogAppStartup(hardware.RevisionString);

        TelemetryTimer = new Timer(TelemetryTimerProc, null, 0, -1);

        return Task.CompletedTask;
    }

    private void OnMeadowCloudServiceConnectionStateChanged(object sender, CloudConnectionState e)
    {
        switch (e)
        {
            case CloudConnectionState.Connected:
                notificationController.SetSystemStatus(NotificationController.SystemStatus.Connected);
                break;
            default:
                notificationController.SetSystemStatus(NotificationController.SystemStatus.ConnectingToCloud);
                break;
        }
    }

    private async void TelemetryTimerProc(object _)
    {
        Resolver.Log.Info($"Collecting telemetry");

        try
        {
            cloudController.LogTelemetry(
                await sensorController.GetSensorData(),
                await powerController.GetPowerData());
        }
        catch (Exception ex)
        {
            Resolver.Log.Warn($"Failed to log telemetry: {ex.Message}");
        }

        TelemetryTimer.Change(TelemetryPublicationPeriod, TimeSpan.FromMilliseconds(-1));
    }

    private void OnNetworkStillDown(object sender, System.TimeSpan e)
    {
        Resolver.Log.Info($"Network has been down for {e.TotalSeconds:N0} seconds");

        // TODO: after some period, should we force-restart the device?
        if (e.TotalMinutes > 5)
        {
            Resolver.Log.Info($"Network Connection timeout.  Resetting the device.");
            Resolver.Device.PlatformOS.Reset();
        }
    }

    private void OnNetworkConnectionStateChanged(object sender, bool e)
    {
        if (e)
        {
            Resolver.Log.Info($"Network connected");
            notificationController.ClearWarning(NotificationController.Warnings.NetworkDisconnected);
            notificationController.SetSystemStatus(NotificationController.SystemStatus.NetworkConnected);
        }
        else
        {
            Resolver.Log.Info($"Network disconnected");
            notificationController.SetWarning(NotificationController.Warnings.NetworkDisconnected);
        }
    }

    private void OnBatteryVoltageWarning(object sender, bool e)
    {
        if (e)
        {
            var message = $"Battery voltage dropped below {powerController.LowBatteryWarningLevel.Volts:N1}";
            Resolver.Log.Warn(message);

            //notificationController.SetWarning(NotificationController.Warnings.BatteryLow);
            cloudController.LogWarning(message);
        }
        else
        {
            var message = $"Battery voltage is back above minimum";
            Resolver.Log.Info(message);

            notificationController.ClearWarning(NotificationController.Warnings.BatteryLow);
            cloudController.LogMessage(message);
        }
    }

    private void OnSolarVoltageWarning(object sender, bool e)
    {
        if (e)
        {
            var message = $"Solar voltage dropped below {powerController.LowSolarWarningLevel.Volts:N1}";
            Resolver.Log.Warn(message);

            //notificationController.SetWarning(NotificationController.Warnings.SolarLoadLow);
            cloudController.LogWarning(message);
        }
        else
        {
            var message = $"Solar voltage is back above minimum";
            Resolver.Log.Info(message);

            notificationController.ClearWarning(NotificationController.Warnings.SolarLoadLow);
            cloudController.LogMessage(message);
        }
    }

    public Task Run()
    {
        return Task.CompletedTask;
    }

    public void LogAppStartupAfterCrash(IEnumerable<string> crashReports)
    {
        // the cloud service's health reporter will log this for us automatically, so no need to manually do so
        Resolver.Log.Warn("Boot after crash!");

        foreach (var report in crashReports)
        {
            Resolver.Log.Info(report);
        }
    }

}
