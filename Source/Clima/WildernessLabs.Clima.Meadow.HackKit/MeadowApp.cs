using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Clima.Contracts.Models;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Sensors.Temperature;
using Meadow.Gateway.WiFi;
using Meadow.Peripherals.Sensors.Atmospheric;

namespace Clima.Meadow.HackKit
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        AnalogTemperature analogTemperature;

        string climateDataUri = "http://192.168.0.41:2792/ClimateData";

        public MeadowApp()
        {
            //==== test serialization
            //SerializationTests.TestJsonDeserializeWeather();
            //SerializationTests.TestJsonSerializeWeather();
            //SerializationTests.TestSystemTextJsonDeserializeWeather();
            //SerializationTests.TestSystemTextJsonSerializeWeather();
            //SerializationTests.TestSimpleJsonDeserializeWeather();
            //SerializationTests.TestSystemJsonDeserializeWeather();

            //==== new up our peripherals
            Initialize();

            //==== connect to wifi
            Console.WriteLine($"Connecting to WiFi Network {Secrets.WIFI_NAME}");
            var result = Device.WiFiAdapter.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD);
            if (result.ConnectionStatus != ConnectionStatus.Success) {
                throw new Exception($"Cannot connect to network: {result.ConnectionStatus}");
            }
            Console.WriteLine($"Connected to {Secrets.WIFI_NAME}.");

            //==== grab the climate readings
            Console.WriteLine("Fetching climate readings.");
            FetchReadings().Wait();

            //==== take a reading
            //AtmosphericConditions conditions = ReadTemp().Wait();

            //==== post the reading
            Console.WriteLine("Posting the temp reading");
            PostTempReading(25.3m).Wait();

            //==== fetch the readings again
            Console.WriteLine("Fetching the readings agian.");
            FetchReadings().Wait();

        }

        void Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            // Analog Temp Sensor
            Console.WriteLine("Initializing analog temp sensor");
            analogTemperature = new AnalogTemperature(
                device: Device,
                analogPin: Device.Pins.A00,
                sensorType: AnalogTemperature.KnownSensorType.LM35
            );

            // WiFi adapter
            Console.WriteLine("Initializaing wifi adapter.");
            Device.InitWiFiAdapter().Wait();

            // display
        }

        protected async Task<AtmosphericConditions> ReadTemp()
        {
            var conditions = await analogTemperature.Read();
            Console.WriteLine($"Initial temp: { conditions.Temperature }");
            return conditions;
        }

        protected async Task PostTempReading(decimal tempC)
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
        protected async Task FetchReadings()
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