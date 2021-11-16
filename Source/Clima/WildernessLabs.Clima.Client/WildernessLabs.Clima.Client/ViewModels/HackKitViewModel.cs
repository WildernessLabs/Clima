using Meadow.Foundation.Maple.Web.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows.Input;
using WildernessLabs.Clima.App.Models;
using WildernessLabs.Clima.Client.ViewModels;
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

        bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(nameof(IsBusy)); }
        }

        bool _isScanning;
        public bool IsScanning 
        {
            get => _isScanning;
            set { _isScanning = value; OnPropertyChanged(nameof(IsScanning)); }
        }

        bool _isServerListEmpty;
        public bool IsServerListEmpty
        {
            get => _isServerListEmpty;
            set { _isServerListEmpty = value; OnPropertyChanged(nameof(IsServerListEmpty)); }
        }

        ServerModel _selectedServer;
        public ServerModel SelectedServer
        {
            get => _selectedServer;
            set
            {
                if (value == null) return;
                _selectedServer = value;
                OnPropertyChanged(nameof(SelectedServer));
            }
        }

        public ObservableCollection<ServerModel> HostList { get; set; }

        public ICommand SendCommand { set; get; }

        public ICommand SearchServersCommand { set; get; }

        public HackKitViewModel()
        {
            HostList = new ObservableCollection<ServerModel>();
            //HostList.Add(new ServerModel() { Name="Meadow (192.168.1.73)", IpAddress="192.168.1.73" });

            ServerPort = 5417;

            client = new MapleClient();
            client.Servers.CollectionChanged += ServersCollectionChanged;

            SearchServersCommand = new Command(async () => await GetServers());
        }

        async Task GetTemperatureLogs()
        {
            var response = await client.GetAsync(SelectedServer.IpAddress, ServerPort, "GetTemperatureLogs", null, null);
            var values = JsonConvert.DeserializeObject<List<ClimaModel>>(response);

            foreach (var value in values)
            {
                TemperatureLog.Add(value);
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

        //public ObservableCollection<ClimaModel> ClimateList { get; set; }

        //string ipAddress;
        //public string IpAddress 
        //{
        //    get => ipAddress;
        //    set { ipAddress = value; OnPropertyChanged(nameof(IpAddress)); } 
        //}

        //bool isRefreshing;
        //public bool IsRefreshing
        //{
        //    get => isRefreshing;
        //    set { isRefreshing = value; OnPropertyChanged(nameof(IsRefreshing)); }
        //}

        //public Command GetHumidityCommand { private set; get; }

        //public HackKitViewModel() 
        //{
        //    ClimateList = new ObservableCollection<ClimaModel>();            

        //    GetHumidityCommand = new Command(async (s) => await GetReadingsAsync());
        //}

        //async Task GetReadingsAsync()
        //{
        //    if (string.IsNullOrEmpty(IpAddress))
        //        throw new InvalidEnumArgumentException("You must enter a valid IP address.");

        //    ClimateList.Clear();

        //    var response = await NetworkManager.GetAsync(IpAddress);

        //    if (response != null)
        //    {
        //        string json = await response.Content.ReadAsStringAsync();
        //        var values = (List<ClimateReading>)System.Text.Json.JsonSerializer.Deserialize(json, typeof(List<ClimateReading>));

        //        foreach (ClimateReading value in values)
        //        {
        //            ClimateList.Add(new ClimaModel() { Date = value.TimeOfReading.ToString(), Temperature = value.TempC });
        //        }
        //    }

        //    IsRefreshing = false;
        //}

        //#region INotifyPropertyChanged Implementation
        //public event PropertyChangedEventHandler PropertyChanged;
        //public void OnPropertyChanged([CallerMemberName] string name = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //}
        //#endregion
    }
}