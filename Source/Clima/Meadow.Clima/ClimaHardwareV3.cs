using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Foundation.Sensors.Environmental;
using Meadow.Foundation.Sensors.Gnss;
using Meadow.Foundation.Sensors.Weather;
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

        public override string RevisionString => "v3.x";

        public ClimaHardwareV3(IF7CoreComputeMeadowDevice device, II2cBus i2cBus) : base()
        {
            base.Initialize(device, i2cBus);

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
                Logger?.Trace("Instantiating environmental sensor");
                EnvironmentalSensor = new Scd40(I2cBus, (byte)Scd40.Addresses.Default);
                Logger?.Trace("Environmental sensor up");
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Unable to create the SCD40 Environmental Sensor: {ex.Message}");
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

            try
            {
                Logger?.Trace("Instantiating Wind Vane");
                WindVane = new WindVane(device.Pins.A00);
                Logger?.Trace("Wind Vane up");
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Unabled to create the Wind Vane: {ex.Message}");
            }

            try
            {
                Logger?.Trace("Instantiating Rain Gauge");
                RainGauge = new SwitchingRainGauge(device.Pins.D11);
                Logger?.Trace("RainGauge up");
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Unabled to create the Switching Rain Gauge: {ex.Message}");
            }

            try
            {
                Logger?.Trace("Instantiating Switching Anemometer");
                Anemometer = new SwitchingAnemometer(device.Pins.A01);
                Logger?.Trace("Switching Anemometer up");
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Unabled to create the Switching Anemometer: {ex.Message}");
            }

            try
            {
                Logger?.Trace("Instantiating Solar Voltage Input");
                SolarVoltageInput = device.Pins.A02.CreateAnalogInputPort(5);
                Logger?.Trace("Solar Voltage Input up");
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Unabled to create the Switching Anemometer: {ex.Message}");
            }
        }
    }
}