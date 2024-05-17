using Meadow.Hardware;

namespace Meadow.Devices;

public partial class ClimaDevice
{
    public class ClimaConnectors : ConnectorCollection
    {
        internal ClimaConnectors(IClimaHardware hardware)
        {
            if (hardware is ClimaHardwareV4 v4)
            {
                var qwiic = v4.CreateQwiicConnector();
                if (qwiic != null)
                {
                    base.Add(qwiic);
                }
            }
        }
    }

}
