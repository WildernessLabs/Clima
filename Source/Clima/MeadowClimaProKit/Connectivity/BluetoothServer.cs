using System;
using Meadow.Gateways.Bluetooth;

namespace MeadowClimaProKit.Connectivity
{
    public class BluetoothServer
    {
        private static readonly Lazy<BluetoothServer> instance =
            new Lazy<BluetoothServer>(() => new BluetoothServer());
        public static BluetoothServer Instance => instance.Value;

        Definition bleTreeDefinition;
        CharacteristicString tempCharacteristic;
        CharacteristicString pressureCharacteristic;
        CharacteristicString humidityCharacteristic;
        CharacteristicString rainFallCharacteristic;
        CharacteristicString windSpeedCharacteristic;
        CharacteristicString windDirectionCharacteristic;

        private BluetoothServer() { }

        public void Initialize()
        {
            bleTreeDefinition = GetDefinition();
            ClimateMonitorAgent.Instance.ClimateConditionsUpdated += ClimateConditionsUpdated;
            MeadowApp.Device.BluetoothAdapter.StartBluetoothServer(bleTreeDefinition);
        }

        private void ClimateConditionsUpdated(object sender, Models.ClimateConditions climateConditions)
        {
            Console.WriteLine("New climate data, setting BLE characteristics.");
            if(climateConditions.New?.Temperature is { } temperature)
            {
                tempCharacteristic.SetValue($"{ temperature.Fahrenheit:N2}°F;");
            }
            if (climateConditions.New?.Pressure is { } pressure)
            {
                pressureCharacteristic.SetValue($"{ pressure.Pascal:N2}hPa;");
            }
            if (climateConditions.New?.Humidity is { } humidity)
            {
                humidityCharacteristic.SetValue($"{ humidity:N2}%;");
            }
            if (climateConditions.New?.RainFall is { } rainFall)
            {
                rainFallCharacteristic.SetValue($"{ rainFall:N2}mm;");
            }
            if (climateConditions.New?.WindSpeed is { } windSpeed)
            {
                windSpeedCharacteristic.SetValue($"{ windSpeed.KilometersPerHour:N2}Kmph;");
            }
            if (climateConditions.New?.WindDirection is { } windDirection)
            {
                windDirectionCharacteristic.SetValue($"{ windDirection.Compass16PointCardinalName};");
            }
        }

        protected Definition GetDefinition()
        {
            //==== Create our charactistics            
            tempCharacteristic = new CharacteristicString(
                    "Temperature",
                    uuid: "e78f7b5e-842b-4b99-94e3-7401bf72b870",
                    permissions: CharacteristicPermission.Read,
                    properties: CharacteristicProperty.Read,
                    maxLength: 32
                );

            pressureCharacteristic = new CharacteristicString(
                    "Pressure",
                    uuid: "2d45f026-d8ea-4d47-813a-13e8f788d328",
                    permissions: CharacteristicPermission.Read,
                    properties: CharacteristicProperty.Read,
                    maxLength: 32
                );
            
            humidityCharacteristic = new CharacteristicString(
                    "Humidity",
                    uuid: "143a3841-e244-4520-a456-214e048a030f",
                    permissions: CharacteristicPermission.Read,
                    properties: CharacteristicProperty.Read,
                    maxLength: 32
                );

            rainFallCharacteristic = new CharacteristicString(
                    "RainFall",
                    uuid: "017e99d6-8a61-11eb-8dcd-0242ac1300aa",
                    permissions: CharacteristicPermission.Read,
                    properties: CharacteristicProperty.Read,
                    maxLength: 32
                );

            windSpeedCharacteristic = new CharacteristicString(
                    "WindSpeed",
                    uuid: "5a0bb016-69ab-4a49-a2f2-de5b292458f3",
                    permissions: CharacteristicPermission.Read,
                    properties: CharacteristicProperty.Read,
                    maxLength: 32
                );

            windDirectionCharacteristic = new CharacteristicString(
                    "WindDirection",
                    uuid: "b54aa605-c1ae-47cd-b4e1-0c5ff6af735c",
                    permissions: CharacteristicPermission.Read,
                    properties: CharacteristicProperty.Read,
                    maxLength: 32
                );

            ICharacteristic[] characteristics = {
                    tempCharacteristic,
                    pressureCharacteristic,
                    humidityCharacteristic,
                    rainFallCharacteristic,
                    windSpeedCharacteristic,
                    windDirectionCharacteristic
            };

            //==== BLE Tree Definition
            var definition = new Definition(
                "Meadow Clima",
                new Service(
                    "Weather_Conditions",
                    896,
                    characteristics
                ));
            return definition;
        }
    }
}