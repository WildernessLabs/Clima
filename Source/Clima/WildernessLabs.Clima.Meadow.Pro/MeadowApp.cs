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

namespace Clima.Meadow.Pro
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        //==== peripherals
        RgbPwmLed onboardLed;

        //==== controllers and such
        ClimateMonitor climateMonitor;
        BluetoothServer bluetoothServer;

        // controllers
        //DisplayController displayController;

        public MeadowApp()
        {
            //==== new up our peripherals
            Initialize();

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

            //==== cache the display controller
            //this.displayController = new DisplayController();

            //==== coprocessor (WiFi and Bluetooth)
            Console.WriteLine("Initializaing coprocessor.");
            Device.InitCoprocessor().Wait();
            onboardLed.SetColor(WildernessLabsColors.PearGreen);

            //==== connect to wifi
            Console.WriteLine($"Connecting to WiFi Network {Secrets.WIFI_NAME}");
            var connectionResult = await Device.WiFiAdapter.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD);
            if (connectionResult.ConnectionStatus != ConnectionStatus.Success) {
                throw new Exception($"Cannot connect to network: {connectionResult.ConnectionStatus}");
            }
            Console.WriteLine($"Connected to {Secrets.WIFI_NAME}.");
            onboardLed.SetColor(WildernessLabsColors.AzureBlue);

            //==== singleton references
            climateMonitor = ClimateMonitor.Instance;
            bluetoothServer = BluetoothServer.Instance;

            Console.WriteLine("Hardware initialization complete.");
        }

        ///// <summary>
        ///// Performs a one-off reading of the temp sensor.
        ///// </summary>
        ///// <returns></returns>
        //protected async Task<AtmosphericConditions> ReadTemp()
        //{
        //    var conditions = await analogTemperature.Read();
        //    return conditions;
        //}
    }
}