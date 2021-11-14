using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WildernessLabs.Clima.Client.Utils;
using Xamarin.Forms;

namespace WildernessLabs.Clima.Client.ViewModels
{
    public class ProKitViewModel : BaseViewModel
    {
        int listenTimeout = 5000;

        ushort DEVICE_ID = 896;

        IAdapter adapter;
        IService service;

        ICharacteristic tempCharacteristic;
        ICharacteristic pressureCharacteristic;
        ICharacteristic humidityCharacteristic;
        ICharacteristic windSpeedCharacteristic;
        ICharacteristic windDirectionCharacteristic;

        public ObservableCollection<IDevice> DeviceList { get; set; }

        IDevice deviceSelected;
        public IDevice DeviceSelected
        {
            get => deviceSelected;
            set { deviceSelected = value; OnPropertyChanged(nameof(DeviceSelected)); }
        }

        bool isScanning;
        public bool IsScanning
        {
            get => isScanning;
            set { isScanning = value; OnPropertyChanged(nameof(IsScanning)); }
        }

        bool isConnected;
        public bool IsConnected
        {
            get => isConnected;
            set { isConnected = value; OnPropertyChanged(nameof(IsConnected)); }
        }

        bool isDeviceListEmpty;
        public bool IsDeviceListEmpty
        {
            get => isDeviceListEmpty;
            set { isDeviceListEmpty = value; OnPropertyChanged(nameof(IsDeviceListEmpty)); }
        }

        string temperatureValue;
        public string TemperatureValue
        {
            get => temperatureValue;
            set { temperatureValue = value; OnPropertyChanged(nameof(TemperatureValue)); }
        }

        string pressureValue;
        public string PressureValue
        {
            get => pressureValue;
            set { pressureValue = value; OnPropertyChanged(nameof(PressureValue)); }
        }

        string humidityValue;
        public string HumidityValue
        {
            get => humidityValue;
            set { humidityValue = value; OnPropertyChanged(nameof(HumidityValue)); }
        }

        string windSpeedValue;
        public string WindSpeedValue
        {
            get => windSpeedValue;
            set { windSpeedValue = value; OnPropertyChanged(nameof(WindSpeedValue)); }
        }

        string windDirectionValue;
        public string WindDirectionValue
        {
            get => windDirectionValue;
            set { windDirectionValue = value; OnPropertyChanged(nameof(WindDirectionValue)); }
        }

        public ICommand CmdToggleConnection { get; set; }

        public ICommand CmdSearchForDevices { get; set; }

        public ICommand CmdGetClimaStatus { get; set; }

        public ProKitViewModel()
        {
            DeviceList = new ObservableCollection<IDevice>();

            adapter = CrossBluetoothLE.Current.Adapter;
            adapter.ScanTimeout = listenTimeout;
            adapter.ScanMode = ScanMode.LowLatency;
            adapter.DeviceConnected += AdapterDeviceConnected;
            adapter.DeviceDiscovered += AdapterDeviceDiscovered;
            adapter.DeviceDisconnected += AdapterDeviceDisconnected;

            CmdToggleConnection = new Command(async () => await ToggleConnection());

            CmdSearchForDevices = new Command(async () => await DiscoverDevices());

            CmdGetClimaStatus = new Command(async () => await GetClimaStatus());
        }

        void AdapterDeviceDisconnected(object sender, DeviceEventArgs e)
        {
            IsConnected = false;
        }

        async void AdapterDeviceConnected(object sender, DeviceEventArgs e)
        {
            IsConnected = true;

            IDevice device = e.Device;

            var services = await device.GetServicesAsync();

            foreach (var serviceItem in services)
            {
                if (UuidToUshort(serviceItem.Id.ToString()) == DEVICE_ID)
                {
                    service = serviceItem;
                }
            }

            tempCharacteristic = await service.GetCharacteristicAsync(Guid.Parse(CharacteristicsConstants.TEMPERATURE));
            pressureCharacteristic = await service.GetCharacteristicAsync(Guid.Parse(CharacteristicsConstants.PRESSURE));
            humidityCharacteristic = await service.GetCharacteristicAsync(Guid.Parse(CharacteristicsConstants.HUMIDITY));
            windSpeedCharacteristic = await service.GetCharacteristicAsync(Guid.Parse(CharacteristicsConstants.WIND_SPEED));
            windDirectionCharacteristic = await service.GetCharacteristicAsync(Guid.Parse(CharacteristicsConstants.WIND_DIRECTION));
        }

        async void AdapterDeviceDiscovered(object sender, DeviceEventArgs e)
        {
            if (DeviceList.FirstOrDefault(x => x.Name == e.Device.Name) == null &&
                !string.IsNullOrEmpty(e.Device.Name))
            {
                DeviceList.Add(e.Device);
            }

            if (e.Device.Name == "Meadow Clima")
            {
                await adapter.StopScanningForDevicesAsync();
                IsDeviceListEmpty = false;
                DeviceSelected = e.Device;
            }
        }

        async Task ScanTimeoutTask()
        {
            await Task.Delay(listenTimeout);
            await adapter.StopScanningForDevicesAsync();
            IsScanning = false;
        }

        async Task DiscoverDevices()
        {
            try
            {
                IsScanning = true;

                var tasks = new Task[]
                {
                    ScanTimeoutTask(),
                    adapter.StartScanningForDevicesAsync()
                };

                await Task.WhenAny(tasks);
            }
            catch (DeviceConnectionException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        async Task ToggleConnection()
        {
            try
            {
                if (IsConnected)
                {
                    await adapter.DisconnectDeviceAsync(DeviceSelected);
                    IsConnected = false;
                }
                else
                {
                    await adapter.ConnectToDeviceAsync(DeviceSelected);
                    IsConnected = true;
                }
            }
            catch (DeviceConnectionException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        async Task GetClimaStatus() 
        {
            var bytes = await tempCharacteristic.ReadAsync();

            TemperatureValue = System.Text.Encoding.Default.GetString(await tempCharacteristic.ReadAsync()).Split(';')[0];
            PressureValue = System.Text.Encoding.Default.GetString(await pressureCharacteristic.ReadAsync()).Split(';')[0];
            HumidityValue = System.Text.Encoding.Default.GetString(await humidityCharacteristic.ReadAsync()).Split(';')[0];
            WindSpeedValue = System.Text.Encoding.Default.GetString(await windSpeedCharacteristic.ReadAsync()).Split(';')[0];
            WindDirectionValue = System.Text.Encoding.Default.GetString(await windDirectionCharacteristic.ReadAsync()).Split(';')[0];
        }

        protected int UuidToUshort(string uuid)
        {
            return int.Parse(uuid.Substring(4, 4), System.Globalization.NumberStyles.HexNumber); ;
        }
    }
}