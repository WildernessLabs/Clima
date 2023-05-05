using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Foundation.Sensors.Gnss;
using Meadow.Hardware;
using System;

namespace Meadow.Devices
{
    public class ClimaHardwareV3 : ClimaHardwareBase
    {
        /// <summary>
        /// The MCP23008 IO expander that contains the ProjectLab hardware version 
        /// </summary>
        Mcp23008? Mcp_Version { get; set; }

        /// <summary>
        /// The Neo GNSS sensor
        /// </summary>
        public override NeoM8 Gnss { get; protected set; }

        public ClimaHardwareV3(IF7FeatherMeadowDevice device, II2cBus i2cBus) :
            base(device, i2cBus)
        {
            try
            {
                Logger?.Trace("Instantiating Mcp Version");
                Mcp_Version = new Mcp23008(I2cBus, address: 0x27);
                Logger?.Info("Mcp Version up");
            }
            catch (Exception e)
            {
                Logger?.Trace($"ERR creating the MCP that has version information: {e.Message}");
            }

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