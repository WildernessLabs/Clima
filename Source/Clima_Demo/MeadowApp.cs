using Meadow;
using Meadow.Devices;
using Meadow.Devices.Esp32.MessagePayloads;
using Meadow.Hardware;
using System.Threading.Tasks;

namespace Clima_Demo;

public class ClimaApp : ClimaAppBase
{
    private MainController? mainController;

    public override Task Initialize()
    {
        Resolver.Log.Info($"Initialize...");

        mainController = new MainController();

        var reliabilityService = Resolver.Services.Get<IReliabilityService>();
        reliabilityService!.MeadowSystemError += OnMeadowSystemError;

        if (reliabilityService.LastBootWasFromCrash)
        {
            mainController.LogAppStartupAfterCrash(reliabilityService.GetCrashData());
            reliabilityService.ClearCrashData();
        }

        var wifi = Hardware.ComputeModule.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
        mainController.Initialize(Hardware, wifi);

        return Task.CompletedTask;
    }

    private void OnMeadowSystemError(MeadowSystemErrorInfo error, bool recommendReset, out bool forceReset)
    {
        if (error is Esp32SystemErrorInfo espError)
        {
            Resolver.Log.Warn($"The ESP32 has had an error ({espError.StatusCode}).");
        }
        else
        {
            Resolver.Log.Info($"We've had a system error: {error}");
        }

        if (recommendReset)
        {
            Resolver.Log.Warn($"Meadow is recommending a device reset");
        }

        forceReset = recommendReset;

        // override the reset recommendation
        //forceReset = false;
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