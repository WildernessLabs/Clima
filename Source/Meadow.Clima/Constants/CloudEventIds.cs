namespace Meadow.Devices.Clima.Constants;

/// <summary>
/// Enumeration of cloud event IDs.
/// </summary>
public enum CloudEventIds
{
    /// <summary>
    /// Event ID for when the device starts.
    /// </summary>
    DeviceStarted = 100,

    /// <summary>
    /// Event ID for telemetry data.
    /// </summary>
    Telemetry = 110,

    /// <summary>
    /// Event ID for booting from a crash.
    /// </summary>
    BootFromCrash = 200
}