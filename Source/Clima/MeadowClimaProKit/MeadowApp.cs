using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Web.Maple.Server;
using Meadow.Gateway.WiFi;
using MeadowClimaProKit.Controller;
using MeadowClimaProKit.ServiceAccessLayer;
using System;
using System.Threading.Tasks;

namespace MeadowClimaProKit
{
    public class MeadowApp : App<F7MicroV2, MeadowApp>
    {
        MapleServer mapleServer;

        public MeadowApp()
        {
            InitializeMaple().Wait();

            mapleServer.Start();
        }

        async Task InitializeMaple()
        {
            LedController.Instance.SetColor(Color.Red);

            var result = await Device.WiFiAdapter.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD);
            if (result.ConnectionStatus != ConnectionStatus.Success)
            {
                throw new Exception($"Cannot connect to network: {result.ConnectionStatus}");
            }

            await DateTimeService.GetTimeAsync();

            mapleServer = new MapleServer(Device.WiFiAdapter.IpAddress, 5417, false);

            LedController.Instance.SetColor(Color.Green);            
        }
    }
}