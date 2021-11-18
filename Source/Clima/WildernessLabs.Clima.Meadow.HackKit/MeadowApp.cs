using Clima.Meadow.HackKit.Controllers;
using Clima.Meadow.HackKit.ServiceAccessLayer;
using Clima.Meadow.HackKit.Utils;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Foundation.Web.Maple.Server;
using Meadow.Gateway.WiFi;
using System;
using System.Threading;
using System.Threading.Tasks;
using WildernessLabs.Clima.Meadow.HackKit.Controllers;
using WildernessLabs.Clima.Meadow.HackKit.ServiceAccessLayer;

namespace Clima.Meadow.HackKit
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        DisplayController displayController;
        MapleServer mapleServer;

        PushButton buttonUp, buttonDown, buttonMenu;

        public MeadowApp()
        {
            Initialize();
            LedIndicator.SetColor(Color.Blue);
            displayController.ShowSplashScreen();

            InitializeWiFi().Wait();
            LedIndicator.SetColor(Color.Green);

            mapleServer = new MapleServer(Device.WiFiAdapter.IpAddress, 5417, false);
            mapleServer.Start();

            _ = StartUpdates();
        }

        /// <summary>
        /// Initializes the hardware.
        /// </summary>
        void Initialize()
        {
            Console.WriteLine("Init RGB");
            LedIndicator.Initialize();
            LedIndicator.SetColor(Color.Red);

            Console.WriteLine("Init analog temperature sensor");
            TemperatureController.Initialize();

            Console.WriteLine("Init display controller");
            displayController = new DisplayController();
            
            Console.WriteLine("Init buttons");
            buttonUp = new PushButton(Device, Device.Pins.D03);
            buttonDown = new PushButton(Device, Device.Pins.D02);
            buttonMenu = new PushButton(Device, Device.Pins.D04);

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
                var dateTime = await DateTimeService.GetTimeAsync();

                Device.SetClock(new DateTime(
                    year: dateTime.Year,
                    month: dateTime.Month,
                    day: dateTime.Day,
                    hour: dateTime.Hour,
                    minute: dateTime.Minute,
                    second: dateTime.Second));

                displayController.UpdateStatusText("WiFi", "Connected!");
            }
        }

        async Task StartUpdates()
        {
            //while(true)
            //{
                var conditions = TemperatureController.TemperatureValue.Value;

                displayController.UpdateDisplay(conditions);

                await ClimateService.FetchReadings();

                await ClimateService.PostTempReading(conditions);

            //    await Task.Delay(TimeSpan.FromSeconds(10));
            //}
        }
    }
}