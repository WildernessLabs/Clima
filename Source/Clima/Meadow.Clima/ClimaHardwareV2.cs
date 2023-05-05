using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Foundation.Sensors.Gnss;
using Meadow.Hardware;
using System;

namespace Meadow.Devices
{
    public class ClimaHardwareV2 : ClimaHardwareBase
    {
        /// <summary>
        /// The Neo GNSS sensor
        /// </summary>
        public override NeoM8 Gnss { get; protected set; }

        public ClimaHardwareV2(IF7FeatherMeadowDevice device, II2cBus i2cBus) :
            base(device, i2cBus)
        {
            try
            {
                Resolver.Log.Debug("Initializing GNSS");
                Gnss = new NeoM8(device, device.PlatformOS.GetSerialPortName("COM4"), device.Pins.D09, device.Pins.D11);
                Resolver.Log.Debug("GNSS initialized");
            }
            catch (Exception e)
            {
                Resolver.Log.Error($"Err initializing GNSS: {e.Message}");
            }
        }
    }
}
