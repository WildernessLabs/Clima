using Meadow;
using Meadow.Devices;
using Meadow.Devices.Esp32.MessagePayloads;
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

        Device.PlatformOS.MeadowSystemError += OnMeadowSystemError;
        return Task.CompletedTask;
    }

    private void OnMeadowSystemError(object sender, MeadowSystemErrorInfo e)
    {
        Resolver.Log.Error($"App has detected a system error: {e.Message}");
        if (e is Esp32SystemErrorInfo esp)
        {
            Resolver.Log.Error($"ESP function: {esp.Function}");
            Resolver.Log.Error($"ESP status code: {esp.StatusCode}");
        }
        if (e.Exception != null)
        {
            Resolver.Log.Error($"Exception: {e.Exception.Message}");
            Resolver.Log.Error($"ErrorNumber: {e.ErrorNumber}");
            Resolver.Log.Error($"HResult: {e.Exception.HResult}");

            if (e.Exception.InnerException != null)
            {
                Resolver.Log.Error($"InnerException: {e.Exception.InnerException.Message}");
                Resolver.Log.Error($"HResult: {e.Exception.InnerException.HResult}");
            }
        }
    }
}