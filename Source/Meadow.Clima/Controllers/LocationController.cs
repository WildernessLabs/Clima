using Meadow.Devices.Clima.Hardware;
using Meadow.Peripherals.Sensors.Location.Gnss;
using Meadow.Units;
using System;
using System.Threading;

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

    private ManualResetEvent positionReceived = new ManualResetEvent(false);

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
        }
    }

    /// <summary>
    /// Gets the current geographic position as a <see cref="GeographicCoordinate"/>.
    /// </summary>
    /// <value>
    /// The geographic position, including latitude, longitude, and altitude, if available.
    /// </value>
    /// <remarks>
    /// This property is updated when valid GNSS data is received. It represents the last known position
    /// and remains unchanged until new valid data is processed.
    /// </remarks>
    public GeographicCoordinate? Position { get; private set; } = default;

    private void OnGnssDataReceived(object sender, IGnssResult e)
    {
        if (e is GnssPositionInfo pi)
        {
            if (pi.IsValid && pi.Position != null)
            {
                // remember our position
                Position = pi.Position;
                // we only need one position fix - weather stations don't move
                Resolver.Log.InfoIf(LogData, $"GNSS Position: lat: [{pi.Position.Latitude}], long: [{pi.Position.Longitude}]");
                positionReceived.Set();
                PositionReceived?.Invoke(this, pi);
                StopUpdating();
            }
        }
    }

    /// <summary>
    /// Starts the GNSS sensor to begin updating location data.
    /// </summary>
    /// <remarks>
    /// This method invokes the <see cref="IGnssSensor.StartUpdating"/> method on the associated GNSS sensor,
    /// if it is available, to start receiving GNSS data updates.
    /// </remarks>
    public void StartUpdating(bool forced = false)
    {
        // start updating if forced to find new data or we don;t have current location
        if (forced || !positionReceived.WaitOne(0))
        {
            gnss?.StartUpdating();
        };
    }

    /// <summary>
    /// Stops the GNSS sensor from updating location data.
    /// </summary>
    /// <remarks>
    /// This method halts the GNSS data updates by invoking the <see cref="IGnssSensor.StopUpdating"/> 
    /// method on the associated GNSS sensor, if it is available.
    /// </remarks>
    public void StopUpdating()
    {
        // stop listening to data arriving from GNSS
        gnss?.StopUpdating();

        // TODO: can we tell GNSS sensor to stop calculating GPS location and stop sending data to reduce power consumption?
    }
}
