using System;
using Clima.Contracts.Bluetooth;
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
                windDirectionCharacteristic.SetValue($"{ windDirection};");
            }
        }

        protected Definition GetDefinition()
        {
            tempCharacteristic = new CharacteristicString(
                    "Temperature",
                    uuid: CharacteristicsConstants.TEMPERATURE,
                    permissions: CharacteristicPermission.Read,
                    properties: CharacteristicProperty.Read,
                    maxLength: 32
                );

            pressureCharacteristic = new CharacteristicString(
                    "Pressure",
                    uuid: CharacteristicsConstants.PRESSURE,
                    permissions: CharacteristicPermission.Read,
                    properties: CharacteristicProperty.Read,
                    maxLength: 32
                );
            
            humidityCharacteristic = new CharacteristicString(
                    "Humidity",
                    uuid: CharacteristicsConstants.HUMIDITY,
                    permissions: CharacteristicPermission.Read,
                    properties: CharacteristicProperty.Read,
                    maxLength: 32
                );

            rainFallCharacteristic = new CharacteristicString(
                    "RainFall",
                    uuid: CharacteristicsConstants.RAIN_FALL,
                    permissions: CharacteristicPermission.Read,
                    properties: CharacteristicProperty.Read,
                    maxLength: 32
                );

            windSpeedCharacteristic = new CharacteristicString(
                    "WindSpeed",
                    uuid: CharacteristicsConstants.WIND_SPEED,
                    permissions: CharacteristicPermission.Read,
                    properties: CharacteristicProperty.Read,
                    maxLength: 32
                );

            windDirectionCharacteristic = new CharacteristicString(
                    "WindDirection",
                    uuid: CharacteristicsConstants.WIND_DIRECTION,
                    permissions: CharacteristicPermission.Read,
                    properties: CharacteristicProperty.Read,
                    maxLength: 32
                );

            ICharacteristic[] characteristics = 
            {
                tempCharacteristic,
                pressureCharacteristic,
                humidityCharacteristic,
                rainFallCharacteristic,
                windSpeedCharacteristic,
                windDirectionCharacteristic
            };

            var service = new Service(
                name: "ServiceA",
                uuid: 253,
                characteristics
            );

            return new Definition("MeadowClimaPro", service);
        }
    }
}