using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Gnss;
using Meadow.Foundation.Sensors.Weather;
using Meadow.Hardware;
using Meadow.Peripherals.Leds;
using Meadow.Peripherals.Sensors.Weather;
using System;

namespace Meadow.Devices;

/// <summary>
/// Represents the Clima v2.x hardware
/// </summary>
public class ClimaHardwareV2 : ClimaHardwareBase
{
    private readonly IF7FeatherMeadowDevice _device;

    /// <inheritdoc/>
    public sealed override II2cBus I2cBus { get; }

    /// <inheritdoc/>
    public override string RevisionString => "v2.x";

    /// <summary>
    /// Create a new ClimaHardwareV2 object
    /// </summary>
    /// <param name="device">The meadow device</param>
    /// <param name="i2cBus">The I2C bus</param>
    public ClimaHardwareV2(IF7FeatherMeadowDevice device, II2cBus i2cBus)
        : base(device)
    {
        _device = device;

        I2cBus = i2cBus;

        // See hack in Meadow.Core\source\implementations\f7\Meadow.F7\Devices\DeviceChannelManager.cs
        // Must initialise any PWM based I/O first
        GetRgbPwmLed();

        try
        {
            Logger?.Trace("Instantiating Solar Voltage Input");
            SolarVoltageInput = device.Pins.A02.CreateAnalogInputPort(5);
            Logger?.Trace("Solar Voltage Input up");
        }
        catch (Exception ex)
        {
            Logger?.Error($"Unable to create the Solar Voltage Input: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    protected override IRgbPwmLed? GetRgbPwmLed()
    {
        if (_rgbLed == null)
        {
            try
            {
                Logger?.Trace("Instantiating RGB LED");
                _rgbLed = new RgbPwmLed(
                    redPwmPin: _device.Pins.OnboardLedRed,
                    greenPwmPin: _device.Pins.OnboardLedGreen,
                    bluePwmPin: _device.Pins.OnboardLedBlue,
                    CommonType.CommonAnode);
                Logger?.Trace("RGB LED up");
            }
            catch (Exception ex)
            {
                Logger?.Error($"Unable to create the RGB LED: {ex.Message}");
            }
        }

        return _rgbLed;
    }

    /// <inheritdoc/>
    protected override NeoM8? GetNeoM8()
    {
        if (_gnss == null)
        {
            try
            {
                Logger?.Trace("Instantiating GNSS");
                _gnss = new NeoM8(_device, _device.PlatformOS.GetSerialPortName("COM4")!, null, null);
                Logger?.Trace("GNSS initialized");
            }
            catch (Exception e)
            {
                Logger?.Error($"Err initializing GNSS: {e.Message}");
            }
        }
        return _gnss;
    }

    /// <inheritdoc/>
    protected override IWindVane? GetWindVane()
    {
        if (_windVane == null)
        {
            try
            {
                Logger?.Trace("Instantiating Wind Vane");
                _windVane = new WindVane(_device.Pins.A00);
                Resolver.SensorService.RegisterSensor(_windVane);
                Logger?.Trace("Wind Vane up");
            }
            catch (Exception ex)
            {
                Logger?.Error($"Unable to create the Wind Vane: {ex.Message}");
            }
        }
        return _windVane;
    }

    /// <inheritdoc/>
    protected override IRainGauge? GetRainGauge()
    {
        if (_rainGauge == null)
        {
            try
            {
                Logger?.Trace("Instantiating Rain Gauge");
                _rainGauge = new SwitchingRainGauge(_device.Pins.D11);
                Resolver.SensorService.RegisterSensor(_rainGauge);
                Logger?.Trace("Rain Gauge up");
            }
            catch (Exception ex)
            {
                Logger?.Error($"Unable to create the Rain Gauge: {ex.Message}");
            }
        }
        return _rainGauge;
    }

    /// <inheritdoc/>
    protected override IAnemometer? GetAnemometer()
    {
        if (_anemometer == null)
        {
            try
            {
                Logger?.Trace("Instantiating Anemometer");
                _anemometer = new SwitchingAnemometer(_device.Pins.A01);
                Resolver.SensorService.RegisterSensor(_anemometer);
                Logger?.Trace("Anemometer up");
            }
            catch (Exception ex)
            {
                Logger?.Error($"Unable to create the Anemometer: {ex.Message}");
            }
        }
        return _anemometer;
    }
}