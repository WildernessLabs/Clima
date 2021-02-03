using Clima.Meadow.HackKit.Controllers;
using Clima.Meadow.HackKit.ServiceAccessLayer;
using Clima.Meadow.HackKit.Utils;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Sensors.Temperature;
using Meadow.Gateway.WiFi;
using Meadow.Peripherals.Sensors.Atmospheric;
using System;
using System.Threading.Tasks;

namespace Clima.Meadow.HackKit
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        AnalogTemperature analogTemperature;
        DisplayController displayController;

        public MeadowApp()
        {
            Initialize();

            var tA = ReadTemp();
            tA.Wait();
            var conditions = tA.Result;

            //displayController.UpdateDisplay(conditions);

            //ClimateServiceFacade.FetchReadings().Wait();

            //ClimateServiceFacade.PostTempReading((decimal)conditions.Temperature).Wait();

            //ClimateServiceFacade.FetchReadings().Wait();
        }

        /// <summary>
        /// Initializes the hardware.
        /// </summary>
        void Initialize()
        {
            LedIndicator.Initialize(
                Device, 
                Device.Pins.OnboardLedRed, 
                Device.Pins.OnboardLedGreen, 
                Device.Pins.OnboardLedBlue
            );
            LedIndicator.SetColor(Color.Red);

            analogTemperature = new AnalogTemperature(
                device: Device,
                analogPin: Device.Pins.A00,
                sensorType: AnalogTemperature.KnownSensorType.LM35
            );

            displayController = new DisplayController();
            displayController.ShowSplashScreen();

            displayController.ShowTextLine1("WIFI ADAPTER");
            Device.InitWiFiAdapter().Wait();
            displayController.ShowTextLine2("READY!");

            LedIndicator.SetColor(Color.Blue);

            displayController.ShowTextLine1("JOIN NETWORK");
            var result = Device.WiFiAdapter.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD);
            if (result.ConnectionStatus != ConnectionStatus.Success)
            {
                throw new Exception($"Cannot connect to network: {result.ConnectionStatus}");
            }
            displayController.ShowTextLine2("CONNECTED!");

            LedIndicator.SetColor(Color.Green);
        }

        async Task<AtmosphericConditions> ReadTemp()
        {
            var conditions = await analogTemperature.Read();
            return conditions;
        }
    }
}