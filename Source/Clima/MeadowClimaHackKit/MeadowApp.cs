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
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV1>
    {
        bool isWiFi = true;

        PushButton buttonUp, buttonDown, buttonMenu;

        public override Task Initialize()
        {
            LedController.Instance.SetColor(Color.Red);

            buttonUp = new PushButton(Device, Device.Pins.D03);
            buttonDown = new PushButton(Device, Device.Pins.D04);
            buttonMenu = new PushButton(Device, Device.Pins.D05);

            buttonUp.Clicked += (s, e) => DisplayController.Instance.MenuUp();
            buttonDown.Clicked += (s, e) => DisplayController.Instance.MenuDown();
            buttonMenu.Clicked += (s, e) => DisplayController.Instance.MenuSelect();

            DisplayController.Instance.ShowSplashScreen();
            
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