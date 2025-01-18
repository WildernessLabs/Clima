using Meadow.Devices.Clima.Hardware;

namespace Meadow.Devices;

/// <summary>
/// Base class for Clima applications.
/// </summary>
public abstract class ClimaAppBase : App<F7CoreComputeV2, ClimaHardwareProvider, IClimaHardware>
{ }