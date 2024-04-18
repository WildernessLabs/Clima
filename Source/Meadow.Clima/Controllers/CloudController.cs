using Meadow;
using Meadow.Cloud;
using Meadow.Peripherals.Leds;
using System;

namespace Clima_Demo;

public class NotificationController
{
    private readonly IRgbPwmLed? rgbLed;

    public NotificationController(IRgbPwmLed? rgbLed)
    {
        this.rgbLed = rgbLed;
    }

    public void Starting()
    {
        rgbLed?.SetColor(RgbLedColors.Red);
    }

    public void NetworkConnected()
    {
        rgbLed?.SetColor(RgbLedColors.Green);
    }

    public void NetworkDisconnected()
    {
        rgbLed?.SetColor(RgbLedColors.Yellow);
    }
}

public class CloudController
{
    public void LogEvent(CloudEventIds eventId, string message)
    {
        if (Resolver.MeadowCloudService == null
            || !Resolver.MeadowCloudService.IsEnabled) return;

        Resolver.MeadowCloudService.SendEvent(
            new CloudEvent
            {
                EventId = (int)eventId,
                Description = message,
                Timestamp = DateTime.UtcNow,
            });
    }
}
