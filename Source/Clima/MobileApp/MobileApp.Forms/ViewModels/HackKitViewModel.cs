using Meadow.Foundation.Web.Maple.Client;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows.Input;
using WildernessLabs.Clima.App.Models;
using MobileApp.ViewModels;
using Xamarin.Forms;

namespace WildernessLabs.Clima.App
{
    public class HackKitViewModel : BaseViewModel
    {
        public MapleClient client { get; private set; }

        public ObservableCollection<ClimaModel> TemperatureLog { get; set; }

        int _serverPort;
        public int ServerPort
        {
            get => _serverPort;
            set { _serverPort = value; OnPropertyChanged(nameof(ServerPort)); }
        }

        bool _isScanning;
        public bool IsScanning 
        {
            get => _isScanning;
            set { _isScanning = value; OnPropertyChanged(nameof(IsScanning)); }
        }

        bool isRefreshing;
        public bool IsRefreshing
        {
            get => isRefreshing;
            set { isRefreshing = value; OnPropertyChanged(nameof(IsRefreshing)); }
        }

        bool _isServerListEmpty;
        public bool IsServerListEmpty
        {
            get => _isServerListEmpty;
            set { _isServerListEmpty = value; OnPropertyChanged(nameof(IsServerListEmpty)); }
        }

        string ipAddress;
        public string IpAddress
        {
            get => ipAddress;
            set { ipAddress = value; OnPropertyChanged(nameof(IpAddress)); }
        }

        ServerModel _selectedServer;
        public ServerModel SelectedServer
        {
            get => _selectedServer;
            set
            {
                if (value == null) return;
                _selectedServer = value;
                IpAddress = _selectedServer.IpAddress;
                OnPropertyChanged(nameof(SelectedServer));
            }
        }

        public ObservableCollection<ServerModel> HostList { get; set; }

        public ICommand SearchServersCommand { set; get; }

        public ICommand CmdReloadTemperatureLog { get; private set; }

        public HackKitViewModel()
        {
            TemperatureLog = new ObservableCollection<ClimaModel>();

            HostList = new ObservableCollection<ServerModel>();            

            ServerPort = 5417;

            client = new MapleClient();
            client.Servers.CollectionChanged += ServersCollectionChanged;

            CmdReloadTemperatureLog = new Command(async () =>
            {
                await GetTemperatureLogs();

                IsRefreshing = false;
            });

            SearchServersCommand = new Command(async () => await GetServers());
        }

        void ServersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (ServerModel server in e.NewItems)
                    {
                        HostList.Add(new ServerModel() { Name = $"{server.Name} ({server.IpAddress})", IpAddress = server.IpAddress });
                        Console.WriteLine($"'{server.Name}' @ ip:[{server.IpAddress}]");
                    }
                    break;
            }
        }

        async Task GetTemperatureLogs()
        {
            try
            {
                var response = await client.GetAsync(SelectedServer != null ? SelectedServer.IpAddress : IpAddress, ServerPort, "GetTemperature", null, null);
                var value = JsonConvert.DeserializeObject<ClimaModel>(response);
                TemperatureLog.Add(value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        async Task GetServers()
        {
            if (IsScanning)
                return;
            IsScanning = true;

            try
            {
                IsServerListEmpty = false;

                await client.StartScanningForAdvertisingServers();

                HostList.Add(new ServerModel() { Name = "Meadow (192.168.1.85)", IpAddress = "192.168.1.85" });

                if (HostList.Count == 0)
                {
                    IsServerListEmpty = true;
                }
                else
                {
                    IsServerListEmpty = false;
                    SelectedServer = HostList[0];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                IsScanning = false;
            }
        }

        public async Task LoadData()
        {
            await GetServers();

            if (SelectedServer != null)
            {
                await GetTemperatureLogs();
            }
        }
    }
}