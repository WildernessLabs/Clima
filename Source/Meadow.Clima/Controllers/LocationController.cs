using Meadow;
using Meadow.Devices;
using Meadow.Peripherals.Sensors.Location.Gnss;

namespace Clima_Demo;

public class LocationController
{
    public bool LogData { get; set; } = false;

    public LocationController(IClimaHardware clima)
    {
        if (clima.Gnss is { } gnss)
        {
            //gnss.GsaReceived += GnssGsaReceived;
            //gnss.GsvReceived += GnssGsvReceived;
            //gnss.VtgReceived += GnssVtgReceived;
            gnss.RmcReceived += GnssRmcReceived;
            gnss.GllReceived += GnssGllReceived;
            gnss.StartUpdating();
        }

    }
    private void GnssGsaReceived(object _, ActiveSatellites e)
    {
        if (e.SatellitesUsedForFix is { } sats)
        {
            Resolver.Log.Info($"Number of active satellites: {sats.Length}");
        }
    }

    private void GnssGsvReceived(object _, SatellitesInView e)
    {
        Resolver.Log.Info($"Satellites in view: {e.Satellites.Length}");
    }

    private void GnssVtgReceived(object _, CourseOverGround e)
    {
        if (e is { } cv)
        {
            Resolver.Log.Info($"{cv}");
        };
    }

    private void GnssRmcReceived(object _, GnssPositionInfo e)
    {
        if (e.Position is not null)
        {
            Resolver.Log.InfoIf(LogData, $"GNSS Position: lat: [{e.Position.Latitude}], long: [{e.Position.Longitude}]");
        }
    }

    private void GnssGllReceived(object _, GnssPositionInfo e)
    {
        if (e.Position is not null)
        {
            Resolver.Log.InfoIf(LogData, $"GNSS Position: lat: [{e.Position.Latitude}], long: [{e.Position.Longitude}]");
        }
    }
}
