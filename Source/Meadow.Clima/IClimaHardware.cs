using Meadow.Foundation.Sensors.Gnss;
using Meadow.Hardware;
using Meadow.Peripherals.Leds;
using Meadow.Peripherals.Sensors;
using Meadow.Peripherals.Sensors.Atmospheric;
using Meadow.Peripherals.Sensors.Environmental;
using Meadow.Peripherals.Sensors.Weather;

namespace Meadow.Devices
{
    /// <summary>
    /// Contract for the Clima hardware definitions
    /// </summary>
    public interface IClimaHardware
    {
        /// <summary>
        /// The I2C Bus
        /// </summary>
        public II2cBus I2cBus { get; }

        /// <summary>
        /// Gets the ITemperatureSensor on the Clima board
        /// </summary>
        public ITemperatureSensor? TemperatureSensor { get; }

        /// <summary>
        /// Gets the IHumiditySensor on the Clima board
        /// </summary>
        public IHumiditySensor? HumiditySensor { get; }

        /// <summary>
        /// Gets the IBarometricPressureSensor on the Clima board
        /// </summary>
        public IBarometricPressureSensor? BarometricPressureSensor { get; }

        /// <summary>
        /// Gets the IGasResistanceSensor on the Clima board
        /// </summary>
        public IGasResistanceSensor? GasResistanceSensor { get; }

        /// <summary>
        /// Gets the ICO2ConcentrationSensor on the Clima board
        /// </summary>
        public ICO2ConcentrationSensor? CO2ConcentrationSensor { get; }

        /// <summary>
        /// The Neo GNSS sensor
        /// </summary>
        public NeoM8? Gnss { get; }

        /// <summary>
        /// The Wind Vane on the Clima board 
        /// </summary>
        public IWindVane? WindVane { get; }

        /// <summary>
        /// The Switching Rain Gauge on the Clima board
        /// </summary>
        public IRainGauge? RainGauge { get; }

        /// <summary>
        /// The Switching Anemometer on the Clima board 
        /// </summary>
        public IAnemometer? Anemometer { get; }

        /// <summary>
        /// The Solar Voltage Input on the Clima board
        /// </summary>
        public IAnalogInputPort? SolarVoltageInput { get; }

        /// <summary>
        /// The Battery Voltage Input on the Clima board
        /// </summary>
        public IAnalogInputPort? BatteryVoltageInput { get; }

        /// <summary>
        /// The RGB PWM LED on the Clima board
        /// </summary>
        public IRgbPwmLed? RgbLed { get; }

        /// <summary>
        /// Gets the Qwiic connector on the Clima board.
        /// </summary>
        public I2cConnector Qwiic { get; }

        /// <summary>
        /// The hardware revision string for the Clima board
        /// </summary>
        public string RevisionString { get; }
    }
}