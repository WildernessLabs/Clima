using System;
using Meadow.Units;
using SQLite;
using MU = Meadow.Units;

namespace Clima_SQLite_Demo.Database
{
    [Table("ClimateReadings")]
    public class ClimateReading
    {
        [PrimaryKey, AutoIncrement]
        public int? ID { get; set; }

        public double? TemperatureValue
        {
            get => Temperature?.Celsius;
            set => Temperature = new Temperature(value.Value, MU.Temperature.UnitType.Celsius);
        }

        public double? PressureValue
        {
            get => Pressure?.Bar;
            set => Pressure = new Pressure(value.Value, MU.Pressure.UnitType.Bar);
        }

        public double? HumidityValue
        {
            get => Humidity?.Percent;
            set => Humidity = new RelativeHumidity(value.Value, MU.RelativeHumidity.UnitType.Percent);
        }

        public double? RainFallValue
        {
            get => RainFall?.Millimeters;
            set => RainFall = new Length(value.Value, MU.Length.UnitType.Millimeters);
        }

        public Azimuth16PointCardinalNames? WindDirectionValue
        {
            get => WindDirection;
            set => WindDirection = value;
        }

        public double? WindSpeedValue
        {
            get => WindSpeed?.KilometersPerHour;
            set => WindSpeed = new Speed(value.Value, MU.Speed.UnitType.KilometersPerHour);
        }

        [Indexed]
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Whether or not this particular reading has been uploaded to the cloud.
        /// </summary>
        public bool Synchronized { get; set; }

        [Ignore]
        public Temperature? Temperature { get; set; }
        [Ignore]
        public Pressure? Pressure { get; set; }
        [Ignore]
        public RelativeHumidity? Humidity { get; set; }
        [Ignore]
        public Length? RainFall { get; set; }
        [Ignore]
        public Azimuth16PointCardinalNames? WindDirection { get; set; }
        [Ignore]
        public Speed? WindSpeed { get; set; }
    }
}