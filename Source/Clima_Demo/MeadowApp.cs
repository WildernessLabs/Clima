using Clima_Demo.Commands;
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

    public override Task Run()
    {
        var svc = Resolver.UpdateService;

        // Uncomment to clear any persisted update info. This allows installing 
        // the same update multiple times, such as you might do during development.
        // svc.ClearUpdates();

        svc.StateChanged += (sender, updateState) =>
        {
            Resolver.Log.Info($"UpdateState {updateState}");
            if (updateState == UpdateState.DownloadingFile)
            {
                mainController?.StopUpdating();
            }
        };

        svc.RetrieveProgress += (updateService, info, token) =>
        {
            short percentage = (short)((double)info.DownloadProgress / info.FileSize * 100);

            Resolver.Log.Info($"Downloading... {percentage}%");
        };

        svc.UpdateAvailable += async (updateService, info, token) =>
        {
            Resolver.Log.Info($"Update available!");
        };

        svc.UpdateRetrieved += async (updateService, info, token) =>
        {
            Resolver.Log.Info($"Update retrieved!");
        };

        RestartCommand.Initialise();

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
    }
}