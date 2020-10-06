using System;
using System.Threading.Tasks;
using Clima.Meadow.HackKit.Controllers;
using Clima.Meadow.HackKit.ServiceAccessLayer;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Sensors.Temperature;
using Meadow.Gateway.WiFi;
using Meadow.Peripherals.Sensors.Atmospheric;

namespace Clima.Meadow.HackKit
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        // peripherals
        AnalogTemperature analogTemperature;

        // controllers
        DisplayController displayController;

        public MeadowApp()
        {
            //==== new up our peripherals
            Initialize();

            //==== connect to wifi
            Console.WriteLine($"Connecting to WiFi Network {Secrets.WIFI_NAME}");
            var result = Device.WiFiAdapter.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD);
            if (result.ConnectionStatus != ConnectionStatus.Success) {
                throw new Exception($"Cannot connect to network: {result.ConnectionStatus}");
            }
            Console.WriteLine($"Connected to {Secrets.WIFI_NAME}.");

            //==== take a reading
            var tA = ReadTemp();
            tA.Wait();
            var conditions = tA.Result;
            Console.WriteLine($"Temp Reading: {conditions.Temperature}");

            //==== update the display
            Console.WriteLine("Updating display.");
            this.displayController.UpdateDisplay(conditions);

            //==== grab the climate readings
            Console.WriteLine("Fetching climate readings.");
            ClimateServiceFacade.FetchReadings().Wait();

            //==== post the reading
            Console.WriteLine("Posting the temp reading");
            ClimateServiceFacade.PostTempReading((decimal)conditions.Temperature).Wait();
            Console.WriteLine("Posted.");

            //==== fetch the readings again
            Console.WriteLine("Fetching the readings agian.");
            ClimateServiceFacade.FetchReadings().Wait();

            //==== farewell, Batty
            Console.WriteLine("All those moments will be lost in time, like tears in rain. Time to die.");

        }

        /// <summary>
        /// Initializes the hardware.
        /// </summary>
        void Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            // Analog Temp Sensor
            Console.WriteLine("Initializing analog temp sensor");
            analogTemperature = new AnalogTemperature(
                device: Device,
                analogPin: Device.Pins.A00,
                sensorType: AnalogTemperature.KnownSensorType.LM35
            );

            // display
            this.displayController = new DisplayController();

            // WiFi adapter
            Console.WriteLine("Initializaing wifi adapter.");
            Device.InitWiFiAdapter().Wait();

        }

        /// <summary>
        /// Performs a one-off reading of the temp sensor.
        /// </summary>
        /// <returns></returns>
        protected async Task<AtmosphericConditions> ReadTemp()
        {
            var conditions = await analogTemperature.Read();
            return conditions;
        }
    }
}