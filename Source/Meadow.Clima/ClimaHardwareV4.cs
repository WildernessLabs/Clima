using Meadow.Hardware;

namespace Meadow.Devices
{
    /// <summary>
    /// Represents the Clima v4.x hardware
    /// </summary>
    public class ClimaHardwareV4 : ClimaHardwareV3
    {
        private readonly IF7CoreComputeMeadowDevice _device;

        /// <inheritdoc/>
        public override string RevisionString => "v4.x";

        /// <summary>
        /// Create a new ClimaHardwareV4 object
        /// </summary>
        /// <param name="device">The meadow device</param>
        /// <param name="i2cBus">The I2C bus</param>
        public ClimaHardwareV4(IF7CoreComputeMeadowDevice device, II2cBus i2cBus)
        : base(device, i2cBus)
        {
            _device = device;
        }

        internal override I2cConnector? CreateQwiicConnector()
        {
            Logger?.Trace("Creating Qwiic I2C connector");

            return new I2cConnector(
               nameof(Qwiic),
                new PinMapping
                {
                new PinMapping.PinAlias(I2cConnector.PinNames.SCL, _device.Pins.I2C1_SCL),
                new PinMapping.PinAlias(I2cConnector.PinNames.SDA, _device.Pins.I2C1_SDA),
                },
                new I2cBusMapping(_device, 1));
        }
    }
}