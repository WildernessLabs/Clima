using System;
using System.Text.Json.Serialization;

namespace CommonContracts.Models
{
    public class WeatherModel
    {
        [JsonPropertyName("id")]
        public long? ID { get; set; }

        [JsonPropertyName("date")]
        public DateTime? TimeOfReading { get; set; }

        [JsonPropertyName("tempC")]
        public double? TempC { get; set; }

        [JsonPropertyName("barometricPressureMillibarHg")]
        public double? BarometricPressureMillibarHg { get; set; }

        [JsonPropertyName("relativeHumdity")]
        public double? RelativeHumdity { get; set; }
    }
}