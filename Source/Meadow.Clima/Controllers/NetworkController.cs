using Meadow.Hardware;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow.Devices.Clima.Controllers;

/// <summary>
/// Controller for managing network connections and reporting network status.
/// </summary>
public class NetworkController
{
    /// <summary>
    /// Event triggered when the connection state changes.
    /// </summary>
    public event EventHandler<bool>? ConnectionStateChanged;

    /// <summary>
    /// Event triggered when the network is down for a specified period.
    /// </summary>
    public event EventHandler<TimeSpan>? NetworkDown;

    private readonly INetworkAdapter networkAdapter;
    private DateTimeOffset? lastDown;
    private readonly Timer downEventTimer;

    /// <summary>
    /// Gets a value indicating whether the network is connected.
    /// </summary>
    public bool IsConnected => networkAdapter.IsConnected;

    /// <summary>
    /// Gets the total time the network has been down.
    /// </summary>
    public TimeSpan DownTime => lastDown == null ? TimeSpan.Zero : DateTime.UtcNow - lastDown.Value;

    /// <summary>
    /// Gets the period for triggering network down events.
    /// </summary>
    public TimeSpan DownEventPeriod { get; } = TimeSpan.FromSeconds(30);
    
    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkController"/> class.
    /// </summary>
    /// <param name="networkAdapter">The network adapter</param>
    public NetworkController(INetworkAdapter networkAdapter)
    {
        if (networkAdapter is IWiFiNetworkAdapter wifi)
        {
            if (wifi.IsConnected)
            {
                _ = ReportWiFiScan(wifi);
            }

            // TODO: make this configurable
            wifi.SetAntenna(AntennaType.External);
        }
        this.networkAdapter = networkAdapter;

        networkAdapter.NetworkConnected += OnNetworkConnected;
        networkAdapter.NetworkDisconnected += OnNetworkDisconnected;

        downEventTimer = new Timer(DownEventTimerProc, null, -1, -1);
    }

    /// <summary>
    /// Connects to the cloud.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task<bool> ConnectToCloud()
    {
        if (networkAdapter is IWiFiNetworkAdapter wifi)
        {
            if (!wifi.IsConnected)
            {
                Resolver.Log.Info("Connecting to network...");
                await wifi.Connect("interwebs", "1234567890");
            }
        }

        Resolver.Log.Info($"Connecting to network {(networkAdapter.IsConnected ? "succeeded" : "FAILED")}");

        return networkAdapter.IsConnected;
    }

    /// <summary>
    /// Shuts down the network.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ShutdownNetwork()
    {
        if (networkAdapter is IWiFiNetworkAdapter wifi)
        {
            Resolver.Log.Info("Disconnecting network...");
            try
            {
                await wifi.Disconnect(true);
                Resolver.Log.Info("Network disconnected");
            }
            catch (Exception ex)
            {
                Resolver.Log.Info($"Network disconnect failed: {ex.Message}");
            }
        }
    }

    private void DownEventTimerProc(object _)
    {
        if (networkAdapter.IsConnected)
        {
            downEventTimer.Change(-1, -1);
            return;
        }

        NetworkDown?.Invoke(this, DownTime);
        downEventTimer.Change(DownEventPeriod, TimeSpan.FromMilliseconds(-1));
    }

    private void OnNetworkDisconnected(INetworkAdapter sender, NetworkDisconnectionEventArgs args)
    {
        lastDown = DateTimeOffset.UtcNow;
        downEventTimer.Change(DownEventPeriod, TimeSpan.FromMilliseconds(-1));
        ConnectionStateChanged?.Invoke(this, false);
    }

    private async Task ReportWiFiScan(IWiFiNetworkAdapter wifi)
    {
        var networks = await wifi.Scan();

        Resolver.Log.Info("WiFi Scan Results");
        if (networks.Count == 0)
        {
            Resolver.Log.Info("No networks found");
        }
        else
        {
            foreach (var network in networks)
            {
                if (string.IsNullOrEmpty(network.Ssid))
                {
                    Resolver.Log.Info($"[no ssid]: {network.SignalDbStrength}dB");
                }
                else
                {
                    Resolver.Log.Info($"{network.Ssid}: {network.SignalDbStrength}dB");
                }
            }
        }
    }

    private void OnNetworkConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
    {
        if (sender is IWiFiNetworkAdapter wifi)
        {
            _ = ReportWiFiScan(wifi);
        }

        lastDown = null;
        ConnectionStateChanged?.Invoke(this, true);
    }
}