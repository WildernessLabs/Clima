using Clima_SQLite_Demo.Connectivity;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Web.Maple;
using Meadow.Hardware;
using System;
using System.Threading.Tasks;

namespace Clima_SQLite_Demo
{
    public class MeadowApp : App<F7CoreComputeV2>
    {
        bool isWiFi = true;

        public override async Task Initialize()
        {
            await ClimateMonitorAgent.Instance.Initialize();

            if (isWiFi)
            {
                try
                {
                    var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
                    wifi.NetworkConnected += NetworkConnected;
                    await wifi.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD, TimeSpan.FromSeconds(45));
                }
                catch (Exception ex)
                {
                    Resolver.Log.Error(ex.Message);
                }
            }
            else
            {
                BluetoothServer.Instance.Initialize();
            }
        }

        private void NetworkConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
        {
            var mapleServer = new MapleServer(sender.IpAddress, 5417, true, logger: Resolver.Log);
            mapleServer.Start();
        }
    }
}