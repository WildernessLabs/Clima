using Meadow.Devices.Clima.Hardware;
using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Hardware;
using Meadow.Logging;
using System;

namespace Meadow.Devices;

/// <summary>
/// Represents the Clima hardware
/// </summary>
public class ClimaHardwareProvider : IMeadowAppEmbeddedHardwareProvider<IClimaHardware>
{
    /// <summary>
    /// Initializes a new instance of the ClimaHardwareProvider class.
    /// </summary>
    public ClimaHardwareProvider()
    { }

    /// <summary>
    /// Creates an instance of the Clima hardware.
    /// </summary>
    /// <returns>An instance of IClimaHardware.</returns>
    public static IClimaHardware Create()
    {
        return new ClimaHardwareProvider()
            .Create(Resolver.Services.Get<IMeadowDevice>()!);
    }

    /// <summary>
    /// Create an instance of the Clima class
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public IClimaHardware Create(IMeadowDevice device)
    {
        IClimaHardware hardware;
        Logger? logger = Resolver.Log;
        II2cBus i2cBus;

        logger?.Debug("Initializing Clima...");

        if (device == null)
        {
            var msg = "Clima instance must be created no earlier than App.Initialize()";
            logger?.Error(msg);
            throw new Exception(msg);
        }

        i2cBus = device.CreateI2cBus();

        logger?.Debug("I2C Bus instantiated");

        if (device is IF7FeatherMeadowDevice { } feather)
        {
            logger?.Info("Instantiating Clima v2 specific hardware");
            hardware = new ClimaHardwareV2(feather, i2cBus);
        }
        else if (device is IF7CoreComputeMeadowDevice { } ccm)
        {
            Mcp23008? mcpVersion = null;
            byte version = 0;

            try
            {
                logger?.Info("Instantiating version MCP23008");

                var resetPort = ccm.Pins.D02.CreateDigitalOutputPort();

                mcpVersion = new Mcp23008(i2cBus, address: 0x27, resetPort: resetPort);

                version = mcpVersion.ReadFromPorts();
            }
            catch
            {
                logger?.Info("Failed to instantiate version MCP23008");
            }

            logger?.Info($"MCP Version: {version}");

            if (version >= 4)
            {
                logger?.Info("Instantiating Clima v4 specific hardware");
                hardware = new ClimaHardwareV4(ccm, i2cBus, mcpVersion!);
            }
            else
            {
                logger?.Info("Instantiating Clima v3 specific hardware");
                hardware = new ClimaHardwareV3(ccm, i2cBus, mcpVersion!);
            }
        }
        else
        {
            throw new NotSupportedException();
        }

        return hardware;
    }
}