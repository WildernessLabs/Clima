using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MeadowProKit.Models;

namespace MeadowProKit.ServiceAccessLayer
{
    public static class ClimateService
    {
        // TODO: change this IP for your localhost
        static string climateDataUri = "http://192.168.86.53:2792/ClimateData";

        static ClimateService()
        {
        }

        /// <summary>
        /// Posts a temperature reading to the web API endpoint
        /// </summary>
        /// <param name="tempC"></param>
        /// <returns></returns>
        public static async Task PostTempReading(double tempC)
        {
            Climate climateReading = new Climate() { TemperatureValue = tempC };

            using (HttpClient client = new HttpClient()) {
                client.Timeout = new TimeSpan(0, 5, 0);

                string json = System.Text.Json.JsonSerializer.Serialize<Climate>(climateReading);

                HttpResponseMessage response = await client.PostAsync(
                    climateDataUri, new StringContent(
                        json, Encoding.UTF8, "application/json"));
                try {
                    response.EnsureSuccessStatusCode();
                } catch (TaskCanceledException) {
                    Console.WriteLine("Request time out.");
                } catch (Exception e) {
                    Console.WriteLine($"Request went sideways: {e.Message}");
                }
            }
        }

        /// <summary>
        /// Fetches the climate readings from the Web API Endpoint
        /// </summary>
        /// <returns></returns>
        public static async Task FetchReadings()
        {
            using (HttpClient client = new HttpClient()) {
                client.Timeout = new TimeSpan(0, 5, 0);

                HttpResponseMessage response = await client.GetAsync(climateDataUri);

                try {
                    response.EnsureSuccessStatusCode();

                    //System.Json[old skool]
                    string json = await response.Content.ReadAsStringAsync();

                    Console.WriteLine(json);

                    var stuff = System.Text.Json.JsonSerializer.Deserialize(json, typeof(Climate[]));

                    Console.WriteLine("deserialized to object");

                    var reading = stuff as Climate[];

                    Console.WriteLine($"Temp: {reading[0].Temperature.Value.Celsius}");
                } catch (TaskCanceledException) {
                    Console.WriteLine("Request time out.");
                } catch (Exception e) {
                    Console.WriteLine($"Request went sideways: {e.Message}");
                }
            }
        }
    }
}
