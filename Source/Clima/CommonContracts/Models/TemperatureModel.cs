using System;
using System.Text.Json.Serialization;

namespace CommonContracts.Models
{
    public class TemperatureModel
    {
        [JsonPropertyName("temperature")]
        public string Temperature { get; set; }

        [JsonPropertyName("date")]
        public DateTime? DateTime { get; set; }
    }
}