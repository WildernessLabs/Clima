using Meadow.Foundation;
using Meadow.Foundation.Sensors.Temperature;
using Meadow.Units;
using MeadowClimaHackKit.Database;
using System;

namespace MeadowClimaHackKit.Controllers
{
    public class TemperatureController
    {
        AnalogTemperature analogTemperature;

        private static readonly Lazy<TemperatureController> instance =
            new Lazy<TemperatureController>(() => new TemperatureController());
        public static TemperatureController Instance => instance.Value;

        private TemperatureController()
        {
            Initialize();
        }

        public void Initialize()
        {
            analogTemperature = new AnalogTemperature(MeadowApp.Device,
                MeadowApp.Device.Pins.A01, AnalogTemperature.KnownSensorType.LM35);
            analogTemperature.StartUpdating(TimeSpan.FromSeconds(30));
            analogTemperature.TemperatureUpdated += AnalogTemperatureUpdated;
        }

        void AnalogTemperatureUpdated(object sender, Meadow.IChangeResult<Temperature> e)
        {
            LedController.Instance.SetColor(Color.Cyan);
            
            var reading = new TemperatureTable()
            {
                TemperatureValue = e.New,
                DateTime = DateTime.Now
            };
            DatabaseManager.Instance.SaveReading(reading);

            LedController.Instance.SetColor(Color.Green);
        }
    }
}