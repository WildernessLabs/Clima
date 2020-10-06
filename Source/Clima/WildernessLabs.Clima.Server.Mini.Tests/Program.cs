using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Clima.Contracts.Models;

namespace WildernessLabs.Clima.Server.Mini.Tests
{
    class Program
    {
        static string climateDataUri = "http://192.168.0.41:2792/ClimateData";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //GetWebPageViaHttpClient("http://192.168.0.41:2792/ClimateData").Wait();

            FetchReadings().Wait();

            PostTempReading(26.4m).Wait();

            FetchReadings().Wait();
        }


        static async Task GetWebPageViaHttpClient(string uri)
        {
            Console.WriteLine($"Requesting {uri}");

            using (HttpClient client = new HttpClient()) {
                client.Timeout = new TimeSpan(0, 5, 0);

                HttpResponseMessage response = await client.GetAsync(uri);

                try {
                    response.EnsureSuccessStatusCode();

                    // this works in a console app _if_ i add the Microsoft.AspNet.WebApi.Client
                    // nuget, which brings in waaaaay too much stuff for Meadow.
                    //var climateReadings = await response.Content.ReadAsAsync<ClimateReading[]>();

                    // Json.Net (NewtonSoft)
                    //string jsonStr = await response.Content.ReadAsStringAsync();
                    //ClimateReading[] climateReadings = Newtonsoft.Json.JsonConvert.DeserializeObject<ClimateReading[]>(jsonStr);

                    // System.Text.Json:
                    //string json = await response.Content.ReadAsStringAsync();
                    //ClimateReading[] climateReadings = System.Text.Json.JsonSerializer.Deserialize<ClimateReading[]>(json);

                    // System.Json [old skool]
                    string json = await response.Content.ReadAsStringAsync();
                    System.Json.JsonArray climateReadings = System.Json.JsonArray.Parse(json) as System.Json.JsonArray;
                    foreach (var climateReading in climateReadings) {
                        Console.WriteLine($"ClimateReading; TempC:{climateReading["tempC"]}");
                    }

                    //string responseBody = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine(responseBody);
                } catch (TaskCanceledException) {
                    Console.WriteLine("Request time out.");
                } catch (Exception e) {
                    Console.WriteLine($"Request went sideways: {e.Message}");
                }
            }
        }


        static async Task FetchReadings()
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

        static async Task PostTempReading(decimal tempC)
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

    }
}
