using System;
namespace Clima.Contracts.Models
{
    public class ClimateReading
    {
        public ClimateReading()
        {
        }

        public long? ID { get; set; }
        public DateTime TimeOfReading { get; set; }
        public decimal? TempC { get; set; }
        public decimal? BarometricPressureMillibarHg { get; set; }
        public decimal? RelativeHumdity { get; set; }
    }
}
