using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Foundation.Web.Maple.Server;
using Meadow.Gateway.WiFi;
using MeadowClimaHackKit.Controllers;
using MeadowClimaHackKit.ServiceAccessLayer;
using System.Threading;
using System.Threading.Tasks;

namespace MeadowClimaHackKit
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        MapleServer mapleServer;
        PushButton buttonUp, buttonDown, buttonMenu;

        CancellationTokenSource token;

        public MeadowApp()
        {
            Initialize();
        }

        void Initialize() 
        {
            InitializeHardware();
            LedController.Instance.SetColor(Color.Blue);
            DisplayController.Instance.ShowSplashScreen();

            InitializeWiFi().Wait();
            LedController.Instance.SetColor(Color.Green);
        }

        void InitializeHardware()
        {
            LedController.Instance.SetColor(Color.Red);
            
            buttonUp = new PushButton(Device, Device.Pins.D03);
            buttonDown = new PushButton(Device, Device.Pins.D02);
            buttonMenu = new PushButton(Device, Device.Pins.D04);

            buttonUp.Clicked += (s, e) => DisplayController.Instance.MenuUp();
            buttonDown.Clicked += (s, e) => DisplayController.Instance.MenuDown();
            buttonMenu.Clicked += (s, e) => DisplayController.Instance.MenuSelect();
        }

        async Task InitializeWiFi()
        {
            //token = new CancellationTokenSource();

            //_ = Task.Run(async () =>
            //{
            //    string ellipsis;
            //    int count = 0;
            //    while (token.IsCancellationRequested == false)
            //    {
            //        ellipsis = (count++ % 4) switch
            //        {
            //            0 => "   ",
            //            1 => ".  ",
            //            2 => ".. ",
            //            _ => "...",
            //        };

            //        DisplayController.Instance.UpdateStatusText("WiFi", "Connecting" + ellipsis);
            //        await Task.Delay(500);
            //    }

            //}, token.Token);

            var result = await Device.WiFiAdapter.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD);
            if (result.ConnectionStatus != ConnectionStatus.Success)
            {
                DisplayController.Instance.UpdateStatusText("WiFi", "Failed");
            }
            else
            {
                await DateTimeService.GetTimeAsync();
                DisplayController.Instance.UpdateStatusText("WiFi", "Connected!");
            }

            //token.Cancel(); //stop the ellipsis task above
            await DateTimeService.GetTimeAsync();

            mapleServer = new MapleServer(Device.WiFiAdapter.IpAddress, 5417, false);
            mapleServer.Start();

            DisplayController.Instance.UpdateStatusText("WiFi", "Connected!");
        }
    }
}