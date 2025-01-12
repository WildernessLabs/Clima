using Meadow.Devices.Clima.Hardware;
using Meadow.Devices.Clima.Models;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace Meadow.Devices.Clima.Controllers;

/// <summary>
/// Controls the power management for the Clima device, including monitoring
/// solar and battery voltages and issuing warnings when levels are low.
/// </summary>
public class PowerController
{
    private readonly IClimaHardware clima;

    /// <summary>
    /// Event triggered when the solar voltage is below the warning level.
    /// </summary>
    public event EventHandler<bool>? SolarVoltageWarning;

    /// <summary>
    /// Event triggered when the battery voltage is below the warning level.
    /// </summary>
    public event EventHandler<bool>? BatteryVoltageWarning;

    private bool inBatteryWarningState = false;
    private bool inSolarWarningState = false;

    /// <summary>
    /// Gets or sets a value indicating whether power data should be logged.
    /// </summary>
    public bool LogPowerData { get; set; } = false;

    /// <summary>
    /// Gets the interval at which power data is updated.
    /// </summary>
    public TimeSpan UpdateInterval { get; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Gets the voltage level below which a battery warning is issued.
    /// </summary>
    public Voltage LowBatteryWarningLevel { get; } = 3.3.Volts();

    /// <summary>
    /// Gets the voltage level below which a solar warning is issued.
    /// </summary>
    public Voltage LowSolarWarningLevel { get; } = 3.0.Volts();

    /// <summary>
    /// Gets the voltage deadband for resetting warnings.
    /// </summary>
    public Voltage WarningDeadband { get; } = 0.25.Volts();

    /// <summary>
    /// Initializes a new instance of the <see cref="PowerController"/> class.
    /// </summary>
    /// <param name="clima">The Clima hardware interface.</param>
    public PowerController(IClimaHardware clima)
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

    /// <summary>
    /// Gets the current power data including battery and solar voltages.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the power data.</returns>
    public Task<PowerData> GetPowerData()
    {
        var data = new PowerData
        {
            BatteryVoltage = clima.BatteryVoltageInput?.Voltage ?? null,
            SolarVoltage = clima.SolarVoltageInput?.Voltage ?? null,
        };

        return new Task<PowerData>(() => data);
    }

    /// <summary>
    /// Puts the device to sleep for the specified duration.
    /// </summary>
    /// <param name="duration">The duration to sleep.</param>
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
