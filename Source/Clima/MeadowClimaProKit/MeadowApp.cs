using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Web.Maple;
using Meadow.Hardware;
using MeadowClimaProKit.Connectivity;
using MeadowClimaProKit.Controller;
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
                try
                {
                    var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
                    wifi.NetworkConnected += NetworkConnected;
                    await wifi.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD, TimeSpan.FromSeconds(45));
                }
                catch(Exception ex)
                {
                    Resolver.Log.Error(ex.Message);
                }
            }
            else
            {
                BluetoothServer.Instance.Initialize();

                LedController.Instance.SetColor(Color.Green);
            }
        }

        private void NetworkConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
        {
            var mapleServer = new MapleServer(sender.IpAddress, 5417, true, logger: Resolver.Log);
            mapleServer.Start();

            LedController.Instance.SetColor(Color.Green);
        }
    }
}