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

        WindVane? WindVane { get; }
        SwitchingRainGauge? RainGauge { get; }
        SwitchingAnemometer? Anemometer { get; }

        IAnalogInputPort? SolarVoltageInput { get; }
    }
}