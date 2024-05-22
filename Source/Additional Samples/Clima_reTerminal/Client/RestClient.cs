using Clima_reTerminal.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Clima_reTerminal.Client
{
    public static class RestClient
    {
        static RestClient() { }

        public static async Task<List<QueryResponse>> GetSensorReadings()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"apikey {Secrets.API_KEY}");
                    client.Timeout = new TimeSpan(0, 5, 0);

                    var response = await client.GetAsync($"" +
                        $"{Secrets.MEADOW_CLOUD_URL}/api/orgs/{Secrets.ORGANIZATION_ID}/search/source:event " +
                        $"deviceId:{Secrets.DEVICE_ID} eventId:100 size:20 sortby:timestamp sortorder:desc");

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Response Content: " + jsonString);

                        var root = JsonSerializer.Deserialize<Root>(jsonString);

                        return root?.data?.queryResponses;
                    }
                    else
                    {
                        Console.WriteLine("Error: " + response.StatusCode);
                        return new List<QueryResponse>();
                    }
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Request timed out.");
                    return new List<QueryResponse>();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Request went sideways: {e.Message}");
                    return new List<QueryResponse>();
                }
            }
        }

        public static async Task<bool> SendCommand(CultivarCommands command, bool relayState)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"apikey {Secrets.API_KEY}");
                    client.Timeout = new TimeSpan(0, 5, 0);

                    string jsonString = $"" +
                        $"{{" +
                            $"\"deviceIds\": [" +
                                $"\"{Secrets.DEVICE_ID}\"]," +
                            $"\"commandName\": \"{command}\"," +
                            $"\"args\": {{     " +
                                $"\"relaystate\": {relayState}" +
                            $"}}," +
                            $"\"qos\": 0" +
                        $"}}";

                    var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync($"{Secrets.MEADOW_CLOUD_URL}/api/devices/commands", content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Response Content: " + responseContent);
                    }
                    else
                    {
                        Console.WriteLine("Error: " + response.StatusCode);
                    }

                    return true;
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Request timed out.");
                    return false;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Request went sideways: {e.Message}");
                    return false;
                }
            }
        }
    }
}