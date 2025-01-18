using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Gnss;
using Meadow.Hardware;
using Meadow.Logging;
using Meadow.Peripherals.Leds;
using Meadow.Peripherals.Sensors;
using Meadow.Peripherals.Sensors.Atmospheric;
using Meadow.Peripherals.Sensors.Environmental;
using Meadow.Peripherals.Sensors.Weather;
using System;

namespace Meadow.Devices.Clima.Hardware;

/// <summary>
/// Contains common elements of Clima hardware
/// </summary>
public abstract class ClimaHardwareBase : IClimaHardware
{
    private IConnector?[]? _connectors;
    private Bme688? _atmosphericSensor;
    private ITemperatureSensor? _temperatureSensor;
    private IHumiditySensor? _humiditySensor;
    private IBarometricPressureSensor? _barometricPressureSensor;
    private IGasResistanceSensor? _gasResistanceSensor;
    internal IWindVane? _windVane;
    internal IRainGauge? _rainGauge;
    internal IAnemometer? _anemometer;
    internal IRgbPwmLed? _rgbLed;
    internal NeoM8? _gnss;

    /// <summary>
    /// Get a reference to Meadow Logger
    /// </summary>
    protected Logger? Logger { get; } = Resolver.Log;

    /// <inheritdoc/>
    public abstract II2cBus I2cBus { get; }

    /// <inheritdoc/>
    public Bme688? AtmosphericSensor => GetAtmosphericSensor();

    /// <inheritdoc/>
    public ITemperatureSensor? TemperatureSensor => GetTemperatureSensor();

    /// <inheritdoc/>
    public IHumiditySensor? HumiditySensor => GetHumiditySensor();

    /// <inheritdoc/>
    public IBarometricPressureSensor? BarometricPressureSensor => GetBarometricPressureSensor();

    /// <inheritdoc/>
    public IGasResistanceSensor? GasResistanceSensor => GetGasResistanceSensor();

    /// <inheritdoc/>
    public virtual ICO2ConcentrationSensor? CO2ConcentrationSensor => throw new NotImplementedException();

    /// <inheritdoc/>
    public IWindVane? WindVane => GetWindVane();

    /// <inheritdoc/>
    public IRainGauge? RainGauge => GetRainGauge();

    /// <inheritdoc/>
    public IAnemometer? Anemometer => GetAnemometer();

    /// <inheritdoc/>
    public IAnalogInputPort? SolarVoltageInput { get; protected set; }

    /// <inheritdoc/>
    public IAnalogInputPort? BatteryVoltageInput { get; protected set; }

    /// <inheritdoc/>
    public IRgbPwmLed? RgbLed => GetRgbPwmLed();

    /// <inheritdoc/>
    public abstract string RevisionString { get; }

    /// <summary>
    /// The Neo GNSS sensor
    /// </summary>
    public NeoM8? Gnss => GetNeoM8();

    /// <inheritdoc/>
    public I2cConnector? Qwiic => (I2cConnector?)Connectors[0];

    /// <inheritdoc/>
    public IMeadowDevice ComputeModule { get; }

    internal ClimaHardwareBase(IMeadowDevice device)
    {
        ComputeModule = device;
    }

    internal virtual I2cConnector? CreateQwiicConnector()
    {
        return null;
    }

    /// <summary>
    /// Collection of connectors on the Clima board
    /// </summary>
    public IConnector?[] Connectors
    {
        get
        {
            if (_connectors == null)
            {
                _connectors = new IConnector[1];
                _connectors[0] = CreateQwiicConnector();
            }

            return _connectors;
        }
    }

    private Bme688? GetAtmosphericSensor()
    {
        if (_atmosphericSensor == null)
        {
            InitializeBme688();
        }

        return _atmosphericSensor;
    }

    private ITemperatureSensor? GetTemperatureSensor()
    {
        if (_temperatureSensor == null)
        {
            InitializeBme688();
        }

        return _temperatureSensor;
    }

    private IHumiditySensor? GetHumiditySensor()
    {
        if (_humiditySensor == null)
        {
            InitializeBme688();
        }

        return _humiditySensor;
    }

    private IBarometricPressureSensor? GetBarometricPressureSensor()
    {
        if (_barometricPressureSensor == null)
        {
            InitializeBme688();
        }

        return _barometricPressureSensor;
    }

    private IGasResistanceSensor? GetGasResistanceSensor()
    {
        if (_gasResistanceSensor == null)
        {
            InitializeBme688();
        }

        return _gasResistanceSensor;
    }

    /// <summary>
    /// Get the Wind Vane on the Clima board
    /// </summary>
    protected abstract IWindVane? GetWindVane();

    /// <summary>
    /// Get the Rain Gauge on the Clima board
    /// </summary>
    protected abstract IRainGauge? GetRainGauge();

    /// <summary>
    /// Get the Anemometer on the Clima board
    /// </summary>
    protected abstract IAnemometer? GetAnemometer();

    /// <summary>
    /// Get the RGB LED on the Clima board
    /// </summary>
    protected abstract IRgbPwmLed? GetRgbPwmLed();

    /// <summary>
    /// Get the Neo GNSS sensor
    /// </summary>
    protected abstract NeoM8? GetNeoM8();

    private void InitializeBme688()
    {
        try
        {
            Logger?.Trace("Instantiating atmospheric sensor");
            var bme = new Bme688(I2cBus, (byte)Bme68x.Addresses.Address_0x76);
            _atmosphericSensor = bme;
            _temperatureSensor = bme;
            _humiditySensor = bme;
            _barometricPressureSensor = bme;
            _gasResistanceSensor = bme;
            Resolver.SensorService.RegisterSensor(_atmosphericSensor);
            Logger?.Trace("Atmospheric sensor up");
        }
        catch (Exception ex)
        {
            Logger?.Error($"Unable to create the BME688 atmospheric sensor: {ex.Message}");
        }
    }
}