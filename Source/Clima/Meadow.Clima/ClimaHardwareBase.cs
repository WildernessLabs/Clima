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
    public abstract class ClimaHardwareBase : IClimaHardware
    {
        /// <summary>
        /// Get a reference to Meadow Logger
        /// </summary>
        protected Logger? Logger { get; } = Resolver.Log;

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
        /// The Neo GNSS sensor on the Clima board
        /// </summary>
        public abstract NeoM8 Gnss { get; protected set; }

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
        public virtual string RevisionString { get; set; } = "unknown";

        internal ClimaHardwareBase()
        {
        }

        protected virtual void Initialize(IF7MeadowDevice device, II2cBus i2cBus)
        {
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
        }
    }
}