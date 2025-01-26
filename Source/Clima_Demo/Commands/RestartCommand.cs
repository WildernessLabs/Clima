using Meadow;
using Meadow.Cloud;
using System.Threading;

namespace Clima_Demo.Commands;

/// <summary>
/// Restart Clima
/// </summary>
public class RestartCommand : IMeadowCommand
{
    /// <summary>
    /// Delay to restart
    /// </summary>
    public int Delay { get; set; }

    /// <summary>
    /// Initialise the command and register handler.
    /// </summary>
    public static void Initialise()
    {
        Resolver.CommandService.Subscribe<RestartCommand>(command =>
        {
            Resolver.Log.Info($"RestartCommand: Meadow will restart in {command.Delay} seconds.");
            Thread.Sleep(command.Delay * 1000);
            Resolver.Device.PlatformOS.Reset();
        });

    }
}