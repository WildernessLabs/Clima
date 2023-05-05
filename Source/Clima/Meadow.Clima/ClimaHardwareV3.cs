using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Environmental;
using Meadow.Foundation.Sensors.Gnss;
using Meadow.Foundation.Sensors.Weather;
using Meadow.Hardware;
using Meadow.Logging;
using System;

namespace Meadow.Devices
{
    public class ClimaHardwareV3 : IClimaHardware
    {
        /// <summary>
        /// Gets the I2C Bus
        /// </summary>
        public II2cBus I2cBus { get; protected set; }

        /// <summary>
        /// Gets the BME688 environmental sensor on the Clima board
        /// </summary>
        public Bme688? AtmosphericSensor { get; protected set; }

        /// <summary>
        /// The BME688 environmental sensor on the Clima board
        /// </summary>
        public Scd40? EnvironmentalSensor { get; protected set; }

        /// <summary>
        /// The Wind Vane on the Clima board
        /// </summary>
        public WindVane? WindVane { get; protected set; }

        /// <summary>
        /// The Switching Rain Gauge on the Clima board
        /// </summary>
        public SwitchingRainGauge? RainGauge { get; protected set; }

        /// <summary>
        /// The Switching Anemometer on the Clima board
        /// </summary>
        public SwitchingAnemometer? Anemometer { get; protected set; }

        /// <summary>
        /// The Solar Voltage Input on the Clima board
        /// </summary>
        public IAnalogInputPort? SolarVoltageInput { get; protected set; }

        /// <summary>
        /// Gets the ProjectLab board hardware revision
        /// </summary>
        public RgbPwmLed ColorLed { get; set; }

        /// <summary>
        /// The MCP23008 IO expander that contains the ProjectLab hardware version 
        /// </summary>
        Mcp23008? Mcp_Version { get; set; }

        /// <summary>
        /// The Neo GNSS sensor
        /// </summary>
        public NeoM8 Gnss { get; protected set; }

        public string RevisionString => "v3.x";

        /// <summary>
        /// Get a reference to Meadow Logger
        /// </summary>
        protected Logger? Logger { get; } = Resolver.Log;

        public ClimaHardwareV3(IF7CoreComputeMeadowDevice device, II2cBus i2cBus)
        {
            I2cBus = i2cBus;

            try
            {
                Logger?.Trace("Instantiating Mcp Version");
                Mcp_Version = new Mcp23008(I2cBus, address: 0x27, resetPort: device.Pins.D02.CreateDigitalOutputPort());
                Logger?.Info("Mcp Version up");
            }
            catch (Exception e)
            {
                Logger?.Trace($"ERR creating the MCP that has version information: {e.Message}");
            }

            try
            {
                Logger?.Trace("Instantiating atmospheric sensor");
                AtmosphericSensor = new Bme688(I2cBus, (byte)Bme688.Addresses.Address_0x76);
                Logger?.Trace("Atmospheric sensor up");
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Unable to create the BME688 Environmental Sensor: {ex.Message}");
            }

            try
            {
                Logger?.Trace("Instantiating environmental sensor");
                EnvironmentalSensor = new Scd40(I2cBus, (byte)Scd40.Addresses.Default);
                Logger?.Trace("Environmental sensor up");
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Unable to create the SCD40 Environmental Sensor: {ex.Message}");
            }

            try
            {
                Resolver.Log.Debug("Initializing GNSS");
                Gnss = new NeoM8(device, device.PlatformOS.GetSerialPortName("COM4"), device.Pins.D05, device.Pins.A03);
                Resolver.Log.Debug("GNSS initialized");
            }
            catch (Exception e)
            {
                Resolver.Log.Error($"Err initializing GNSS: {e.Message}");
            }

            try
            {
                Logger?.Trace("Instantiating Wind Vane");
                WindVane = new WindVane(device.Pins.A00);
                Logger?.Trace("Wind Vane up");
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Unabled to create the Wind Vane: {ex.Message}");
            }

            try
            {
                Logger?.Trace("Instantiating Rain Gauge");
                RainGauge = new SwitchingRainGauge(device.Pins.D16);
                Logger?.Trace("RainGauge up");
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Unabled to create the Switching Rain Gauge: {ex.Message}");
            }

            try
            {
                Logger?.Trace("Instantiating Switching Anemometer");
                Anemometer = new SwitchingAnemometer(device.Pins.A01);
                Logger?.Trace("Switching Anemometer up");
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Unabled to create the Switching Anemometer: {ex.Message}");
            }

            try
            {
                Logger?.Trace("Instantiating Solar Voltage Input");
                SolarVoltageInput = device.Pins.A02.CreateAnalogInputPort(5);
                Logger?.Trace("Solar Voltage Input up");
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Unabled to create the Switching Anemometer: {ex.Message}");
            }

            try
            {
                Logger?.Trace("Instantiating RGB LED");
                ColorLed = new RgbPwmLed(device.Pins.D09, device.Pins.D10, device.Pins.D11, Peripherals.Leds.CommonType.CommonAnode);
                Logger?.Trace("RGB LED up");
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Unabled to create the RGB LED: {ex.Message}");
            }
        }
    }
}