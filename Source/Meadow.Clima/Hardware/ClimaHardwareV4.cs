using Meadow.Foundation;
using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Foundation.Sensors.Light;
using Meadow.Hardware;
using Meadow.Peripherals.Sensors.Light;
using Meadow.Units;

namespace Meadow.Devices.Clima.Hardware;

/// <summary>
/// Represents the Clima v4.x hardware
/// </summary>
public class ClimaHardwareV4 : ClimaHardwareV3
{
    private ILightSensor? _lightSensor;
    private bool _firstLightQuery = true;

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
    {
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

    protected override ILightSensor? GetLightSensor()
    {
        if (_lightSensor == null && _firstLightQuery)
        {
            try
            {
                Logger?.Trace("Creating Light sensor");
                _lightSensor = new Veml7700(_device.CreateI2cBus());
                if (_lightSensor is PollingSensorBase<Illuminance> pollingSensor)
                {
                    pollingSensor.CommunicationError += (s, e) => { _lightSensor.StopUpdating(); };
                }
            }
            catch
            {
                Logger?.Warn("Light sensor not found on I2C bus");
                _lightSensor = null;
            }

            _firstLightQuery = false;
        }

        return _lightSensor;
    }
}