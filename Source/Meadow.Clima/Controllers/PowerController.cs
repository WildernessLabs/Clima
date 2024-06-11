using Meadow;
using Meadow.Devices;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace Clima_Demo;

public class PowerController
{
    private readonly IClimaHardware clima;

    public event EventHandler<bool>? SolarVoltageWarning;
    public event EventHandler<bool>? BatteryVoltageWarning;

    private bool inBatteryWarningState = false;
    private bool inSolarWarningState = false;

    public bool LogPowerData { get; set; } = false;
    public TimeSpan UpdateInterval { get; } = TimeSpan.FromSeconds(5);
    public Voltage LowBatteryWarningLevel { get; } = 3.3.Volts();
    public Voltage LowSolarWarningLevel { get; } = 3.0.Volts();
    public Voltage WarningDeadband { get; } = 0.25.Volts();

    public PowerController(
        IClimaHardware clima)
    {
        this.clima = clima;

        Initialize();
    }

    private void Initialize()
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

    public async Task<PowerData> GetPowerData()
    {
        return new PowerData
        {
            BatteryVoltage = clima.BatteryVoltageInput?.Voltage ?? null,
            SolarVoltage = clima.SolarVoltageInput?.Voltage ?? null,
        };
    }

    public void TimedSleep(TimeSpan duration)
    {
        Resolver.Log.Info("Going to sleep...");

        Resolver.Device.PlatformOS.Sleep(duration);

        Resolver.Log.Info("PowerController completed sleep");
    }

    private void SolarVoltageUpdated(object sender, IChangeResult<Voltage> e)
    {
        Resolver.Log.InfoIf(LogPowerData, $"Solar Voltage:   {e.New.Volts:0.#} volts");

        if (e.New < LowSolarWarningLevel)
        {
            if (!inSolarWarningState)
            {
                SolarVoltageWarning?.Invoke(this, true);

                inSolarWarningState = true;
            }
        }
        else
        {
            if (inSolarWarningState)
            {
                var resetVoltage = LowBatteryWarningLevel + WarningDeadband;

                if (e.New > resetVoltage)
                {
                    SolarVoltageWarning?.Invoke(this, false);

                    inSolarWarningState = false;
                }
            }
        }
    }

    private void BatteryVoltageUpdated(object sender, IChangeResult<Voltage> e)
    {
        Resolver.Log.InfoIf(LogPowerData, $"Battery Voltage:   {e.New.Volts:0.#} volts");

        if (e.New < LowBatteryWarningLevel)
        {
            if (!inBatteryWarningState)
            {
                BatteryVoltageWarning?.Invoke(this, true);

                inBatteryWarningState = true;
            }
        }
        else
        {
            if (inBatteryWarningState)
            {
                var resetVoltage = LowBatteryWarningLevel + WarningDeadband;

                if (e.New > resetVoltage)
                {
                    BatteryVoltageWarning?.Invoke(this, false);

                    inBatteryWarningState = false;
                }
            }
        }
    }
}
