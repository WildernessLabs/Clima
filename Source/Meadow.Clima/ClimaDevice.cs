using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Hardware;
using Meadow.Logging;
using System;

namespace Meadow.Devices;

public partial class ClimaDevice : CustomMeadowDevice<F7CoreComputeV2>
{
    private IClimaHardware hardware;
    private ConnectorCollection? connectors;
    private byte hardwareVersion;

    public virtual ConnectorCollection Connectors => connectors ??= new ClimaConnectors(hardware);
    public override string Revision => $"v{hardwareVersion}";

    public ClimaDevice()
    {
        hardware = DetectHardware();
    }

    private IClimaHardware DetectHardware()
    {
        Logger? logger = Resolver.Log;
        II2cBus i2cBus;

        logger?.Debug("Initializing Clima...");

        var device = Resolver.Device;

        if (Resolver.Device == null)
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
            hardwareVersion = 0;

            try
            {
                logger?.Info("Instantiating version MCP23008");

                var resetPort = ccm.Pins.D02.CreateDigitalOutputPort();

                mcpVersion = new Mcp23008(i2cBus, address: 0x27, resetPort: resetPort);

                hardwareVersion = mcpVersion.ReadFromPorts();
            }
            catch
            {
                logger?.Info("Failed to instantiate version MCP23008");
            }

            logger?.Info($"MCP Version: {hardwareVersion}");

            if (hardwareVersion >= 4)
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
