using Clima_reTerminal.Client;
using Clima_reTerminal.Models;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace Clima_reTerminal.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private MeasureType LeftSeriesSelected = MeasureType.Temperature;
    private MeasureType RightSeriesSelected = MeasureType.Humidity;
    private MeasureType SeriesSelected;

    public ObservableCollection<MeasurementData> ClimaLogs { get; set; }

    public ObservableCollection<Pnl> SeriesLeft { get; set; }

    public ObservableCollection<Pnl> SeriesRight { get; set; }

    private string _leftLinearAxisTitle;
    public string LeftLinearAxisTitle
    {
        get => _leftLinearAxisTitle;
        set => this.RaiseAndSetIfChanged(ref _leftLinearAxisTitle, value);
    }

    private string _rightLinearAxisTitle;
    public string RightLinearAxisTitle
    {
        get => _rightLinearAxisTitle;
        set => this.RaiseAndSetIfChanged(ref _rightLinearAxisTitle, value);
    }

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

    public ReactiveCommand<MeasureType, Unit> MeasureTypeCommand { get; set; }

    public MainWindowViewModel()
    {
        LeftLinearAxisTitle = "Temperature (°C)";
        RightLinearAxisTitle = "Humidity (%)";

        ClimaLogs = new ObservableCollection<MeasurementData>();

        SeriesLeft = new ObservableCollection<Pnl>();
        SeriesRight = new ObservableCollection<Pnl>();

        MeasureTypeCommand = ReactiveCommand.Create<MeasureType>(MeasureTypeSelected);

        _ = GetCurrentConditionsSimulated();
        //_ = GetCurrentConditionsViaDigitalTwin();
        //_ = GetCurrentConditionsViaMeadowCloud();
    }

    public void MeasureTypeSelected(MeasureType type)
    {
        if (LeftSeriesSelected == type || RightSeriesSelected == type)
        {
            return;
        }

        SeriesSelected = type;

        if (type == MeasureType.PowerSupply)
        {
            LeftLinearAxisTitle = "Solar Voltage (V)";
            RightLinearAxisTitle = "Battery Voltage (V)";
            LeftSeriesSelected = MeasureType.PowerSupply;
            RightSeriesSelected = MeasureType.PowerSupply;
            SeriesLeft.Clear();
            SeriesRight.Clear();
        }
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

            ClimaLogs.Add(sensorReading);

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

            SeriesLeft.Add(new Pnl(dateTime, LeftSeriesSelected switch
            {
                MeasureType.Temperature => sensorReading.Temperature,
                MeasureType.Humidity => sensorReading.Humidity,
                MeasureType.Pressure => sensorReading.Pressure,
                MeasureType.Wind => sensorReading.WindSpeed,
                MeasureType.Rain => sensorReading.Rain,
                MeasureType.Illuminance => sensorReading.Light,
                MeasureType.PowerSupply => sensorReading.SolarVoltage,
                _ => throw new NotImplementedException()
            }));
            SeriesRight.Add(new Pnl(dateTime, RightSeriesSelected switch
            {
                MeasureType.Temperature => sensorReading.Temperature,
                MeasureType.Humidity => sensorReading.Humidity,
                MeasureType.Pressure => sensorReading.Pressure,
                MeasureType.Wind => sensorReading.WindSpeed,
                MeasureType.Rain => sensorReading.Rain,
                MeasureType.Illuminance => sensorReading.Light,
                MeasureType.PowerSupply => sensorReading.BatteryVoltage,
                _ => throw new NotImplementedException()
            }));

            await Task.Delay(TimeSpan.FromSeconds(10));

            if (ClimaLogs.Count > 10)
            {
                SeriesLeft.RemoveAt(0);
                SeriesRight.RemoveAt(0);
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

            SeriesLeft.Add(new Pnl(dateTime, sensorReading.Temperature));
            SeriesRight.Add(new Pnl(dateTime, sensorReading.Humidity));

            await Task.Delay(TimeSpan.FromSeconds(10));

            if (ClimaLogs.Count > 10)
            {
                SeriesLeft.RemoveAt(0);
                SeriesRight.RemoveAt(0);
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
                SeriesLeft.Clear();
                SeriesRight.Clear();

                foreach (var reading in sensorReadings.Take(10))
                {
                    SeriesLeft.Add(new Pnl(reading.record.timestamp.AddHours(TIMEZONE_OFFSET), reading.record.measurements.Temperature));
                    SeriesRight.Add(new Pnl(reading.record.timestamp.AddHours(TIMEZONE_OFFSET), reading.record.measurements.Humidity));
                }

                Temperature = $"{ClimaLogs[0].Temperature:N1}°C";
                Humidity = $"{ClimaLogs[0].Humidity:N1}%";
            }


            await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }
}

public enum MeasureType
{
    Temperature,
    Humidity,
    Pressure,
    Wind,
    Rain,
    Illuminance,
    PowerSupply
}