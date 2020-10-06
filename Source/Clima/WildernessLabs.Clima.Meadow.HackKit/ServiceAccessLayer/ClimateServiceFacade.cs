using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Clima.Contracts.Models;

namespace Clima.Meadow.HackKit.ServiceAccessLayer
{
    public static class ClimateServiceFacade
    {
        // TODO: change this IP for your localhost
        static string climateDataUri = "http://192.168.0.41:2792/ClimateData";


        static ClimateServiceFacade()
        {
        }

        /// <summary>
        /// Posts a temperature reading to the web API endpoint
        /// </summary>
        /// <param name="tempC"></param>
        /// <returns></returns>
        public static async Task PostTempReading(decimal tempC)
        {
            ClimateReading climateReading = new ClimateReading() { TempC = tempC };

            using (HttpClient client = new HttpClient()) {
                client.Timeout = new TimeSpan(0, 5, 0);

                string json = System.Text.Json.JsonSerializer.Serialize<ClimateReading>(climateReading);

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
                    System.Json.JsonArray climateReadings = System.Json.JsonArray.Parse(json) as System.Json.JsonArray;
                    foreach (var climateReading in climateReadings) {
                        Console.WriteLine($"ClimateReading; TempC:{climateReading["tempC"]}");
                    }

                } catch (TaskCanceledException) {
                    Console.WriteLine("Request time out.");
                } catch (Exception e) {
                    Console.WriteLine($"Request went sideways: {e.Message}");
                }
            }
        }
    }
}
