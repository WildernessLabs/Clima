using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WildernessLabs.Clima.App
{
    public static class NetworkManager
    {

        static NetworkManager() { }

        /// <summary>
        /// Fetches the climate readings from the Web API Endpoint
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> GetAsync(string ipAddress)
        {
            using (HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri($"http://{ipAddress}:2792/"),
                Timeout = TimeSpan.FromMinutes(5)
            })
            {
                try
                {
                    var response = await client.GetAsync("ClimateData", HttpCompletionOption.ResponseContentRead);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();

                        Console.WriteLine(json);
                    }
                    else
                    {
                        throw new InvalidOperationException("Could not connect to device");
                    }

                    //var values = System.Text.Json.JsonSerializer.Deserialize(json, typeof(List<ClimateReading>));

                    return response;
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Request time out.");
                    return null;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Request went sideways: {e.Message}");
                    return null;
                }
            }
        }
    }
}