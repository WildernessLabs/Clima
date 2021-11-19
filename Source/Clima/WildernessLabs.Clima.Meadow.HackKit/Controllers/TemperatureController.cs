using Clima.Meadow.HackKit;
using Meadow.Foundation.Sensors.Temperature;
using Meadow.Units;
using System;
using WildernessLabs.Clima.Meadow.HackKit.Entities;

namespace WildernessLabs.Clima.Meadow.HackKit.Controllers
{
    public class TemperatureController
    {
        AnalogTemperature analogTemperature;

        public event EventHandler<TemperatureModel> Updated = delegate { };

        public Temperature? TemperatureValue => analogTemperature.Temperature;

        public TemperatureController()
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
            var reading = new TemperatureModel()
            {
                Temperature = e.New,
                DateTime = DateTime.Now
            };

            Updated(this, reading);
        }
    }
}