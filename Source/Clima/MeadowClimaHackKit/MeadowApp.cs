using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Foundation.Web.Maple;
using Meadow.Gateway.WiFi;
using Meadow.Hardware;
using MeadowClimaHackKit.Connectivity;
using MeadowClimaHackKit.Controller;
using MeadowClimaHackKit.ServiceAccessLayer;
using System;
using System.Threading.Tasks;

namespace MeadowClimaHackKit
{
    // public class MeadowApp : App<F7Micro, MeadowApp> <- If you have a Meadow F7v1.*
    public class MeadowApp : App<F7FeatherV2>
    {
        PushButton buttonUp, buttonDown, buttonMenu;

        public override Task Initialize()
        {
            LedController.Instance.SetColor(Color.Red);

            buttonUp = new PushButton(Device, Device.Pins.D03);
            buttonDown = new PushButton(Device, Device.Pins.D02);
            buttonMenu = new PushButton(Device, Device.Pins.D04);

            buttonUp.Clicked += (s, e) => DisplayController.Instance.MenuUp();
            buttonDown.Clicked += (s, e) => DisplayController.Instance.MenuDown();
            buttonMenu.Clicked += (s, e) => DisplayController.Instance.MenuSelect();

            DisplayController.Instance.ShowSplashScreen();

            //InitializeBluetooth();
            InitializeMaple().Wait();

            return base.Initialize();
        }

        void InitializeBluetooth()
        {
            BluetoothServer.Instance.Initialize();
        }

        async Task InitializeMaple()
        {
            DisplayController.Instance.StartWifiConnectingAnimation();

            var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();

            var connectionResult = await wifi.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD, TimeSpan.FromSeconds(45));
            if (connectionResult.ConnectionStatus != ConnectionStatus.Success)
            {
                throw new Exception($"Cannot connect to network: {connectionResult.ConnectionStatus}");
            }

            DisplayController.Instance.StopWifiConnectingAnimation();

            await DateTimeService.GetTimeAsync();

            var mapleServer = new MapleServer(wifi.IpAddress, 5417, false);
            mapleServer.Start();

            TemperatureController.Instance.Initialize();

            LedController.Instance.SetColor(Color.Green);
        }
    }
}