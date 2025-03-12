using Meadow.Cloud;
using Meadow.Devices.Clima.Constants;
using Meadow.Devices.Clima.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meadow.Devices.Clima.Controllers;

/// <summary>
/// Controller for handling cloud-related operations.
/// </summary>
public class CloudController
{
    /// <summary>
    /// Waits for all data to be sent to the cloud.
    /// </summary>
    public async Task WaitForDataToSend()
    {
        // TODO: add a timeout here
        while (Resolver.MeadowCloudService.QueueCount > 0)
        {
            // Resolver.Log.Info($"Waiting for {Resolver.MeadowCloudService.QueueCount} items to be delivered...");
            await Task.Delay(1000);
        }
        Resolver.Log.Info($"All cloud data has been sent");
    }

    /// <summary>
    /// Logs the application startup after a crash.
    /// </summary>
    public void LogAppStartupAfterCrash()
    {
        SendEvent(CloudEventIds.DeviceStarted, $"Device restarted after crash");
    }

    /// <summary>
    /// Logs the application startup with the specified hardware revision.
    /// </summary>
    /// <param name="hardwareRevision">The hardware revision of the device.</param>
    public void LogAppStartup(string hardwareRevision)
    {
        SendEvent(CloudEventIds.DeviceStarted, $"Device started (hardware {hardwareRevision})");
    }

    /// <summary>
    /// Logs the device information including name and location.
    /// </summary>
    /// <param name="deviceName">The name of the device.</param>
    /// <param name="latitude">The latitude of the device location.</param>
    /// <param name="longitude">The longitude of the device location.</param>
    public void LogDeviceInfo(string deviceName, double latitude, double longitude)
    {
        Resolver.Log.Info("LogDeviceInfo: Create CloudEvent");
        CloudEvent cloudEvent = new CloudEvent
        {
            Description = "Clima Position Telemetry",
            Timestamp = DateTime.UtcNow,
            EventId = 109,
            Measurements = new Dictionary<string, object> { { "device_name", deviceName }, { "lat", latitude }, { "long", longitude } }
        };
        
        SendEvent(cloudEvent);
    }

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The warning message to log.</param>
    public void LogWarning(string message)
    {
        SendLog(message, "warning");
    }

    /// <summary>
    /// Logs an informational message.
    /// </summary>
    /// <param name="message">The informational message to log.</param>
    public void LogMessage(string message)
    {
        SendLog(message, "information");
    }

    /// <summary>
    /// Logs telemetry data from sensors and power data.
    /// </summary>
    /// <param name="sensorData">The sensor data to log.</param>
    /// <param name="powerData">The power data to log.</param>
    public void LogTelemetry(SensorData sensorData, PowerData powerData)
    {
        var measurements = sensorData
            .AsTelemetryDictionary()
            .Concat(powerData.AsTelemetryDictionary())
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        var cloudEvent = new CloudEvent
        {
            Description = "Clima Telemetry",
            Timestamp = DateTime.UtcNow,
            EventId = (int)CloudEventIds.Telemetry,
            Measurements = measurements
        };

        SendEvent(cloudEvent);
    }

    private void SendLog(string message, string severity)
    {
        if (Resolver.MeadowCloudService == null)
        {
            Resolver.Log.Warn($"CLOUD SERVICE IS NULL");
            return;
        }

        if (!Resolver.MeadowCloudService.IsEnabled)
        {
            Resolver.Log.Warn($"CLOUD INTEGRATION IS DISABLED");
            return;
        }

        Resolver.Log.Info($"Sending cloud log");

        Resolver.MeadowCloudService.SendLog(
            new CloudLog
            {
                Message = message,
                Timestamp = DateTime.UtcNow,
                Severity = severity
            });
    }

    private void SendEvent(CloudEventIds eventId, string message)
    {
        SendEvent(new CloudEvent
        {
            EventId = (int)eventId,
            Description = message,
            Timestamp = DateTime.UtcNow,
        });
    }

    private void SendEvent(CloudEvent cloudEvent)
    {
        if (Resolver.MeadowCloudService == null)
        {
            Resolver.Log.Warn($"CLOUD SERVICE IS NULL");
            return;
        }

        if (!Resolver.MeadowCloudService.IsEnabled)
        {
            Resolver.Log.Warn($"CLOUD INTEGRATION IS DISABLED");
            return;
        }

        Resolver.Log.Info($"Sending cloud event");

        try
        {
            Resolver.MeadowCloudService.SendEvent(cloudEvent);
        }
        catch (Exception ex)
        {
            Resolver.Log.Warn($"Failed to send cloud event: {ex.Message}");
        }
    }
}