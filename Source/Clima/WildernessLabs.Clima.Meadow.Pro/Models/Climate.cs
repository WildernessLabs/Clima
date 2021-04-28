using System;
using Meadow.Units;

namespace Clima.Meadow.Pro.Models
{
    public class ClimateConditions
    {
        public Climate? New { get; set; }
        public Climate? Old { get; set; }
    }

    public class Climate
    {
        public Temperature? Temperature { get; set; }
        public Pressure? Pressure { get; set; }
        public RelativeHumidity? Humidity { get; set; }
        public Azimuth? WindDirection { get; set; }
        public Speed? Windspeed { get; set; }
    }
}
