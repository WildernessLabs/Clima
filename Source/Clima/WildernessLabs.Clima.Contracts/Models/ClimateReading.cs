using System;
using System.Text.Json.Serialization;

namespace Clima.Contracts.Models
{
    public class ClimateReading
    {
        [JsonPropertyName("id")]
        public long? ID { get; set; }

        [JsonPropertyName("date")]
        public DateTime? TimeOfReading { get; set; }

        [JsonPropertyName("tempC")]
        public decimal? TempC { get; set; }

        [JsonPropertyName("barometricPressureMillibarHg")]
        public decimal? BarometricPressureMillibarHg { get; set; }

        [JsonPropertyName("relativeHumdity")]
        public decimal? RelativeHumdity { get; set; }
    }
}