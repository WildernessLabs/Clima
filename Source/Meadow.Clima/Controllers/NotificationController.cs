using Meadow;
using Meadow.Peripherals.Leds;

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
        Resolver.Log.Info("Network connected");
        rgbLed?.SetColor(RgbLedColors.Green);
    }

    public void NetworkDisconnected()
    {
        Resolver.Log.Info("Network disconnected");
        rgbLed?.SetColor(RgbLedColors.Yellow);
    }
}
