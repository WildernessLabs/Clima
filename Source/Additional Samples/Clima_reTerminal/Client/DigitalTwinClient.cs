using Azure.DigitalTwins.Core;
using Azure.Identity;
using Clima_reTerminal.Models;
using System;
using System.Threading.Tasks;

namespace Clima_reTerminal.Client
{
    public static class DigitalTwinClient
    {
        public static async Task<GreenhouseModel> GetDigitalTwinData()
        {
            var clientSecretCredential = new ClientSecretCredential(Secrets.TENANT_ID, Secrets.CLIENT_ID, Secrets.CLIENT_SECRET);
            var digitalTwinsClient = new DigitalTwinsClient(new Uri(Secrets.DIGITAL_TWIN_ENDPOINT), clientSecretCredential);

            var twin = await digitalTwinsClient.GetDigitalTwinAsync<BasicDigitalTwin>(Secrets.DIGITAL_TWIN_ID);

            var model = new GreenhouseModel();

            if (twin != null)
            {
                Console.WriteLine($"Digital Twin ID: {twin.Value.Id}");
                Console.WriteLine($"Model ID: {twin.Value.Metadata.ModelId}");

                if (twin.Value.Contents.TryGetValue("Temperature", out var temperature))
                {
                    double.TryParse(temperature.ToString(), out var dtemperature);
                    model.TemperatureCelsius = dtemperature;
                }
                if (twin.Value.Contents.TryGetValue("Humidity", out var humidity))
                {
                    double.TryParse(humidity.ToString(), out var dhumidity);
                    model.HumidityPercentage = dhumidity;
                }
                if (twin.Value.Contents.TryGetValue("SoilMoisture", out var soilMoisture))
                {
                    double.TryParse(soilMoisture.ToString(), out var dsoilMoisture);
                    model.SoilMoisturePercentage = dsoilMoisture;
                }
                if (twin.Value.Contents.TryGetValue("IsLightOn", out var isLightOn))
                {
                    bool.TryParse(isLightOn.ToString(), out var bisLightOn);
                    model.IsLightOn = bisLightOn;
                }
                if (twin.Value.Contents.TryGetValue("IsHeaterOn", out var isHeaterOn))
                {
                    bool.TryParse(isHeaterOn.ToString(), out var bisHeaterOn);
                    model.IsHeaterOn = bisHeaterOn;
                }
                if (twin.Value.Contents.TryGetValue("IsSprinklerOn", out var isSprinklerOn))
                {
                    bool.TryParse(isSprinklerOn.ToString(), out var bisSprinklerOn);
                    model.IsSprinklerOn = bisSprinklerOn;
                }
                if (twin.Value.Contents.TryGetValue("IsVentilationOn", out var isVentilationOn))
                {
                    bool.TryParse(isVentilationOn.ToString(), out var bisVentilationOn);
                    model.IsVentilationOn = bisVentilationOn;
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