using Meadow.Cloud;
using Meadow.Devices.Clima.Constants;
using Meadow.Devices.Clima.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Meadow.Devices.Clima.Controllers;

public class CloudController
{
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

    public void LogAppStartupAfterCrash()
    {
        SendEvent(CloudEventIds.DeviceStarted, $"Device restarted after crash");
    }

    public void LogAppStartup(string hardwareRevision)
    {
        SendEvent(CloudEventIds.DeviceStarted, $"Device started (hardware {hardwareRevision})");
    }

    public void LogDeviceInfo(string deviceName, double latitiude, double longitude)
    {
        // {
        //      "description": "Clima Boot Telemetry",
        //      "eventId": 109,
        //      "timestamp": "2024-05-20T22:25:15.862Z",
        //      "measurements": {
        //          "lat": 34.2277472,
        //          "long": -118.2273136
        //      }
        // }
        var cloudEvent = new CloudEvent
        {
            Description = "Clima Position Telemetry",
            Timestamp = DateTime.UtcNow,
            EventId = 109,
        };

        cloudEvent.Measurements.Add("device_name", deviceName);
        cloudEvent.Measurements.Add("lat", latitiude);
        cloudEvent.Measurements.Add("long", longitude);
        
        SendEvent(cloudEvent);
    }
    public void LogWarning(string message)
    {
        SendLog(message, "warning");
    }

    public void LogMessage(string message)
    {
        SendLog(message, "information");
    }

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