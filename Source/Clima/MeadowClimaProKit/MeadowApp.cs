using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Web.Maple;
using Meadow.Gateway.WiFi;
using Meadow.Hardware;
using MeadowClimaProKit.Connectivity;
using MeadowClimaProKit.Controller;
using MeadowClimaProKit.ServiceAccessLayer;
using System;
using System.Threading.Tasks;

namespace MeadowClimaProKit
{
    public class MeadowApp : App<F7FeatherV2>
    {
        bool isWiFi = true;

        public override async Task Initialize()
        {
            LedController.Instance.SetColor(Color.Red);

            ClimateMonitorAgent.Instance.Initialize();

            if (isWiFi)
            {
                var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();

                var connectionResult = await wifi.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD, TimeSpan.FromSeconds(45));
                if (connectionResult.ConnectionStatus != ConnectionStatus.Success)
                {
                    throw new Exception($"Cannot connect to network: {connectionResult.ConnectionStatus}");
                }

                await DateTimeService.GetTimeAsync();

                var mapleServer = new MapleServer(wifi.IpAddress, 5417, true, logger: Resolver.Log);
                mapleServer.Start();
            }
            else
            {
                BluetoothServer.Instance.Initialize();
            }

            LedController.Instance.SetColor(Color.Green);
        }
    }
}