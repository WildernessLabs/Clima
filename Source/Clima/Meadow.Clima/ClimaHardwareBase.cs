using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Environmental;
using Meadow.Foundation.Sensors.Gnss;
using Meadow.Foundation.Sensors.Weather;
using Meadow.Hardware;
using Meadow.Logging;
using System;

namespace Meadow.Devices
{
    abstract internal class ClimaHardwareBase : IClimaHardware
    {
        /// <summary>
        /// Get a reference to Meadow Logger
        /// </summary>
        protected Logger? Logger { get; } = Resolver.Log;

        /// <summary>
        /// Gets the I2C Bus
        /// </summary>
        public II2cBus I2cBus { get; }

        /// <summary>
        /// Gets the BME688 environmental sensor on the Clima board
        /// </summary>
        public Bme688? AtmosphericSensor { get; }

        /// <summary>
        /// The BME688 environmental sensor on the Clima board
        /// </summary>
        public Scd40? EnvironmentalSensor { get; }

        /// <summary>
        /// The Neo GNSS sensor on the Clima board
        /// </summary>
        public abstract NeoM8 Gnss { get; protected set; }

        /// <summary>
        /// The Wind Vane on the Clima board
        /// </summary>
        WindVane? WindVane { get; }

        /// <summary>
        /// The Switching Rain Gauge on the Clima board
        /// </summary>
        SwitchingRainGauge? RainGauge { get; }

        /// <summary>
        /// The Switching Anemometer on the Clima board
        /// </summary>
        SwitchingAnemometer? Anemometer { get; }

        /// <summary>
        /// The Solar Voltage Input on the Clima board
        /// </summary>
        IAnalogInputPort? SolarVoltageInput { get; }

        /// <summary>
        /// Gets the ProjectLab board hardware revision
        /// </summary>
        public virtual string RevisionString { get; set; } = "unknown";

        internal ClimaHardwareBase(IF7FeatherMeadowDevice device, II2cBus i2cBus)
        {
            SpiBus = spiBus;
            I2cBus = i2cBus;

            try
            {
                Logger?.Trace("Instantiating atmospheric sensor");
                AtmosphericSensor = new Bme688(I2cBus, (byte)Bme688.Addresses.Address_0x76);
                Logger?.Trace("Environmental sensor up");
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
                RainGauge = new SwitchingRainGauge(device.Pins.D11);
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
        }
    }
}