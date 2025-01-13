using Meadow.Devices.Clima.Controllers;
using Meadow.Devices.Clima.Hardware;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Meadow.Devices.Clima.Controllers.NotificationController;

namespace Meadow.Devices;

/// <summary>
/// Main controller for Clima.
/// </summary>
public class MainController
{
    private NotificationController notificationController;
    private SensorController sensorController;
    private PowerController powerController;
    private LocationController locationController;
    private NetworkController? networkController;
    private CloudController cloudController;
    private int tick;
    private const int SensorReadPeriodSeconds = 10;
    private const int PublicationPeriodMinutes = 1;
    private bool lowPowerMode = false;
    private Timer sleepSimulationTimer;

    /// <summary>
    /// Gets the telemetry publication period.
    /// </summary>
    public TimeSpan TelemetryPublicationPeriod { get; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Initializes the MainController with the clima hardware and network adapter.
    /// </summary>
    /// <param name="hardware">The Clima hardware to use.</param>
    /// <param name="networkAdapter">The network adapter to use, or null if no network adapter is available.</param>
    public Task Initialize(IClimaHardware hardware, INetworkAdapter? networkAdapter)
    {
        Resolver.Log.Info("Initialize hardware...");

        notificationController = new NotificationController(hardware.RgbLed);
        Resolver.Services.Add(notificationController);

        notificationController.SetSystemStatus(NotificationController.SystemStatus.Starting);

        cloudController = new CloudController();

        Resolver.Log.Info($"Running on Clima Hardware {hardware.RevisionString}");

        sensorController = new SensorController(hardware);

        powerController = new PowerController(hardware);
        powerController.SolarVoltageWarning += OnSolarVoltageWarning;
        powerController.BatteryVoltageWarning += OnBatteryVoltageWarning;

        locationController = new LocationController(hardware);

        locationController.PositionReceived += OnPositionReceived;

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

        Resolver.Device.PlatformOS.AfterWake += PlatformOS_AfterWake;

        if (!lowPowerMode)
        {
            sleepSimulationTimer = new Timer((_) => PlatformOS_AfterWake(this, WakeSource.Unknown), null, -1, -1);
        }

        _ = SystemPreSleepStateProc();

        return Task.CompletedTask;
    }

    private void OnPositionReceived(object sender, Peripherals.Sensors.Location.Gnss.GnssPositionInfo e)
    {
        if (e.Position != null)
        {
            // crop to 2 decimal places (~1km accuracy) for privacy
            var lat = Math.Round(e.Position.Latitude, 2);
            var lon = Math.Round(e.Position.Longitude, 2);

            cloudController.LogDeviceInfo(Resolver.Device.Information.DeviceName, lat, lon);
        }
    }

    private void PlatformOS_AfterWake(object sender, WakeSource e)
    {
        Resolver.Log.Info("PlatformOS_AfterWake");
        SystemPostWakeStateProc();
    }

    private async Task SystemPreSleepStateProc()
    {
        await CollectTelemetry();

        // connect to cloud
        if (networkController != null)
        {
            notificationController.SetSystemStatus(SystemStatus.SearchingForNetwork);
            var connected = await networkController.ConnectToCloud();
            if (connected)
            {
                if (cloudController != null)
                {
                    await cloudController.WaitForDataToSend();
                }

                if (lowPowerMode)
                {
                    await networkController.ShutdownNetwork();
                }
            }
        }

        notificationController.SetSystemStatus(SystemStatus.LowPower);
        if (lowPowerMode)
        {
            powerController.TimedSleep(TimeSpan.FromSeconds(SensorReadPeriodSeconds));
        }
        else
        {
            Resolver.Log.Info("Simulating sleep");
            sleepSimulationTimer.Change(TimeSpan.FromSeconds(SensorReadPeriodSeconds), TimeSpan.FromMilliseconds(-1));
        }
    }

    private void SystemPostWakeStateProc()
    {
        // collect data

        if (++tick % PublicationPeriodMinutes * 60 / SensorReadPeriodSeconds == 0)
        {
            _ = SystemPreSleepStateProc();
        }
        else
        {
            if (lowPowerMode)
            {
                powerController.TimedSleep(TimeSpan.FromSeconds(SensorReadPeriodSeconds));
            }
            else
            {
                sleepSimulationTimer.Change(TimeSpan.FromSeconds(SensorReadPeriodSeconds), TimeSpan.FromMilliseconds(-1));
            }
        }
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

    private async Task CollectTelemetry()
    {
        // collect telemetry every tick
        Resolver.Log.Info($"Collecting telemetry");

        try
        {
            // publish telemetry to the cloud every N ticks
            cloudController.LogTelemetry(
                await sensorController.GetSensorData(),
                await powerController.GetPowerData());
        }
        catch (Exception ex)
        {
            Resolver.Log.Warn($"Failed to log telemetry: {ex.Message}");
        }
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

    /// <summary>
    /// Logs the application startup after a crash.
    /// </summary>
    /// <param name="crashReports">Crash report data</param>
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