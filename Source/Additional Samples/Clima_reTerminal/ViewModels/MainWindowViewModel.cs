using Clima_reTerminal.Client;
using Clima_reTerminal.Models;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Clima_reTerminal.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<Pnl> TemperatureLogs { get; set; }

        public ObservableCollection<Pnl> HumidityLogs { get; set; }

        public ObservableCollection<Pnl> PressureLogs { get; set; }

        private string _temperature;
        public string Temperature
        {
            get => _temperature;
            set => this.RaiseAndSetIfChanged(ref _temperature, value);
        }

        private string _rain;
        public string Rain
        {
            get => _rain;
            set => this.RaiseAndSetIfChanged(ref _rain, value);
        }

        private string _light;
        public string Light
        {
            get => _light;
            set => this.RaiseAndSetIfChanged(ref _light, value);
        }

        private string _solarVoltage;
        public string SolarVoltage
        {
            get => _solarVoltage;
            set => this.RaiseAndSetIfChanged(ref _solarVoltage, value);
        }

        private string _humidity;
        public string Humidity
        {
            get => _humidity;
            set => this.RaiseAndSetIfChanged(ref _humidity, value);
        }

        private string _windSpeed;
        public string WindSpeed
        {
            get => _windSpeed;
            set => this.RaiseAndSetIfChanged(ref _windSpeed, value);
        }

        private string _windDirection;
        public string WindDirection
        {
            get => _windDirection;
            set => this.RaiseAndSetIfChanged(ref _windDirection, value);
        }

        private string _pressure;
        public string Pressure
        {
            get => _pressure;
            set => this.RaiseAndSetIfChanged(ref _pressure, value);
        }

        private string _co2Level;
        public string Co2Level
        {
            get => _co2Level;
            set => this.RaiseAndSetIfChanged(ref _co2Level, value);
        }

        private string _batteryVoltage;
        public string BatteryVoltage
        {
            get => _batteryVoltage;
            set => this.RaiseAndSetIfChanged(ref _batteryVoltage, value);
        }

        public MainWindowViewModel()
        {
            TemperatureLogs = new ObservableCollection<Pnl>();

            HumidityLogs = new ObservableCollection<Pnl>();

            PressureLogs = new ObservableCollection<Pnl>();

            _ = GetCurrentConditionsSimulated();
            //_ = GetCurrentConditionsViaDigitalTwin();
            //_ = GetCurrentConditionsViaMeadowCloud();
        }

        async Task GetCurrentConditionsSimulated()
        {
            var random = new Random();

            while (true)
            {
                var dateTime = DateTime.Now;

                var sensorReading = new MeasurementData()
                {
                    Temperature = Math.Round(random.NextDouble() * 40 - 10, 2), // Random temperature between -10 and 30°C
                    Rain = Math.Round(random.NextDouble() * 100, 2), // Random rainfall between 0 and 100 mm
                    Light = Math.Round(random.NextDouble() * 1000, 2), // Random light level between 0 and 1000 lux
                    SolarVoltage = Math.Round(random.NextDouble() * 20, 2), // Random solar voltage between 0 and 20 V
                    Humidity = Math.Round(random.NextDouble() * 100, 2), // Random humidity between 0 and 100%
                    WindSpeed = Math.Round(random.NextDouble() * 40, 2), // Random wind speed between 0 and 40 m/s
                    WindDirection = Math.Round(random.NextDouble() * 360, 2), // Random wind direction between 0 and 360 degrees
                    Pressure = Math.Round(random.NextDouble() * 50 + 950, 2), // Random pressure between 950 and 1000 hPa
                    Co2Level = Math.Round(random.NextDouble() * 2000 + 300, 2), // Random CO2 level between 300 and 2300 ppm
                    BatteryVoltage = Math.Round(random.NextDouble() * 12 + 1, 2) // Random battery voltage between 1 and 13 V
                };

                Temperature = $"{sensorReading.Temperature:N0}";
                Rain = $"{sensorReading.Rain:N0}";
                Light = $"{sensorReading.Light:N0}";
                SolarVoltage = $"{sensorReading.SolarVoltage:N0}";
                Humidity = $"{sensorReading.Humidity:N1}";
                WindSpeed = $"{sensorReading.WindSpeed:N0}";
                WindDirection = $"{sensorReading.WindDirection:N0}";
                Pressure = $"{sensorReading.Pressure:N1}";
                Co2Level = $"{sensorReading.Co2Level:N0}";
                BatteryVoltage = $"{sensorReading.BatteryVoltage:N0}";

                TemperatureLogs.Add(new Pnl(dateTime, sensorReading.Temperature));
                HumidityLogs.Add(new Pnl(dateTime, sensorReading.Humidity));
                PressureLogs.Add(new Pnl(dateTime, sensorReading.Pressure));

                await Task.Delay(TimeSpan.FromSeconds(10));

                if (TemperatureLogs.Count > 10)
                {
                    TemperatureLogs.RemoveAt(0);
                    HumidityLogs.RemoveAt(0);
                    PressureLogs.RemoveAt(0);
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
                    Temperature = $"{sensorReading.Temperature:N0}°C";
                    Humidity = $"{sensorReading.Humidity:N0}%";
                    Pressure = $"{sensorReading.Pressure:N0}%";
                }

                Temperature = $"{sensorReading.Temperature:N0}°C";
                Humidity = $"{sensorReading.Humidity}%";
                Pressure = $"{sensorReading.Pressure:N0}%";

                TemperatureLogs.Add(new Pnl(dateTime, sensorReading.Temperature));
                HumidityLogs.Add(new Pnl(dateTime, sensorReading.Humidity));
                PressureLogs.Add(new Pnl(dateTime, sensorReading.Pressure));

                await Task.Delay(TimeSpan.FromSeconds(10));

                if (TemperatureLogs.Count > 10)
                {
                    TemperatureLogs.RemoveAt(0);
                    HumidityLogs.RemoveAt(0);
                    PressureLogs.RemoveAt(0);
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
                    PressureLogs.Clear();

                    foreach (var reading in sensorReadings.Take(10))
                    {
                        TemperatureLogs.Add(new Pnl(reading.record.timestamp.AddHours(TIMEZONE_OFFSET), reading.record.measurements.Temperature));
                        HumidityLogs.Add(new Pnl(reading.record.timestamp.AddHours(TIMEZONE_OFFSET), reading.record.measurements.Humidity));
                        PressureLogs.Add(new Pnl(reading.record.timestamp.AddHours(TIMEZONE_OFFSET), reading.record.measurements.Pressure));
                    }

                    Temperature = $"{TemperatureLogs[0].Value:N1}°C";
                    Humidity = $"{HumidityLogs[0].Value:N1}%";
                    Pressure = $"{PressureLogs[0].Value:N1}%";
                }


                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }
    }
}