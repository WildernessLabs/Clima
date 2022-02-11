using Meadow.Foundation;
using Meadow.Gateways.Bluetooth;
using MeadowClimaHackKit.Controller;

namespace MeadowClimaHackKit.Connectivity
{
    public class BluetoothServer
    {
        Definition bleTreeDefinition;
        CharacteristicString temperatureCharacteristic;

        readonly string TEMPERATURE = "24517ccc888e4ffc9da521884353b08d";

        public BluetoothServer() 
        { 
            
        }

        void Initialize()
        {
            bleTreeDefinition = GetDefinition();
            MeadowApp.Device.BluetoothAdapter.StartBluetoothServer(bleTreeDefinition);

            temperatureCharacteristic.ValueSet += TemperatureCharacteristicValueSet;
        }

        private void TemperatureCharacteristicValueSet(ICharacteristic c, object data)
        {
            LedController.Instance.SetColor(Color.Yellow);

            

            LedController.Instance.SetColor(Color.Green);
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

            return new Definition("Clima.HackKit", service);
        }
    }
}
