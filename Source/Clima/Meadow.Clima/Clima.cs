using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Hardware;
using Meadow.Logging;
using System;

#nullable enable

namespace Meadow.Devices
{
    public class Clima
    {
        private Clima() { }

        /// <summary>
        /// Create an instance of the Clima class
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static IClimaHardware Create()
        {
            IClimaHardware hardware;
            Logger? logger = Resolver.Log;
            II2cBus i2cBus;

            // v3+ stuff
            Mcp23008? mcp1 = null;

            logger?.Debug("Initializing Clima Lab...");

            var device = Resolver.Device; //convenience local var

            // make sure not getting instantiated before the App Initialize method
            if (Resolver.Device == null)
            {
                var msg = "Clima instance must be created no earlier than App.Initialize()";
                logger?.Error(msg);
                throw new Exception(msg);
            }

            I32PinFeatherBoardPinout pins = device switch
            {
                IF7FeatherMeadowDevice f => f.Pins,
                IF7CoreComputeMeadowDevice c => c.Pins,
                _ => throw new NotSupportedException("Device must be a Feather F7 or F7 Core Compute module"),
            };

            i2cBus = device.CreateI2cBus();

            logger?.Debug("I2C Bus instantiated");

            IDigitalInputPort? mcp1Interrupt = null;
            IDigitalOutputPort? mcp1Reset = null;

            try
            {
                if (device is IF7FeatherMeadowDevice)
                {
                    mcp1Interrupt = device.CreateDigitalInputPort(pins.D09, InterruptMode.EdgeRising, ResistorMode.InternalPullDown);
                    mcp1Reset = device.CreateDigitalOutputPort(pins.D14);

                    mcp1 = new Mcp23008(i2cBus, address: 0x20, mcp1Interrupt, mcp1Reset);

                    logger?.Trace("Mcp_1 up");
                }
            }
            catch (Exception e)
            {
                logger?.Debug($"Failed to create MCP1: {e.Message}, could be a v1 or v2 board");
                mcp1Interrupt?.Dispose();
                mcp1Reset?.Dispose();
            }

            if (device is IF7FeatherMeadowDevice { } feather)
            {
                logger?.Debug("Instantiating Clima v2 specific hardware");
                hardware = new ClimaHardwareV2(feather, i2cBus);
            }
            else if (device is IF7CoreComputeMeadowDevice { } ccm)
            {
                logger?.Info("Instantiating Clima v3 specific hardware");
                hardware = new ClimaHardwareV3(ccm, i2cBus);
            }
            else
            {
                throw new NotSupportedException(); //should never get here
            }

            return hardware;
        }
    }
}