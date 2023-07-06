using System;
using System.Text.Json.Serialization;

namespace CommonContracts.Models
{
    public class ClimateModel
    {
        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("temperature")]
        public string Temperature { get; set; }

        [JsonPropertyName("pressure")]
        public string Pressure { get; set; }

        [JsonPropertyName("humdity")]
        public string Humidity { get; set; }

        [JsonPropertyName("rain")]
        public string Rain { get; set; }

        [JsonPropertyName("windspeed")]
        public string WindSpeed { get; set; }

        [JsonPropertyName("winddirection")]
        public string WindDirection { get; set; }
    }
}