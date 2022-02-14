using System.Text.Json.Serialization;

namespace CommonContracts.Models
{
    public class TemperatureModel
    {
        [JsonPropertyName("temperature")]
        public string Temperature { get; set; }
        [JsonPropertyName("dateTime")]
        public string DateTime { get; set; }
    }
}