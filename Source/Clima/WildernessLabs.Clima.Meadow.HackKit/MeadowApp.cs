using Clima.Meadow.HackKit.Controllers;
using Clima.Meadow.HackKit.ServiceAccessLayer;
using Clima.Meadow.HackKit.Utils;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Foundation.Sensors.Temperature;
using Meadow.Gateway.WiFi;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Clima.Meadow.HackKit
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        AnalogTemperature analogTemperature;
        DisplayController displayController;

        PushButton buttonUp, buttonDown, buttonMenu;

        public MeadowApp()
        {
            Initialize();
            LedIndicator.SetColor(Color.Blue);
            displayController.ShowSplashScreen();

            InitializeWiFi().Wait();
            LedIndicator.SetColor(Color.Green);

            _ = StartUpdates();
        }

        /// <summary>
        /// Initializes the hardware.
        /// </summary>
        void Initialize()
        {
            Console.WriteLine("Init RGB");
            LedIndicator.Initialize(
                Device,
                Device.Pins.OnboardLedRed,
                Device.Pins.OnboardLedGreen,
                Device.Pins.OnboardLedBlue
            );
            LedIndicator.SetColor(Color.Red);

            Console.WriteLine("Init analog temperature sensor");
            analogTemperature = new AnalogTemperature(
                device: Device,
                analogPin: Device.Pins.A00,
                sensorType: AnalogTemperature.KnownSensorType.LM35
            );

            Console.WriteLine("Init display controller");
            displayController = new DisplayController();
            
            Console.WriteLine("Init buttons");
            //initialize physical buttons
            buttonUp = new PushButton(Device, Device.Pins.D03);
            buttonDown = new PushButton(Device, Device.Pins.D02);
            buttonMenu = new PushButton(Device, Device.Pins.D04);

            //subscribe to button clicked events to control menu
            buttonUp.Clicked += (s, e) => displayController.MenuUp();
            buttonDown.Clicked += (s, e) => displayController.MenuDown();
            buttonMenu.Clicked += (s, e) => displayController.MenuSelect();
        }

        async Task InitializeWiFi()
        {
            var cts = new CancellationTokenSource();

            _ = Task.Run(async () =>
            {
                string ellipsis;
                int count = 0;
                while (cts.IsCancellationRequested == false)
                {
                    ellipsis = (count++ % 4) switch
                    {
                        0 => "   ",
                        1 => ".  ",
                        2 => ".. ",
                        _ => "...",
                    };

                    displayController.UpdateStatusText("WiFi", "Connecting" + ellipsis);
                    await Task.Delay(500);
                }

            }, cts.Token);

            var result = await Device.WiFiAdapter.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD);

            cts.Cancel(); //stop the ellipsis task above

            if (result.ConnectionStatus != ConnectionStatus.Success)
            {
                displayController.UpdateStatusText("WiFi", "Failed");
            }
            else
            {
                displayController.UpdateStatusText("WiFi", "Connected!");
            }
        }

        async Task StartUpdates() 
        {
            while(true)
            {
                var conditions = await analogTemperature.Read();

                displayController.UpdateDisplay(conditions);

                await ClimateService.FetchReadings();

                await ClimateService.PostTempReading(conditions);

                await Task.Delay(5000);
            }
        }
    }
}