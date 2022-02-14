using System;
using SQLite;
using MU = Meadow.Units;

namespace MeadowClimaProKit.Database
{
    [Table("ClimateReadings")]
    public class ClimateReading
    {
        [PrimaryKey, AutoIncrement]
        public int? ID { get; set; }

        public double? TemperatureValue
        {
            get => Temperature?.Celsius;
            set => Temperature = new MU.Temperature(value.Value, MU.Temperature.UnitType.Celsius);
        }

        public double? PressureValue
        {
            get => Pressure?.Bar;
            set => Pressure = new MU.Pressure(value.Value, MU.Pressure.UnitType.Bar);
        }

        public double? HumidityValue
        {
            get => Humidity?.Percent;
            set => Humidity = new MU.RelativeHumidity(value.Value, MU.RelativeHumidity.UnitType.Percent);
        }

        public double? RainFallValue
        {
            get => RainFall?.Millimeters;
            set => RainFall = new MU.Length(value.Value, MU.Length.UnitType.Millimeters);
        }

        public double? WindDirectionValue
        {
            get => WindDirection?.DecimalDegrees;
            set => WindDirection = new MU.Azimuth(value.Value);
        }

        public double? WindSpeedValue
        {
            get => WindSpeed?.KilometersPerHour;
            set => WindSpeed = new MU.Speed(value.Value, MU.Speed.UnitType.KilometersPerHour);
        }

        [Indexed]
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Whether or not this particular reading has been uploaded to the cloud.
        /// </summary>
        public bool Synchronized { get; set; }

        [Ignore]
        public MU.Temperature? Temperature { get; set; }
        [Ignore]
        public MU.Pressure? Pressure { get; set; }
        [Ignore]
        public MU.RelativeHumidity? Humidity { get; set; }
        [Ignore]
        public MU.Length? RainFall { get; set; }
        [Ignore]
        public MU.Azimuth? WindDirection { get; set; }
        [Ignore]
        public MU.Speed? WindSpeed { get; set; }
    }
}
