using Meadow;
using Meadow.Devices;
using Meadow.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clima_Simulation;
// Change F7FeatherV2 to F7FeatherV1 if using Feather V1 Meadow boards
// Change to F7CoreComputeV2 for Project Lab V3.x
public class MeadowApp : App<F7CoreComputeV2>
{
    public override async Task Initialize()
    {
        Resolver.Log.Info($"Initializing...");

        var cloudLogger = new CloudLogger();
        Resolver.Log.AddProvider(cloudLogger);
        Resolver.Services.Add(cloudLogger);

        var count = 1;
        var r = new Random();

        while (true)
        {
            // send a cloud log
            Resolver.Log.Info($"log loop {count++}");

            // send a cloud event
            var cl = Resolver.Services.Get<CloudLogger>();
            cl.LogEvent(100, "clima conditions", new Dictionary<string, object>()
            {
                { "Temperature", r.Next(20, 25) },
                { "Rain", r.Next(0, 5) },
                { "Light", r.Next(1000, 2000) },
                { "SolarVoltage", r.Next(3, 5) },
                { "Humidity", r.Next(80, 110) },
                { "WindSpeed", r.Next(10, 15) },
                { "WindDirection", r.Next(0, 360) },
                { "Pressure", r.Next(1, 5) },
                { "Co2Level", r.Next(750, 1000) },
                { "BatteryVoltage", r.Next(3,5) },
            });

            await Task.Delay(60 * 1000);
        }
    }
}