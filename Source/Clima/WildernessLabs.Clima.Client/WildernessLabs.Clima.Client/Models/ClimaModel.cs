using Newtonsoft.Json;

namespace WildernessLabs.Clima.App.Models
{
    public class ClimaModel
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("temperature")]
        public decimal? Temperature { get; set; }
    }
}