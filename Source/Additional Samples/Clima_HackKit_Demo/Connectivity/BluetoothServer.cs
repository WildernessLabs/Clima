using Clima.Contracts.Bluetooth;
using Meadow.Gateways.Bluetooth;
using Clima_HackKit_Demo.Controller;
using System;
using Meadow;
using Meadow.Foundation;

namespace Clima_HackKit_Demo.Connectivity
{
    public class BluetoothServer
    {
        private static readonly Lazy<BluetoothServer> instance =
            new Lazy<BluetoothServer>(() => new BluetoothServer());
        public static BluetoothServer Instance => instance.Value;

        Definition bleTreeDefinition;
        CharacteristicString temperatureCharacteristic;

        private BluetoothServer() { }

        public void Initialize()
        {
            bleTreeDefinition = GetDefinition();
            TemperatureController.Instance.TemperatureUpdated += TemperatureUpdated;
            MeadowApp.Device.BluetoothAdapter.StartBluetoothServer(bleTreeDefinition);

            LedController.Instance.SetColor(Color.Green);
        }

        private void TemperatureUpdated(object sender, Meadow.Units.Temperature e)
        {
            temperatureCharacteristic.SetValue($"{ e.Celsius:N2}°C;");
        }

        Definition GetDefinition()
        {
            temperatureCharacteristic = new CharacteristicString(
                name: "Temperature",
                uuid: CharacteristicsConstants.TEMPERATURE,
                maxLength: 20,
                permissions: CharacteristicPermission.Read,
                properties: CharacteristicProperty.Read);

            var service = new Service(
                name: "ServiceA",
                uuid: 253,
                temperatureCharacteristic
            );

            return new Definition("Clima_HackKit_Demo", service);
        }
    }
}