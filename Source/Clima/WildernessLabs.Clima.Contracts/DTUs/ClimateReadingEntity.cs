using System;

namespace Clima.Contracts.DTUs
{
    public class ClimateReadingEntity
    {
        public long? id { get; set; }
        public DateTime? date { get; set; }
        public decimal? tempC { get; set; }
        public decimal? barometricPressureMillibarHg { get; set; }
        public decimal? relativeHumdity { get; set; }
    }
}