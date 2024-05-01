using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clima_Demo;

public class MeadowApp : App<F7CoreComputeV2>
{
    private MainController mainController;

    public MeadowApp()
    {
        mainController = new MainController();
    }

    public override void OnBootFromCrash(IEnumerable<string> crashReports)
    {
        mainController.LogAppStartupAfterCrash(crashReports);
    }

    public override Task Initialize()
    {
        var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
        mainController.Initialize(Clima.Create(), wifi);

        return Task.CompletedTask;
    }
}