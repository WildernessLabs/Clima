using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Environmental;
using Meadow.Foundation.Sensors.Gnss;
using Meadow.Foundation.Sensors.Weather;
using Meadow.Hardware;
using Meadow.Logging;
using System;

#nullable enable

namespace Meadow.Devices
{
    public class ClimaHardwareV2 : IClimaHardware
    {
        /// <summary>
        /// The I2C Bus
        /// </summary>
        public II2cBus I2cBus { get; protected set; }

        /// <summary>
        /// The BME688 atmospheric sensor on the Clima board
        /// </summary>
        public Bme688? AtmosphericSensor { get; protected set; }

        /// <summary>
        /// The SCD40 environmental sensor on the Clima board
        /// </summary>
        public Scd40? EnvironmentalSensor => null;

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
        /// The Solar Voltage Input on the Clima board
        /// </summary>
        public IAnalogInputPort? BatteryVoltageInput { get; protected set; }

        /// <summary>
        /// Gets the RGB PWM LED
        /// </summary>
        public RgbPwmLed? ColorLed { get; set; }

        /// <summary>
        /// The Neo GNSS sensor
        /// </summary>
        public NeoM8? Gnss { get; protected set; }

        /// <summary>
        /// The revision string for the Clima board
        /// </summary>
        public string RevisionString => "v2.x";

        /// <summary>
        /// Get a reference to Meadow Logger
        /// </summary>
        protected Logger? Logger { get; } = Resolver.Log;

        /// <summary>
        /// Create a new ClimaHardwareV2 object
        /// </summary>
        /// <param name="device">The meadow device</param>
        /// <param name="i2cBus">The I2C bus</param>
        public ClimaHardwareV2(IF7FeatherMeadowDevice device, II2cBus i2cBus)
        {
            I2cBus = i2cBus;

            // See HACKin Meadow.Core\source\implementations\f7\Meadow.F7\Devices\DeviceChannelManager.cs
            // Must initialise any PWM based I/O first.
            try
            {
                Logger?.Trace("Instantiating RGB LED");
                ColorLed = new RgbPwmLed(device.Pins.OnboardLedRed, device.Pins.OnboardLedGreen, device.Pins.OnboardLedBlue, Peripherals.Leds.CommonType.CommonAnode);
                Logger?.Trace("RGB LED up");
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Unable to create the RGB LED: {ex.Message}");
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
                Resolver.Log?.Trace("Initializing GNSS");
                Gnss = new NeoM8(device, device.PlatformOS.GetSerialPortName("COM4"), null, null);
                Resolver.Log?.Trace("GNSS initialized");
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
                Resolver.Log.Error($"Unable to create the Wind Vane: {ex.Message}");
            }

            try
            {
                Logger?.Trace("Instantiating Rain Gauge");
                RainGauge = new SwitchingRainGauge(device.Pins.D11);
                Logger?.Trace("RainGauge up");
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Unable to create the Switching Rain Gauge: {ex.Message}");
            }

            try
            {
                Logger?.Trace("Instantiating Switching Anemometer");
                Anemometer = new SwitchingAnemometer(device.Pins.A01);
                Logger?.Trace("Switching Anemometer up");
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Unable to create the Switching Anemometer: {ex.Message}");
            }

            try
            {
                Logger?.Trace("Instantiating Solar Voltage Input");
                SolarVoltageInput = device.Pins.A02.CreateAnalogInputPort(5);
                Logger?.Trace("Solar Voltage Input up");
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Unable to create the Solar Voltage Input: {ex.Message}");
            }
        }
    }
}