﻿using Clima.Contracts.DTUs;
using Clima.Meadow.HackKit.Utils;
using Json.Net;
using Meadow.Foundation;
using Meadow.Units;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Clima.Meadow.HackKit.ServiceAccessLayer
{
    public static class ClimateService
    {
        // TODO: change this IP for your localhost 
        static string climateDataUri = "http://192.168.1.74:2792/ClimateData";

        static ClimateService() { }

        /// <summary>
        /// Posts a temperature reading to the web API endpoint
        /// </summary>
        /// <param name="tempC"></param>
        /// <returns></returns>
        public static async Task PostTempReading(Temperature temperature)
        {
            LedIndicator.StartPulse(Color.Magenta);

            var climateReading = new ClimateReadingEntity() { tempC = temperature.Celsius };

            using (HttpClient client = new HttpClient()) 
            {
                try
                {
                    client.Timeout = new TimeSpan(0, 5, 0);

                    string json = JsonNet.Serialize(climateReading);

                    HttpResponseMessage response = await client.PostAsync(
                        climateDataUri, new StringContent(
                            json, Encoding.UTF8, "application/json"));
                
                    response.EnsureSuccessStatusCode();
                } 
                catch (TaskCanceledException) 
                {
                    LedIndicator.StartBlink(Color.OrangeRed);
                    Console.WriteLine("Request time out.");
                } 
                catch (Exception e) 
                {
                    LedIndicator.StartBlink(Color.OrangeRed);
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
            LedIndicator.StartPulse(Color.Magenta);

            using (HttpClient client = new HttpClient()) 
            {
                try
                {
                    client.Timeout = new TimeSpan(0, 5, 0);

                    var response = await client.GetAsync(climateDataUri);
                
                    response.EnsureSuccessStatusCode();

                    string json = await response.Content.ReadAsStringAsync();

                    Console.WriteLine(json);

                    var readingStuffs = JsonNet.Deserialize<ClimateReadingEntity[]>(json);                   

                    Console.WriteLine($"Temp: {readingStuffs[0].tempC}");
                } 
                catch (TaskCanceledException) 
                {
                    LedIndicator.StartBlink(Color.OrangeRed);
                    Console.WriteLine("Request time out.");
                } 
                catch (Exception e) 
                {
                    LedIndicator.StartBlink(Color.OrangeRed);
                    Console.WriteLine($"Request went sideways: {e.Message}");
                }
            }
        }
    }
}