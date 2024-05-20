using Azure.DigitalTwins.Core;
using Azure.Identity;
using Clima_reTerminal.Models;
using System;
using System.Threading.Tasks;

namespace Clima_reTerminal.Client
{
    public static class DigitalTwinClient
    {
        public static async Task<MeasurementData> GetDigitalTwinData()
        {
            var clientSecretCredential = new ClientSecretCredential(Secrets.TENANT_ID, Secrets.CLIENT_ID, Secrets.CLIENT_SECRET);
            var digitalTwinsClient = new DigitalTwinsClient(new Uri(Secrets.DIGITAL_TWIN_ENDPOINT), clientSecretCredential);

            var twin = await digitalTwinsClient.GetDigitalTwinAsync<BasicDigitalTwin>(Secrets.DIGITAL_TWIN_ID);

            var model = new MeasurementData();

            if (twin != null)
            {
                Console.WriteLine($"Digital Twin ID: {twin.Value.Id}");
                Console.WriteLine($"Model ID: {twin.Value.Metadata.ModelId}");

                if (twin.Value.Contents.TryGetValue("Temperature", out var temperature))
                {
                    double.TryParse(temperature.ToString(), out var dtemperature);
                    model.Temperature = dtemperature;
                }
                if (twin.Value.Contents.TryGetValue("Humidity", out var humidity))
                {
                    double.TryParse(humidity.ToString(), out var dhumidity);
                    model.Humidity = dhumidity;
                }
                if (twin.Value.Contents.TryGetValue("SoilMoisture", out var soilMoisture))
                {
                    double.TryParse(soilMoisture.ToString(), out var dsoilMoisture);
                    model.Pressure = dsoilMoisture;
                }
            }
            else
            {
                Console.WriteLine($"Digital Twin with ID '{Secrets.DIGITAL_TWIN_ID}' not found.");
            }

            return model;
        }
    }
}