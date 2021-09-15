using System;
using System.Threading.Tasks;
using Clima.Meadow.Pro.Server.Bluetooth;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Weather;
using Meadow.Gateway.WiFi;
using Meadow.Hardware;
using Meadow.Peripherals.Leds;
using Clima.Meadow.Pro.DataAccessLayer;
using Meadow.Foundation;
using Clima.Meadow.Pro.Models;

namespace Clima.Meadow.Pro
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        //==== peripherals
        RgbPwmLed onboardLed;

        //==== controllers and such

        public MeadowApp()
        {
            //==== new up our peripherals
            Initialize().Wait();

            ClimateMonitorAgent.Instance.ClimateConditionsUpdated += (s,e) => { DebugOut(e); };

            // start our sensor updating
            Console.WriteLine("Here");
            ClimateMonitorAgent.Instance.StartUpdating(TimeSpan.FromSeconds(10));

            // start the DbManager
            LocalDbManager.Instance.StartUpdating();

            Console.WriteLine("MeadowApp finished ctor.");
        }

        /// <summary>
        /// Initializes the hardware.
        /// </summary>
        async Task Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            //==== onboard LED
            onboardLed = new RgbPwmLed(device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue,
                3.3f, 3.3f, 3.3f,
                IRgbLed.CommonType.CommonAnode);

            Console.WriteLine("RgbPwmLed up");
            onboardLed.SetColor(WildernessLabsColors.ChileanFire);

            //==== coprocessor (WiFi and Bluetooth)
            Console.WriteLine("Initializaing coprocessor.");
            Device.InitCoprocessor().Wait();
            onboardLed.SetColor(WildernessLabsColors.PearGreen);

            //==== connect to wifi
            Console.WriteLine($"Connecting to WiFi Network {Secrets.WIFI_NAME}");
            try {
                var connectionResult = await Device.WiFiAdapter.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD);
                if (connectionResult.ConnectionStatus != ConnectionStatus.Success) {
                    throw new Exception($"Cannot connect to network: {connectionResult.ConnectionStatus}");
                }
                Console.WriteLine($"Connected to {Secrets.WIFI_NAME}.");
                onboardLed.SetColor(WildernessLabsColors.AzureBlue);
            } catch (Exception e) {
                Console.WriteLine($"Err when connecting to WiFi: {e.Message}");
            }

            Console.WriteLine("Hardware initialization complete.");
        }

        protected void DebugOut(ClimateConditions conditions)
        {
            Console.WriteLine("New climate reading:");
            Console.WriteLine($"Temperature: {conditions.New?.Temperature?.Celsius:N2}C");
            Console.WriteLine($"Pressure: {conditions.New?.Pressure?.Millibar:N2}millibar");
            Console.WriteLine($"Humidity: {conditions.New?.Humidity:N2}%");
            Console.WriteLine($"Wind Direction: {conditions.New?.WindDirection?.Compass16PointCardinalName}");
        }
    }
}