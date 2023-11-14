using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Environmental;
using Meadow.Foundation.Sensors.Gnss;
using Meadow.Foundation.Sensors.Weather;
using Meadow.Hardware;

#nullable enable

namespace Meadow.Devices
{
    public interface IClimaHardware
    {
        /// <summary>
        /// The I2C Bus
        /// </summary>
        public II2cBus I2cBus { get; }

        /// <summary>
        /// The BME688 atmospheric sensor on the Clima board
        /// </summary>
        public Bme688? AtmosphericSensor { get; }

        /// <summary>
        /// The SCD40 environmental sensor on the Clima board
        /// </summary>
        public Scd40? EnvironmentalSensor { get; }

        /// <summary>
        /// The Neo GNSS sensor
        /// </summary>
        public NeoM8? Gnss { get; }

        /// <summary>
        /// The Wind Vane on the Clima board
        /// </summary>
        public WindVane? WindVane { get; }

        /// <summary>
        /// The Switching Rain Gauge on the Clima board
        /// </summary>
        public SwitchingRainGauge? RainGauge { get; }

        /// <summary>
        /// The Switching Anemometer on the Clima board
        /// </summary>
        public SwitchingAnemometer? Anemometer { get; }

        /// <summary>
        /// The Solar Voltage Input on the Clima board
        /// </summary>
        public IAnalogInputPort? SolarVoltageInput { get; }

        /// <summary>
        /// The Battery Voltage Input on the Clima board
        /// </summary>
        public IAnalogInputPort? BatteryVoltageInput { get; }

        /// <summary>
        /// Gets the RGB PWM LED
        /// </summary>
        public RgbPwmLed? ColorLed { get; }

        public string RevisionString { get; }
    }
}