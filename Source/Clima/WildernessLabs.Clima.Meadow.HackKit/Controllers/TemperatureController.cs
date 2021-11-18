using Clima.Meadow.HackKit;
using Meadow.Foundation.Sensors.Temperature;
using Meadow.Units;
using System;

namespace WildernessLabs.Clima.Meadow.HackKit.Controllers
{
    public static class TemperatureController
    {
        static AnalogTemperature analogTemperature;

        public static Temperature? TemperatureValue => analogTemperature.Temperature;

        static TemperatureController() { }

        public static void Initialize()
        {
            analogTemperature = new AnalogTemperature(
                device: MeadowApp.Device,
                analogPin: MeadowApp.Device.Pins.A00,
                sensorType: AnalogTemperature.KnownSensorType.LM35
            );
            analogTemperature.StartUpdating(TimeSpan.FromSeconds(30));
        }
    }
}