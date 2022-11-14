using System;
using Meadow.Units;
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
            set => Temperature = new MU.Temperature((int)value.Value, MU.Temperature.UnitType.Celsius);
        }

        public double? PressureValue
        {
            get => Pressure?.Bar;
            set => Pressure = new MU.Pressure((int)value.Value, MU.Pressure.UnitType.Bar);
        }

        public double? HumidityValue
        {
            get => Humidity?.Percent;
            set => Humidity = new MU.RelativeHumidity((int)value.Value, MU.RelativeHumidity.UnitType.Percent);
        }

        public double? RainFallValue
        {
            get => RainFall?.Millimeters;
            set => RainFall = new MU.Length((int)value.Value, MU.Length.UnitType.Millimeters);
        }

        public Azimuth16PointCardinalNames? WindDirectionValue
        {
            get => WindDirection;
            set => WindDirection = value;
        }

        public double? WindSpeedValue
        {
            get => WindSpeed?.KilometersPerHour;
            set => WindSpeed = new MU.Speed((int)value.Value, MU.Speed.UnitType.KilometersPerHour);
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
        public MU.Azimuth16PointCardinalNames? WindDirection { get; set; }
        [Ignore]
        public MU.Speed? WindSpeed { get; set; }
    }
}
