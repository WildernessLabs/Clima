using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Hardware;

namespace Meadow.Devices.Clima.Hardware;

/// <summary>
/// Represents the Clima v4.x hardware
/// </summary>
public class ClimaHardwareV4 : ClimaHardwareV3
{
    /// <inheritdoc/>
    public override string RevisionString => "v4.x";

    /// <summary>
    /// Create a new ClimaHardwareV4 object
    /// </summary>
    /// <param name="device">The meadow device</param>
    /// <param name="i2cBus">The I2C bus</param>
    /// <param name="mcpVersion">The Mcp23008 used to read version information</param>
    public ClimaHardwareV4(IF7CoreComputeMeadowDevice device, II2cBus i2cBus, Mcp23008 mcpVersion)
        : base(device, i2cBus, mcpVersion)
    { }

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