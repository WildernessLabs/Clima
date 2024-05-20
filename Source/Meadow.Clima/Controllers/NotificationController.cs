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

    public enum SystemStatus
    {
        LowPower,
        Starting,
        SearchingForNetwork,
        NetworkConnected,
        ConnectingToCloud,
        Connected,

    }

    private readonly IRgbPwmLed? rgbLed;
    private Warnings activeWarnings = Warnings.None;

    public NotificationController(IRgbPwmLed? rgbLed)
    {
        this.rgbLed = rgbLed;
    }

    public void SetSystemStatus(SystemStatus status)
    {
        switch (status)
        {
            case SystemStatus.LowPower:
                if (rgbLed != null)
                {
                    rgbLed.StopAnimation();
                    rgbLed.IsOn = false;
                }
                break;
            case SystemStatus.Starting:
                rgbLed?.SetColor(RgbLedColors.Red);
                break;
            case SystemStatus.SearchingForNetwork:
                rgbLed?.StartBlink(RgbLedColors.Red);
                break;
            case SystemStatus.NetworkConnected:
                rgbLed?.StopAnimation();
                rgbLed?.SetColor(RgbLedColors.Magenta);
                break;
            case SystemStatus.ConnectingToCloud:
                rgbLed?.StartBlink(RgbLedColors.Cyan);
                break;
            case SystemStatus.Connected:
                rgbLed?.StopAnimation();
                rgbLed?.SetColor(RgbLedColors.Green);
                break;

        }
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
