using Meadow.Peripherals.Leds;
using System;

namespace Meadow.Devices.Clima.Controllers;

/// <summary>
/// Controller for handling system notifications and warnings using an RGB LED.
/// </summary>
public class NotificationController
{
    /// <summary>
    /// Enum representing various warning states.
    /// </summary>
    [Flags]
    public enum Warnings
    {
        /// <summary>
        /// No warnings.
        /// </summary>
        None = 0,
        /// <summary>
        /// Network is disconnected.
        /// </summary>
        NetworkDisconnected = 1 << 0,
        /// <summary>
        /// Solar load is low.
        /// </summary>
        SolarLoadLow = 1 << 1,
        /// <summary>
        /// Battery is low.
        /// </summary>
        BatteryLow = 1 << 2,
    }

    /// <summary>
    /// Enum representing various system statuses.
    /// </summary>
    public enum SystemStatus
    {
        /// <summary>
        /// System is in low power mode.
        /// </summary>
        LowPower,
        /// <summary>
        /// System is starting.
        /// </summary>
        Starting,
        /// <summary>
        /// System is searching for a network.
        /// </summary>
        SearchingForNetwork,
        /// <summary>
        /// System is connected to the network.
        /// </summary>
        NetworkConnected,
        /// <summary>
        /// System is connecting to the cloud.
        /// </summary>
        ConnectingToCloud,
        /// <summary>
        /// System is connected to the cloud.
        /// </summary>
        Connected,
    }

    private readonly IRgbPwmLed? rgbLed;
    private Warnings activeWarnings = Warnings.None;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationController"/> class.
    /// </summary>
    /// <param name="rgbLed">The RGB LED to use for notifications.</param>
    public NotificationController(IRgbPwmLed? rgbLed)
    {
        this.rgbLed = rgbLed;
    }

    /// <summary>
    /// Sets the system status and updates the RGB LED accordingly.
    /// </summary>
    /// <param name="status">The system status to set.</param>
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

    /// <summary>
    /// Sets a warning and updates the RGB LED accordingly.
    /// </summary>
    /// <param name="warning">The warning to set.</param>
    public void SetWarning(Warnings warning)
    {
        activeWarnings |= warning;
        ReportWarnings();
    }

    /// <summary>
    /// Clears a warning and updates the RGB LED accordingly.
    /// </summary>
    /// <param name="warning">The warning to clear.</param>
    public void ClearWarning(Warnings warning)
    {
        activeWarnings &= ~warning;
        ReportWarnings();
    }

    /// <summary>
    /// Reports the current warnings by updating the RGB LED.
    /// </summary>
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
