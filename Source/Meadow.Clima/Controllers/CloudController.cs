using Meadow;
using Meadow.Cloud;
using System;

namespace Clima_Demo;

public class CloudController
{
    public void LogAppStartupAfterCrash()
    {
        LogEvent(CloudEventIds.DeviceStarted, $"Device restarted after crash");
    }

    public void LogAppStartup(string hardwareRevision)
    {
        LogEvent(CloudEventIds.DeviceStarted, $"Device started (hardware {hardwareRevision})");
    }

    private void LogEvent(CloudEventIds eventId, string message)
    {
        if (Resolver.MeadowCloudService == null) { return; }

        if (!Resolver.MeadowCloudService.IsEnabled)
        {
            Resolver.Log.Warn($"CLOUD INTEGRATION IS DISABLED");
            return;
        }

        Resolver.MeadowCloudService.SendEvent(
            new CloudEvent
            {
                EventId = (int)eventId,
                Description = message,
                Timestamp = DateTime.UtcNow,
            });
    }
}
