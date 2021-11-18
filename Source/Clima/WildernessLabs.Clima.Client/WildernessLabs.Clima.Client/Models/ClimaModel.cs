using Newtonsoft.Json;

namespace WildernessLabs.Clima.App.Models
{
    public class ClimaModel
    {
        [JsonProperty("DateTime")]
        public string DateTime { get; set; }

        [JsonProperty("Temperature")]
        public decimal? Temperature { get; set; }
    }
}