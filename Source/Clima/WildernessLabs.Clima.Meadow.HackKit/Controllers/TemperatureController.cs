using MeadowHackKit;
using MeadowHackKit.Utils;
using Meadow.Foundation;
using Meadow.Foundation.Sensors.Temperature;
using Meadow.Units;
using System;
using WildernessLabs.MeadowHackKit.Entities;

namespace WildernessLabs.MeadowHackKit.Controllers
{
    public class TemperatureController
    {
        AnalogTemperature analogTemperature;

        public event EventHandler<TemperatureModel> Updated = delegate { };

        public Temperature? TemperatureValue => analogTemperature.Temperature;

        private static readonly Lazy<TemperatureController> instance =
            new Lazy<TemperatureController>(() => new TemperatureController());
        public static TemperatureController Instance => instance.Value;

        private TemperatureController()
        {
            Initialize();
        }

        void Initialize()
        {
            analogTemperature = new AnalogTemperature(MeadowApp.Device, MeadowApp.Device.Pins.A00, AnalogTemperature.KnownSensorType.LM35);
            analogTemperature.StartUpdating(TimeSpan.FromSeconds(30));
            analogTemperature.Updated += AnalogTemperatureUpdated;
        }

        void AnalogTemperatureUpdated(object sender, global::Meadow.IChangeResult<Temperature> e)
        {
            LedController.Instance.SetColor(Color.Cyan);

            var reading = new TemperatureModel()
            {
                Temperature = e.New,
                DateTime = DateTime.Now
            };

            Updated(this, reading);

            LedController.Instance.SetColor(Color.Green);
        }
    }
}