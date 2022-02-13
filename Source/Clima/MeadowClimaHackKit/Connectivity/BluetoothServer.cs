using Meadow.Gateways.Bluetooth;
using MeadowClimaHackKit.Controller;
using System;

namespace MeadowClimaHackKit.Connectivity
{
    public class BluetoothServer
    {
        private static readonly Lazy<BluetoothServer> instance =
            new Lazy<BluetoothServer>(() => new BluetoothServer());
        public static BluetoothServer Instance => instance.Value;

        Definition bleTreeDefinition;
        CharacteristicString temperatureCharacteristic;

        readonly string TEMPERATURE = "24517ccc888e4ffc9da521884353b08d";

        private BluetoothServer() { }        

        public void Initialize()
        {
            bleTreeDefinition = GetDefinition();
            TemperatureController.Instance.TemperatureUpdated += TemperatureUpdated;
            MeadowApp.Device.BluetoothAdapter.StartBluetoothServer(bleTreeDefinition);
        }

        private void TemperatureUpdated(object sender, Meadow.Units.Temperature e)
        {
            temperatureCharacteristic.SetValue($"{ e.Celsius:N2}°C;");
        }

        Definition GetDefinition()
        {
            temperatureCharacteristic = new CharacteristicString(
                name: "Temperature",
                uuid: TEMPERATURE,
                maxLength: 20,
                permissions: CharacteristicPermission.Read,
                properties: CharacteristicProperty.Read);

            var service = new Service(
                name: "ServiceA",
                uuid: 253,
                temperatureCharacteristic
            );

            return new Definition("MeadowClimaHackKit", service);
        }
    }
}