using Meadow.Peripherals.Leds;
using System;

namespace Clima_Demo;

public class NotificationController
{
    [Flags]
    public enum Warnings
    {
        None = 0,
        NetworkDisconnected = 1 << 0,
        SolarLoadLow = 1 << 1,
        BatteryLow = 1 << 2,
    }

    private readonly IRgbPwmLed? rgbLed;
    private Warnings activeWarnings = Warnings.None;

    public NotificationController(IRgbPwmLed? rgbLed)
    {
        this.rgbLed = rgbLed;
    }

    public void SystemStarting()
    {
        rgbLed?.SetColor(RgbLedColors.Red);
    }

    public void SystemUp()
    {
        ReportWarnings();
    }

    public void SetWarning(Warnings warning)
    {
        activeWarnings |= warning;
        ReportWarnings();
    }

    public void ClearWarning(Warnings warning)
    {
        activeWarnings &= ~warning;
        ReportWarnings();
    }

    private void ReportWarnings()
    {
        if (activeWarnings != Warnings.None)
        {
            rgbLed?.SetColor(RgbLedColors.Yellow);
        }
        else
        {
            rgbLed?.SetColor(RgbLedColors.Green);
        }
    }
}
