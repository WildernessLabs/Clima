using System;

namespace Clima.Contracts.DTUs
{
    public class ClimateReadingEntity
    {
        public long? id { get; set; }
        public DateTime? date { get; set; }
        public double? tempC { get; set; }
        public double? barometricPressureMillibarHg { get; set; }
        public double? relativeHumdity { get; set; }
    }
}