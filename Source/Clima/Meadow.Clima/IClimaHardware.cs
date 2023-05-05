using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Environmental;
using Meadow.Foundation.Sensors.Gnss;
using Meadow.Foundation.Sensors.Weather;
using Meadow.Hardware;

namespace Meadow.Devices
{
    public interface IClimaHardware
    {
        public II2cBus I2cBus { get; }

        public Bme688? AtmosphericSensor { get; }
        public Scd40? EnvironmentalSensor { get; }
        public NeoM8? Gnss { get; }

        public WindVane? WindVane { get; }
        public SwitchingRainGauge? RainGauge { get; }
        public SwitchingAnemometer? Anemometer { get; }

        public IAnalogInputPort? SolarVoltageInput { get; }

        public RgbPwmLed ColorLed { get; }

        public string RevisionString { get; }
    }
}