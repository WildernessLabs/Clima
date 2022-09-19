using System.Text.Json.Serialization;

namespace CommonContracts.Models
{
    public class TemperatureModel
    {
        [JsonPropertyName("Temperature")]
        public string Temperature { get; set; }
        [JsonPropertyName("DateTime")]
        public string DateTime { get; set; }
    }
}