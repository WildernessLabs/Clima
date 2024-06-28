using Meadow.Devices.Clima.Hardware;

namespace Meadow.Devices;

public abstract class ClimaAppBase : App<F7CoreComputeV2, ClimaHardwareProvider, IClimaHardware>
{
}
