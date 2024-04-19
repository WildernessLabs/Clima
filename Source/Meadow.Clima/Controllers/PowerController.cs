using Meadow;
using Meadow.Devices;
using Meadow.Units;
using System;

namespace Clima_Demo;

public class PowerController
{
    public bool LogPowerData { get; set; } = false;
    public TimeSpan UpdateInterval { get; } = TimeSpan.FromSeconds(5);

    public PowerController(IClimaHardware clima)
    {
        if (clima.SolarVoltageInput is { } solarVoltage)
        {
            solarVoltage.Updated += SolarVoltageUpdated;
            solarVoltage.StartUpdating(UpdateInterval);
        }

        if (clima.BatteryVoltageInput is { } batteryVoltage)
        {
            batteryVoltage.Updated += BatteryVoltageUpdated;
            batteryVoltage.StartUpdating(UpdateInterval);
        }
    }

    private void SolarVoltageUpdated(object sender, IChangeResult<Voltage> e)
    {
        Resolver.Log.InfoIf(LogPowerData, $"Solar Voltage:   {e.New.Volts:0.#} volts");
    }

    private void BatteryVoltageUpdated(object sender, IChangeResult<Voltage> e)
    {
        Resolver.Log.InfoIf(LogPowerData, $"Battery Voltage: {e.New.Volts:0.#} volts");
    }
}
