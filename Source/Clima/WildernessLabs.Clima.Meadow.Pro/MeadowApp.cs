using Clima.Meadow.Pro.DataAccessLayer;
using Clima.Meadow.Pro.Models;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Gateway.WiFi;
using System;
using System.Threading.Tasks;

namespace Clima.Meadow.Pro
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        //==== Controllers and such
        public MeadowApp()
        {
            Console.WriteLine("MeadowApp constructor started.");

            //==== new up our peripherals
            Initialize().Wait();

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
        async Task Initialize()
        {
            Console.WriteLine("Hardware initialization started.");
            var onboardLed = new RgbPwmLed(device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue);

            onboardLed.SetColor(WildernessLabsColors.ChileanFire);

            //==== connect to wifi
            Console.WriteLine($"Connecting to WiFi Network {Secrets.WIFI_NAME}");
            try 
            {
                var connectionResult = await Device.WiFiAdapter.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD);
                if (connectionResult.ConnectionStatus != ConnectionStatus.Success) 
                {
                    throw new Exception($"Cannot connect to network: {connectionResult.ConnectionStatus}");
                }
                Console.WriteLine($"Connected to {Secrets.WIFI_NAME}.");
                onboardLed.SetColor(WildernessLabsColors.AzureBlue);
            } 
            catch (Exception e) 
            {
                Console.WriteLine($"Err when connecting to WiFi: {e.Message}");
            }

            onboardLed.SetColor(Color.Green);
            Console.WriteLine("Hardware initialization complete.");
        }

        protected void DebugOut(Climate climate)
        {
            Console.WriteLine("New climate reading:");
            Console.WriteLine($"Temperature: {climate.Temperature?.Celsius:N2}C");
            Console.WriteLine($"Pressure: {climate.Pressure?.Millibar:N2}millibar");
            Console.WriteLine($"Humidity: {climate.Humidity:N2}%");
            Console.WriteLine($"Wind Direction: {climate.WindDirection?.Compass16PointCardinalName}");
        }
    }
}