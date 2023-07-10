using Clima_HackKit_Demo.Connectivity;
using Clima_HackKit_Demo.Controller;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Foundation.Web.Maple;
using Meadow.Hardware;
using System;
using System.Threading.Tasks;

namespace Clima_HackKit_Demo
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2>
    {
        bool isWiFi = false;

        PushButton buttonUp, buttonMenu;
        PollingPushButton buttonDown;

        public override Task Initialize()
        {
            LedController.Instance.SetColor(Color.Red);

            buttonUp = new PushButton(Device.Pins.D03);
            buttonDown = new PollingPushButton(Device.Pins.D02);
            buttonMenu = new PushButton(Device.Pins.D04);

            buttonUp.Clicked += (s, e) => DisplayController.Instance.MenuUp();
            buttonDown.Clicked += (s, e) => DisplayController.Instance.MenuDown();
            buttonMenu.Clicked += (s, e) => DisplayController.Instance.MenuSelect();

            DisplayController.Instance.ShowSplashScreen();

            TemperatureController.Instance.Initialize();

            if (isWiFi)
            {
                InitializeMaple().Wait();
            }
            else
            {
                InitializeBluetooth();
            }

            return base.Initialize();
        }

        void InitializeBluetooth()
        {
            BluetoothServer.Instance.Initialize();
        }

        async Task InitializeMaple()
        {
            _ = DisplayController.Instance.StartWifiConnectingAnimation();

            var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            wifi.NetworkConnected += NetworkConnected;
            await wifi.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD, TimeSpan.FromSeconds(45));
        }

        private void NetworkConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
        {
            DisplayController.Instance.StopWifiConnectingAnimation();

            var mapleServer = new MapleServer(sender.IpAddress, 5417, true, logger: Resolver.Log);
            mapleServer.Start();

            LedController.Instance.SetColor(Color.Green);
        }
    }
}