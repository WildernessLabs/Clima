using Clima_reTerminal.Client;
using Clima_reTerminal.Models;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace Clima_reTerminal.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private bool _isLightsOn;
        public bool IsLightsOn
        {
            get => _isLightsOn;
            set => this.RaiseAndSetIfChanged(ref _isLightsOn, value);
        }

        private bool _isHeaterOn;
        public bool IsHeaterOn
        {
            get => _isHeaterOn;
            set => this.RaiseAndSetIfChanged(ref _isHeaterOn, value);
        }

        private bool _isVentilationOn;
        public bool IsVentilationOn
        {
            get => _isVentilationOn;
            set => this.RaiseAndSetIfChanged(ref _isVentilationOn, value);
        }

        private bool _isSprinklerOn;
        public bool IsSprinklerOn
        {
            get => _isSprinklerOn;
            set => this.RaiseAndSetIfChanged(ref _isSprinklerOn, value);
        }

        private string _currentTemperature;
        public string CurrentTemperature
        {
            get => _currentTemperature;
            set => this.RaiseAndSetIfChanged(ref _currentTemperature, value);
        }

        private string _currentHumidity;
        public string CurrentHumidity
        {
            get => _currentHumidity;
            set => this.RaiseAndSetIfChanged(ref _currentHumidity, value);
        }

        private string _currentSoilMoisture;
        public string CurrentSoilMoisture
        {
            get => _currentSoilMoisture;
            set => this.RaiseAndSetIfChanged(ref _currentSoilMoisture, value);
        }
        public ReactiveCommand<Unit, Unit> ToggleLightsCommand { get; set; }

        public ReactiveCommand<Unit, Unit> ToggleHeaterCommand { get; set; }

        public ReactiveCommand<Unit, Unit> ToggleVentilationCommand { get; set; }

        public ReactiveCommand<Unit, Unit> ToggleSprinklerCommand { get; set; }

        public ObservableCollection<Pnl> TemperatureLogs { get; set; }

        public ObservableCollection<Pnl> HumidityLogs { get; set; }

        public ObservableCollection<Pnl> SoilMoistureLogs { get; set; }

        public MainWindowViewModel()
        {
            ToggleLightsCommand = ReactiveCommand.Create(ToggleLights);

            ToggleHeaterCommand = ReactiveCommand.Create(ToggleHeater);

            ToggleVentilationCommand = ReactiveCommand.Create(ToggleVentilation);

            ToggleSprinklerCommand = ReactiveCommand.Create(ToggleSprinkler);

            TemperatureLogs = new ObservableCollection<Pnl>();

            HumidityLogs = new ObservableCollection<Pnl>();

            SoilMoistureLogs = new ObservableCollection<Pnl>();

            //_ = GetCurrentConditionsSimulated();
            //_ = GetCurrentConditionsViaDigitalTwin();
            _ = GetCurrentConditionsViaMeadowCloud();
        }

        async Task GetCurrentConditionsSimulated()
        {
            var random = new Random();

            while (true)
            {
                var dateTime = DateTime.Now;

                var sensorReading = new GreenhouseModel()
                {
                    TemperatureCelsius = random.Next(26, 28) + random.NextDouble(),
                    HumidityPercentage = random.Next(95, 97),
                    SoilMoisturePercentage = random.Next(75, 77) + random.NextDouble(),
                    IsLightOn = true,
                    IsHeaterOn = true,
                    IsVentilationOn = true,
                    IsSprinklerOn = true,
                };

                CurrentTemperature = $"{sensorReading.TemperatureCelsius:N0}°C";
                CurrentHumidity = $"{sensorReading.HumidityPercentage}%";
                CurrentSoilMoisture = $"{sensorReading.SoilMoisturePercentage:N0}%";

                TemperatureLogs.Add(new Pnl(dateTime, sensorReading.TemperatureCelsius));
                HumidityLogs.Add(new Pnl(dateTime, sensorReading.HumidityPercentage));
                SoilMoistureLogs.Add(new Pnl(dateTime, sensorReading.SoilMoisturePercentage));

                await Task.Delay(TimeSpan.FromSeconds(10));

                if (TemperatureLogs.Count > 10)
                {
                    TemperatureLogs.RemoveAt(0);
                    HumidityLogs.RemoveAt(0);
                    SoilMoistureLogs.RemoveAt(0);
                }
            }
        }

        async Task GetCurrentConditionsViaDigitalTwin()
        {
            var random = new Random();

            while (true)
            {
                var dateTime = DateTime.Now;

                var sensorReading = await DigitalTwinClient.GetDigitalTwinData();
                if (sensorReading != null)
                {
                    CurrentTemperature = $"{sensorReading.TemperatureCelsius:N0}°C";
                    CurrentHumidity = $"{sensorReading.HumidityPercentage:N0}%";
                    CurrentSoilMoisture = $"{sensorReading.SoilMoisturePercentage:N0}%";
                    IsLightsOn = sensorReading.IsLightOn;
                    IsHeaterOn = sensorReading.IsHeaterOn;
                    IsSprinklerOn = sensorReading.IsSprinklerOn;
                    IsVentilationOn = sensorReading.IsVentilationOn;
                }

                CurrentTemperature = $"{sensorReading.TemperatureCelsius:N0}°C";
                CurrentHumidity = $"{sensorReading.HumidityPercentage}%";
                CurrentSoilMoisture = $"{sensorReading.SoilMoisturePercentage:N0}%";

                TemperatureLogs.Add(new Pnl(dateTime, sensorReading.TemperatureCelsius));
                HumidityLogs.Add(new Pnl(dateTime, sensorReading.HumidityPercentage));
                SoilMoistureLogs.Add(new Pnl(dateTime, sensorReading.SoilMoisturePercentage));

                await Task.Delay(TimeSpan.FromSeconds(10));

                if (TemperatureLogs.Count > 10)
                {
                    TemperatureLogs.RemoveAt(0);
                    HumidityLogs.RemoveAt(0);
                    SoilMoistureLogs.RemoveAt(0);
                }
            }
        }

        async Task GetCurrentConditionsViaMeadowCloud()
        {
            int TIMEZONE_OFFSET = -7;

            while (true)
            {
                var sensorReadings = await RestClient.GetSensorReadings();

                if (sensorReadings != null && sensorReadings.Count > 0)
                {
                    TemperatureLogs.Clear();
                    HumidityLogs.Clear();
                    SoilMoistureLogs.Clear();

                    foreach (var reading in sensorReadings.Take(10))
                    {
                        TemperatureLogs.Add(new Pnl(reading.record.timestamp.AddHours(TIMEZONE_OFFSET), reading.record.measurements.TemperatureCelsius));
                        HumidityLogs.Add(new Pnl(reading.record.timestamp.AddHours(TIMEZONE_OFFSET), reading.record.measurements.HumidityPercent));
                        SoilMoistureLogs.Add(new Pnl(reading.record.timestamp.AddHours(TIMEZONE_OFFSET), reading.record.measurements.SoilMoistureDouble));
                    }

                    CurrentTemperature = $"{TemperatureLogs[0].Value:N1}°C";
                    CurrentHumidity = $"{HumidityLogs[0].Value:N1}%";
                    CurrentSoilMoisture = $"{SoilMoistureLogs[0].Value:N1}%";
                }


                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }

        public async void ToggleLights()
        {
            await DigitalTwinClient.GetDigitalTwinData();

            var res = await RestClient.SendCommand(CultivarCommands.LightControl, !IsLightsOn);
            if (res)
            {
                IsLightsOn = !IsLightsOn;
            }
        }

        public async void ToggleHeater()
        {
            var res = await RestClient.SendCommand(CultivarCommands.HeaterControl, !IsHeaterOn);
            if (res)
            {
                IsHeaterOn = !IsHeaterOn;
            }
        }

        public async void ToggleVentilation()
        {
            var res = await RestClient.SendCommand(CultivarCommands.FanControl, !IsVentilationOn);
            if (res)
            {
                IsVentilationOn = !IsVentilationOn;
            }
        }

        public async void ToggleSprinkler()
        {
            var res = await RestClient.SendCommand(CultivarCommands.ValveControl, !IsSprinklerOn);
            if (res)
            {
                IsSprinklerOn = !IsSprinklerOn;
            }
        }
    }
}