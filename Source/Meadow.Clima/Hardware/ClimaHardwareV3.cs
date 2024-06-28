using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Environmental;
using Meadow.Foundation.Sensors.Gnss;
using Meadow.Foundation.Sensors.Weather;
using Meadow.Hardware;
using Meadow.Peripherals.Leds;
using Meadow.Peripherals.Sensors.Environmental;
using Meadow.Peripherals.Sensors.Weather;
using System;

namespace Meadow.Devices;

/// <summary>
/// Represents the Clima v3.x hardware
/// </summary>
public class ClimaHardwareV3 : ClimaHardwareBase
{
    /// <summary>
    /// The Meadow CCM device
    /// </summary>
    protected readonly IF7CoreComputeMeadowDevice _device;

    private Scd40? _environmentalSensor;
    private ICO2ConcentrationSensor? _co2ConcentrationSensor;

    /// <inheritdoc/>
    public sealed override II2cBus I2cBus { get; }

    /// <summary>
    /// The SCD40 environmental sensor on the Clima board
    /// </summary>
    public Scd40? EnvironmentalSensor => GetEnvironmentalSensor();

    /// <inheritdoc/>
    public override ICO2ConcentrationSensor? CO2ConcentrationSensor => GetCO2ConcentrationSensor();

    /// <summary>
    /// The MCP23008 IO expander that contains the Clima hardware version 
    /// </summary>
    public Mcp23008 McpVersion { get; protected set; }

    /// <inheritdoc/>
    public override string RevisionString => "v3.x";

    /// <summary>
    /// Analog inputs to measure Solar voltage has Resistor Divider with R1 = 1000 Ohm, R2 = 680 Ohm
    /// Measured analogue voltage needs to be scaled to RETURN actual input voltage
    ///     Input Voltage = AIN / Resistor Divider
    /// </summary>
    protected const double SolarVoltageResistorDivider = 680.0 / (1000.0 + 680.0);

    /// <summary>
    /// Analog inputs to measure Solar voltage has Resistor Divider with R1 = 1000 Ohm, R2 = 680 Ohm
    /// Measured analogue voltage needs to be scaled to RETURN actual input voltage
    ///     Input Voltage = AIN / Resistor Divider
    /// </summary>
    protected const double BatteryVoltageResistorDivider = 2000.0 / (1000.0 + 2000.0);

    /// <summary>
    /// Create a new ClimaHardwareV3 object
    /// </summary>
    /// <param name="device">The meadow device</param>
    /// <param name="i2cBus">The I2C bus</param>
    /// <param name="mcpVersion">The Mcp23008 used to read version information</param>
    public ClimaHardwareV3(IF7CoreComputeMeadowDevice device, II2cBus i2cBus, Mcp23008 mcpVersion)
        : base(device)
    {
        McpVersion = mcpVersion;

        _device = device;

        I2cBus = i2cBus;

        // See hack in Meadow.Core\source\implementations\f7\Meadow.F7\Devices\DeviceChannelManager.cs
        // Must initialise any PWM based I/O first
        GetRgbPwmLed();

        try
        {
            Logger?.Trace("Instantiating Solar Voltage Input");
            SolarVoltageInput = device.Pins.A02.CreateAnalogInputPort(5,
                AnalogInputPort.DefaultSampleInterval,
                AnalogInputPort.DefaultReferenceVoltage / SolarVoltageResistorDivider);
            Logger?.Trace("Solar Voltage Input up");
        }
        catch (Exception ex)
        {
            Logger?.Error($"Unable to create the Solar Voltage Input: {ex.Message}");
        }

        try
        {
            Logger?.Trace("Instantiating Battery Voltage Input");
            BatteryVoltageInput = device.Pins.A04.CreateAnalogInputPort(5,
                AnalogInputPort.DefaultSampleInterval,
                AnalogInputPort.DefaultReferenceVoltage / BatteryVoltageResistorDivider);

            Logger?.Trace("Battery Voltage Input up");
        }
        catch (Exception ex)
        {
            Logger?.Error($"Unable to create the Battery Voltage Input: {ex.Message}");
        }
    }

    private Scd40? GetEnvironmentalSensor()
    {
        if (_environmentalSensor == null)
        {
            InitializeScd40();
        }

        return _environmentalSensor;
    }

    private ICO2ConcentrationSensor? GetCO2ConcentrationSensor()
    {
        if (_co2ConcentrationSensor == null)
        {
            InitializeScd40();
        }

        return _co2ConcentrationSensor;
    }

    private void InitializeScd40()
    {
        try
        {
            Logger?.Trace("Instantiating environmental sensor");
            var scd = new Scd40(I2cBus, (byte)Scd40.Addresses.Default);
            _co2ConcentrationSensor = scd;
            _environmentalSensor = scd;
            Resolver.SensorService.RegisterSensor(scd);
            Logger?.Trace("Environmental sensor up");
        }
        catch (Exception ex)
        {
            Logger?.Error($"Unable to create the SCD40 Environmental Sensor: {ex.Message}");
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
                    redPwmPin: _device.Pins.D09,
                    greenPwmPin: _device.Pins.D10,
                    bluePwmPin: _device.Pins.D11,
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
                _gnss = new NeoM8(_device, _device.PlatformOS.GetSerialPortName("COM4")!, _device.Pins.D05, _device.Pins.A03);
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
                _rainGauge = new SwitchingRainGauge(_device.Pins.D16);
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