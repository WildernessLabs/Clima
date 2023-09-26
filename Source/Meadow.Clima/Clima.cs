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
                logger?.Info("Instantiating Clima v3 specific hardware");
                hardware = new ClimaHardwareV3(ccm, i2cBus);
            }
            else
            {
                throw new NotSupportedException();
            }

            return hardware;
        }
    }
}