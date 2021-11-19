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
using WildernessLabs.Clima.Meadow.HackKit.Entities;

namespace Clima.Meadow.HackKit
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        MapleServer mapleServer;
        DisplayController displayController;
        TemperatureController temperatureController;
        PushButton buttonUp, buttonDown, buttonMenu;

        public TemperatureModel CurrentReading { get; set; }

        public MeadowApp()
        {
            Initialize();

            mapleServer = new MapleServer(Device.WiFiAdapter.IpAddress, 5417, false);
            mapleServer.Start();
        }

        void Initialize() 
        {
            InitializeHardware();
            LedIndicator.SetColor(Color.Blue);
            displayController.ShowSplashScreen();

            InitializeWiFi().Wait();
            LedIndicator.SetColor(Color.Green);

            temperatureController.Updated += TemperatureControllerUpdated;
        }

        void InitializeHardware()
        {
            LedIndicator.Initialize();
            LedIndicator.SetColor(Color.Red);

            Console.WriteLine("Init analog temperature sensor");
            temperatureController = new TemperatureController();            

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
                await DateTimeService.GetTimeAsync();
                displayController.UpdateStatusText("WiFi", "Connected!");
            }
        }

        async void TemperatureControllerUpdated(object sender, TemperatureModel e)
        {
            CurrentReading = e;

            displayController.UpdateDisplay(e.Temperature);

            await ClimateService.FetchReadings();

            await ClimateService.PostTempReading(e.Temperature);
        }
    }
}