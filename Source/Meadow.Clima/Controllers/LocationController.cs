using Meadow.Devices.Clima.Hardware;
using Meadow.Peripherals.Sensors.Location.Gnss;
using System;

namespace Meadow.Devices.Clima.Controllers;

/// <summary>
/// Controller for handling GNSS location data.
/// </summary>
public class LocationController
{
    private readonly IGnssSensor? gnss = null;

    /// <summary>
    /// Gets or sets a value indicating whether to log data.
    /// </summary>
    public bool LogData { get; set; } = false;

    /// <summary>
    /// Event that is triggered when a GNSS position is received.
    /// </summary>
    public event EventHandler<GnssPositionInfo>? PositionReceived = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocationController"/> class.
    /// </summary>
    /// <param name="clima">The Clima hardware interface.</param>
    public LocationController(IClimaHardware clima)
    {
        if (clima.Gnss is { } gnss)
        {
            this.gnss = gnss;
            this.gnss.GnssDataReceived += OnGnssDataReceived;
            this.gnss.StartUpdating();
        }
    }

    private void OnGnssDataReceived(object sender, IGnssResult e)
    {
        if (e is GnssPositionInfo pi)
        {
            if (pi.IsValid && pi.Position != null)
            {
                // we only need one position fix - weather stations don't move
                Resolver.Log.InfoIf(LogData, $"GNSS Position: lat: [{pi.Position.Latitude}], long: [{pi.Position.Longitude}]");
                PositionReceived?.Invoke(this, pi);
                gnss?.StopUpdating();
            }
        }
    }
}