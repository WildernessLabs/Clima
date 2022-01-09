using MeadowProKit.DataAccessLayer;
using MeadowProKit.Models;
using MeadowProKit.Server.Bluetooth;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Gateway.WiFi;
using System;
using System.Threading.Tasks;

namespace MeadowProKit
{
    public class MeadowApp : App<F7MicroV2, MeadowApp>
    {
        //==== Controllers and such
        public MeadowApp()
        {
            Console.WriteLine("MeadowApp constructor started.");

            //==== new up our peripherals
            Initialize();

            //==== subscribe to climate updates and save them to the database
            ClimateMonitorAgent.Instance.ClimateConditionsUpdated += (s, e) =>
            {
                DebugOut(e.New);
                LocalDbManager.Instance.SaveReading(e.New);
            };
            ClimateMonitorAgent.Instance.StartUpdating(TimeSpan.FromSeconds(10));

            Console.WriteLine("MeadowApp constructor finished.");
        }

        //==== Initializes the hardware.
        protected void Initialize()
        {
            Console.WriteLine("Hardware initialization started.");
            var onboardLed = new RgbPwmLed(device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue);

            onboardLed.SetColor(WildernessLabsColors.ChileanFire);

            BluetoothServer.Instance.Initialize();

            onboardLed.SetColor(Color.Green);
            Console.WriteLine("Hardware initialization complete.");
        }

        protected void DebugOut(Climate climate)
        {
            Console.WriteLine("New climate reading:");
            Console.WriteLine($"Temperature: {climate.Temperature?.Celsius:N2}C");
            Console.WriteLine($"Pressure: {climate.Pressure?.Millibar:N2}millibar");
            Console.WriteLine($"Humidity: {climate.Humidity:N2}%");
            Console.WriteLine($"Wind Speed: {climate.WindSpeed?.KilometersPerHour}");
            Console.WriteLine($"Wind Direction: {climate.WindDirection?.Compass16PointCardinalName}");
        }
    }
}