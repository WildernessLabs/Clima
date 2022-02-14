using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Foundation.Web.Maple.Server;
using Meadow.Gateway.WiFi;
using MeadowClimaHackKit.Connectivity;
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
            Initialize();
        }

        void Initialize() 
        {
            LedController.Instance.SetColor(Color.Red);

            buttonUp = new PushButton(Device, Device.Pins.D03);
            buttonDown = new PushButton(Device, Device.Pins.D02);
            buttonMenu = new PushButton(Device, Device.Pins.D04);

            buttonUp.Clicked += (s, e) => DisplayController.Instance.MenuUp();
            buttonDown.Clicked += (s, e) => DisplayController.Instance.MenuDown();
            buttonMenu.Clicked += (s, e) => DisplayController.Instance.MenuSelect();

            DisplayController.Instance.ShowSplashScreen();

            TemperatureController.Instance.Initialize();

            InitializeBluetooth();

            //InitializeMaple().Wait();

            LedController.Instance.SetColor(Color.Green);
        }

        void InitializeBluetooth()
        {
            BluetoothServer.Instance.Initialize();
        }

        async Task InitializeMaple()
        {            
            DisplayController.Instance.StartWifiConnectingAnimation();

            var result = await Device.WiFiAdapter.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD);
            if (result.ConnectionStatus != ConnectionStatus.Success)
            {
                throw new Exception($"Cannot connect to network: {result.ConnectionStatus}");
            }

            DisplayController.Instance.StopWifiConnectingAnimation();

            await DateTimeService.GetTimeAsync();

            mapleServer = new MapleServer(Device.WiFiAdapter.IpAddress, 5417, false);
            mapleServer.Start();
        }
    }
}