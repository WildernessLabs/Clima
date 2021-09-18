using System;
using Meadow.Units;
using SQLite;

namespace Clima.Meadow.Pro.Models
{
    public class ClimateConditions
    {
        public Climate? New { get; set; }
        public Climate? Old { get; set; }

        public ClimateConditions() { }
        public ClimateConditions(Climate newClimate, Climate oldClimate) {
            this.New = newClimate;
            this.Old = oldClimate;
        }
    }

    [Table("ClimateReadings")]
    public class Climate
    {
        [PrimaryKey, AutoIncrement]
        public int? ID { get; set; }
        public Temperature? Temperature { get; set; }
        public Pressure? Pressure { get; set; }
        public RelativeHumidity? Humidity { get; set; }
        public Azimuth? WindDirection { get; set; }
        public Speed? Windspeed { get; set; }
        [Indexed]
        public DateTime DateTime { get; set; }
        /// <summary>
        /// Whether or not this particular reading has been uploaded to the cloud.
        /// </summary>
        public bool Synchronized { get; set; }
    }
}
