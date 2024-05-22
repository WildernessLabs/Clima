using Clima_reTerminal.Client;
using Clima_reTerminal.Models;
using Clima_reTerminal.Utils;
using ReactiveUI;
using System;
using System.Collections.Generic;
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

    private List<Record> climaLogs;

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
        climaLogs = new List<Record>();

        LeftLinearAxisTitle = "Temperature (°C)";
        LeftSeriesSelected = MeasureType.Temperature;

        RightLinearAxisTitle = "Humidity (%)";
        RightSeriesSelected = MeasureType.Humidity;

        LeftSeries = new ObservableCollection<Pnl>();
        RightSeries = new ObservableCollection<Pnl>();

        MeasureTypeCommand = ReactiveCommand.Create<MeasureType>(MeasureTypeSelected);

        SelectLeftChartCommand = ReactiveCommand.Create(ShowMeasureTypeOnLeftChart);

        SelectRightChartCommand = ReactiveCommand.Create(ShowMeasureTypeOnRightChart);

        _ = GetCurrentConditionsSimulated();
        //_ = GetCurrentConditionsViaMeadowCloud();
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

            RightLinearAxisTitle = "Battery Voltage (V)";
            RightSeriesSelected = MeasureType.PowerSupply;

            UpdateDashboard();
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

        UpdateDashboard();
    }

    private void ShowMeasureTypeOnRightChart()
    {
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

        UpdateDashboard();
    }

    private async Task GetCurrentConditionsSimulated()
    {
        var random = new Random();
        var today = DateTime.Now;

        while (true)
        {
            for (int i = 0; i < 10; i++)
            {
                climaLogs.Add(new Record()
                {
                    timestamp = today.AddHours(i),
                    measurements = new MeasurementData()
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
                    },
                });
            }

            UpdateDashboard();

            await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }

    private async Task GetCurrentConditionsViaMeadowCloud()
    {
        while (true)
        {
            var sensorReadings = await RestClient.GetSensorReadings();

            if (sensorReadings != null && sensorReadings.Count > 0)
            {
                climaLogs.Clear();

                foreach (var reading in sensorReadings.Take(10))
                {
                    climaLogs.Add(reading.record);
                }

                UpdateDashboard();
            }

            await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }

    private void UpdateDashboard()
    {
        int TIMEZONE_OFFSET = -7;

        LeftSeries.Clear();
        RightSeries.Clear();

        foreach (var climaLog in climaLogs.Take(10))
        {
            LeftSeries.Add(new Pnl(climaLog.timestamp.AddHours(TIMEZONE_OFFSET),
                LeftSeriesSelected switch
                {
                    MeasureType.Temperature => climaLog.measurements.Temperature,
                    MeasureType.Humidity => climaLog.measurements.Humidity,
                    MeasureType.Pressure => climaLog.measurements.Pressure,
                    MeasureType.Wind => climaLog.measurements.WindSpeed,
                    MeasureType.Rain => climaLog.measurements.Rain,
                    MeasureType.Illuminance => climaLog.measurements.Light,
                    MeasureType.PowerSupply => climaLog.measurements.SolarVoltage,
                    _ => throw new NotImplementedException()
                }));

            RightSeries.Add(new Pnl(climaLog.timestamp.AddHours(TIMEZONE_OFFSET),
                RightSeriesSelected switch
                {
                    MeasureType.Temperature => climaLog.measurements.Temperature,
                    MeasureType.Humidity => climaLog.measurements.Humidity,
                    MeasureType.Pressure => climaLog.measurements.Pressure,
                    MeasureType.Wind => climaLog.measurements.WindSpeed,
                    MeasureType.Rain => climaLog.measurements.Rain,
                    MeasureType.Illuminance => climaLog.measurements.Light,
                    MeasureType.PowerSupply => climaLog.measurements.BatteryVoltage,
                    _ => throw new NotImplementedException()
                }));
        }

        Temperature = $"{climaLogs[0].measurements.Temperature:N1}";
        Rain = $"{climaLogs[0].measurements.Rain:N0}";
        Light = $"{climaLogs[0].measurements.Light:N0}";
        SolarVoltage = $"{climaLogs[0].measurements.SolarVoltage:N1}";
        Humidity = $"{climaLogs[0].measurements.Humidity:N1}";
        WindSpeed = $"{climaLogs[0].measurements.WindSpeed:N0}";
        WindDirection = $"{climaLogs[0].measurements.WindDirection:N0}";
        Pressure = $"{climaLogs[0].measurements.Pressure:N1}";
        Co2Level = $"{climaLogs[0].measurements.Co2Level:N0}";
        BatteryVoltage = $"{climaLogs[0].measurements.BatteryVoltage:N1}";
    }
}