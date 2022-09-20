using CommonContracts.Models;
using Meadow.Foundation.Web.Maple;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

namespace MobileClima.ViewModel
{
    public class MapleViewModel : BaseViewModel
    {
        public MapleClient client { get; private set; }

        public ObservableCollection<TemperatureModel> TemperatureLog { get; set; }

        public ObservableCollection<ClimateModel> WeatherLog { get; set; }

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

        public ICommand SearchServersCommand { get; private set; }

        public ICommand CmdReloadTemperatureLog { get; private set; }

        public MapleViewModel(bool isClimaPro)
        {
            IsClimaPro = isClimaPro;

            if (isClimaPro)
            {
                WeatherLog = new ObservableCollection<ClimateModel>();
                //WeatherLog.Add(new ClimateModel()
                //{
                //    Date = "2022-01-21 10:00:00 AM",
                //    Temperature = "10°C",
                //    Pressure = "1035mB",
                //    Humidity = "93%",
                //    WindSpeed = "6Kmh",
                //    WindDirection = "N",
                //    Rain = "1mm"
                //});
                //WeatherLog.Add(new ClimateModel()
                //{
                //    Date = "2022-01-21 11:00:00 AM",
                //    Temperature = "12°C",
                //    Pressure = "1040mB",
                //    Humidity = "94%",
                //    WindSpeed = "7Kmh",
                //    WindDirection = "N",
                //    Rain = "1mm"
                //});
                //WeatherLog.Add(new ClimateModel()
                //{
                //    Date = "2022-01-21 12:00:00 PM",
                //    Temperature = "14°C",
                //    Pressure = "1045mB",
                //    Humidity = "95%",
                //    WindSpeed = "5Kmh",
                //    WindDirection = "N",
                //    Rain = "1mm"
                //});
            }
            else
            {
                TemperatureLog = new ObservableCollection<TemperatureModel>();
                //TemperatureLog.Add(new TemperatureModel() 
                //{ 
                //    DateTime = DateTime.Now,
                //    Temperature = "20 C"
                //});
                //TemperatureLog.Add(new TemperatureModel()
                //{
                //    DateTime = DateTime.Now,
                //    Temperature = "22 C"
                //});
                //TemperatureLog.Add(new TemperatureModel()
                //{
                //    DateTime = DateTime.Now,
                //    Temperature = "21 C"
                //});
                //TemperatureLog.Add(new TemperatureModel()
                //{
                //    DateTime = DateTime.Now,
                //    Temperature = "24 C"
                //});
            }

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

        async Task GetServers()
        {
            if (IsScanning)
                return;
            IsScanning = true;

            try
            {
                IsServerListEmpty = false;

                await client.StartScanningForAdvertisingServers();

                //HostList.Add(new ServerModel() { Name = "Meadow (192.168.1.81)", IpAddress = "192.168.1.81" });

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

        async Task GetTemperatureLogs()
        {
            try
            {
                var response = await client.GetAsync(
                    hostAddress: SelectedServer != null ? SelectedServer.IpAddress : IpAddress,
                    port: ServerPort,
                    endPoint: "gettemperaturelogs",
                    param: null,
                    value: null);

                if (response == null)
                    return;

                var values = System.Text.Json.JsonSerializer.Deserialize<List<TemperatureModel>>(response);

                foreach (var value in values)
                {
                    TemperatureLog.Add(value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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