using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Foundation.Web.Maple.Server;
using Meadow.Gateway.WiFi;
using MeadowClimaHackKit.Controller;
using MeadowClimaHackKit.ServiceAccessLayer;
using System;
using System.Threading.Tasks;

namespace MeadowClimaHackKit
{
    // public class MeadowApp : App<F7Micro, MeadowApp> <- If you have a Meadow F7v1.*
    public class MeadowApp : App<F7MicroV2, MeadowApp>
    {
        MapleServer mapleServer;
        PushButton buttonUp, buttonDown, buttonMenu;

        public MeadowApp()
        {
            InitializeMaple().Wait();

            buttonUp = new PushButton(Device, Device.Pins.D03);
            buttonDown = new PushButton(Device, Device.Pins.D02);
            buttonMenu = new PushButton(Device, Device.Pins.D04);

            buttonUp.Clicked += (s, e) => DisplayController.Instance.MenuUp();
            buttonDown.Clicked += (s, e) => DisplayController.Instance.MenuDown();
            buttonMenu.Clicked += (s, e) => DisplayController.Instance.MenuSelect();

            mapleServer.Start();
        }

        async Task InitializeMaple()
        {
            LedController.Instance.SetColor(Color.Red);

            DisplayController.Instance.ShowSplashScreen();
            DisplayController.Instance.StartWifiConnectingAnimation();

            var result = await Device.WiFiAdapter.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD);
            if (result.ConnectionStatus != ConnectionStatus.Success)
            {
                throw new Exception($"Cannot connect to network: {result.ConnectionStatus}");
            }

            DisplayController.Instance.StopWifiConnectingAnimation();

            await DateTimeService.GetTimeAsync();

            mapleServer = new MapleServer(Device.WiFiAdapter.IpAddress, 5417, false);

            TemperatureController.Instance.Initialize();

            LedController.Instance.SetColor(Color.Green);
        }
    }
}