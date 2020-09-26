using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Clima.Contracts.Models;

namespace WildernessLabs.Clima.Server.Mini.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            GetWebPageViaHttpClient("http://192.168.0.41:2792/ClimateData").Wait();
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

    }
}
