using Clima_reTerminal.Client;
using Clima_reTerminal.Models;
using Clima_reTerminal.Utils;
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

    public ObservableCollection<Pnl> LeftSeries { get; set; }

    public ObservableCollection<Pnl> RightSeries { get; set; }

    private bool _isSelectLeftButtonVisible;
    public bool IsSelectLeftButtonVisible
    {
        get => _isSelectLeftButtonVisible;
        set => this.RaiseAndSetIfChanged(ref _isSelectLeftButtonVisible, value);
    }

    private bool _isSelectRightButtonVisible;
    public bool IsSelectRightButtonVisible
    {
        get => _isSelectRightButtonVisible;
        set => this.RaiseAndSetIfChanged(ref _isSelectRightButtonVisible, value);
    }

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

    public ReactiveCommand<Unit, Unit> SelectLeftChartCommand { get; set; }

    public ReactiveCommand<Unit, Unit> SelectRightChartCommand { get; set; }

    public MainWindowViewModel()
    {
        LeftLinearAxisTitle = "Temperature (°C)";
        LeftSeriesSelected = MeasureType.Temperature;

        RightLinearAxisTitle = "Humidity (%)";
        RightSeriesSelected = MeasureType.Humidity;

        ClimaLogs = new ObservableCollection<MeasurementData>();

        LeftSeries = new ObservableCollection<Pnl>();
        RightSeries = new ObservableCollection<Pnl>();

        MeasureTypeCommand = ReactiveCommand.Create<MeasureType>(MeasureTypeSelected);

        SelectLeftChartCommand = ReactiveCommand.Create(ShowMeasureTypeOnLeftChart);

        SelectRightChartCommand = ReactiveCommand.Create(ShowMeasureTypeOnRightChart);

        //_ = GetCurrentConditionsSimulated();
        //_ = GetCurrentConditionsViaDigitalTwin();
        _ = GetCurrentConditionsViaMeadowCloud();
    }

    private void MeasureTypeSelected(MeasureType type)
    {
        if (LeftSeriesSelected == type || RightSeriesSelected == type)
        {
            return;
        }

        if (type == MeasureType.PowerSupply)
        {
            LeftLinearAxisTitle = "Solar Voltage (V)";
            LeftSeriesSelected = MeasureType.PowerSupply;
            LeftSeries.Clear();

            RightLinearAxisTitle = "Battery Voltage (V)";
            RightSeriesSelected = MeasureType.PowerSupply;
            RightSeries.Clear();
        }
        else
        {
            IsSelectLeftButtonVisible = true;
            IsSelectRightButtonVisible = true;
            SeriesSelected = type;
        }
    }

    private void ShowMeasureTypeOnLeftChart()
    {
        LeftSeries.Clear();
        RightSeries.Clear();

        IsSelectLeftButtonVisible = false;
        IsSelectRightButtonVisible = false;
        LeftSeriesSelected = SeriesSelected;

        switch (SeriesSelected)
        {
            case MeasureType.Temperature:
                LeftLinearAxisTitle = "Temperature (°C)";
                break;
            case MeasureType.Humidity:
                LeftLinearAxisTitle = "Humidity (%)";
                break;
            case MeasureType.Pressure:
                LeftLinearAxisTitle = "Pressure (hPa)";
                break;
            case MeasureType.Wind:
                LeftLinearAxisTitle = "Wind Speed (m/s)";
                break;
            case MeasureType.Rain:
                LeftLinearAxisTitle = "Rain Volume (mm)";
                break;
            case MeasureType.Illuminance:
                LeftLinearAxisTitle = "Illuminance (Lux)";
                break;
            case MeasureType.PowerSupply:
                LeftLinearAxisTitle = "Solar Power (V)";
                break;
        }
    }

    private void ShowMeasureTypeOnRightChart()
    {
        LeftSeries.Clear();
        RightSeries.Clear();

        IsSelectLeftButtonVisible = false;
        IsSelectRightButtonVisible = false;
        RightSeriesSelected = SeriesSelected;

        switch (SeriesSelected)
        {
            case MeasureType.Temperature:
                RightLinearAxisTitle = "Temperature (°C)";
                break;
            case MeasureType.Humidity:
                RightLinearAxisTitle = "Humidity (%)";
                break;
            case MeasureType.Pressure:
                RightLinearAxisTitle = "Pressure (hPa)";
                break;
            case MeasureType.Wind:
                RightLinearAxisTitle = "Wind Speed (m/s)";
                break;
            case MeasureType.Rain:
                RightLinearAxisTitle = "Rain Volume (mm)";
                break;
            case MeasureType.Illuminance:
                RightLinearAxisTitle = "Illuminance (Lux)";
                break;
            case MeasureType.PowerSupply:
                RightLinearAxisTitle = "Battery Voltage (V)";
                break;
        }
    }

    private async Task GetCurrentConditionsSimulated()
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

            LeftSeries.Add(new Pnl(dateTime, LeftSeriesSelected switch
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
            RightSeries.Add(new Pnl(dateTime, RightSeriesSelected switch
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
                LeftSeries.RemoveAt(0);
                RightSeries.RemoveAt(0);
            }
        }
    }

    private async Task GetCurrentConditionsViaMeadowCloud()
    {
        int TIMEZONE_OFFSET = -7;

        while (true)
        {
            var sensorReadings = await RestClient.GetSensorReadings();

            if (sensorReadings != null && sensorReadings.Count > 0)
            {
                LeftSeries.Clear();
                RightSeries.Clear();

                foreach (var reading in sensorReadings.Take(10))
                {
                    LeftSeries.Add(new Pnl(reading.record.timestamp.AddHours(TIMEZONE_OFFSET),
                        LeftSeriesSelected switch
                        {
                            MeasureType.Temperature => reading.record.measurements.Temperature,
                            MeasureType.Humidity => reading.record.measurements.Humidity,
                            MeasureType.Pressure => reading.record.measurements.Pressure,
                            MeasureType.Wind => reading.record.measurements.WindSpeed,
                            MeasureType.Rain => reading.record.measurements.Rain,
                            MeasureType.Illuminance => reading.record.measurements.Light,
                            MeasureType.PowerSupply => reading.record.measurements.SolarVoltage,
                            _ => throw new NotImplementedException()
                        }));

                    RightSeries.Add(new Pnl(reading.record.timestamp.AddHours(TIMEZONE_OFFSET),
                        RightSeriesSelected switch
                        {
                            MeasureType.Temperature => reading.record.measurements.Temperature,
                            MeasureType.Humidity => reading.record.measurements.Humidity,
                            MeasureType.Pressure => reading.record.measurements.Pressure,
                            MeasureType.Wind => reading.record.measurements.WindSpeed,
                            MeasureType.Rain => reading.record.measurements.Rain,
                            MeasureType.Illuminance => reading.record.measurements.Light,
                            MeasureType.PowerSupply => reading.record.measurements.BatteryVoltage,
                            _ => throw new NotImplementedException()
                        }));
                }

                Temperature = $"{ClimaLogs[0].Temperature:N1}";
                Rain = $"{ClimaLogs[0].Rain:N0}";
                Light = $"{ClimaLogs[0].Light:N0}";
                SolarVoltage = $"{ClimaLogs[0].SolarVoltage:N1}";
                Humidity = $"{ClimaLogs[0].Humidity:N1}";
                WindSpeed = $"{ClimaLogs[0].WindSpeed:N0}";
                WindDirection = $"{ClimaLogs[0].WindDirection:N0}";
                Pressure = $"{ClimaLogs[0].Pressure:N1}";
                Co2Level = $"{ClimaLogs[0].Co2Level:N0}";
                BatteryVoltage = $"{ClimaLogs[0].BatteryVoltage:N1}";
            }

            await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }

    private async Task GetCurrentConditionsViaDigitalTwin()
    {
        var random = new Random();

        while (true)
        {
            var dateTime = DateTime.Now;

            var sensorReading = await DigitalTwinClient.GetDigitalTwinData();
            if (sensorReading != null)
            {
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
            }

            LeftSeries.Add(new Pnl(dateTime, sensorReading.Temperature));
            RightSeries.Add(new Pnl(dateTime, sensorReading.Humidity));

            await Task.Delay(TimeSpan.FromSeconds(10));

            if (ClimaLogs.Count > 10)
            {
                LeftSeries.RemoveAt(0);
                RightSeries.RemoveAt(0);
            }
        }
    }
}