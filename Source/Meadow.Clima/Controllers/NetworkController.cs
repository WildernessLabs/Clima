using Meadow.Hardware;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow.Devices.Clima.Controllers;

public class NetworkController
{
    public event EventHandler<bool>? ConnectionStateChanged;
    public event EventHandler<TimeSpan>? NetworkDown;

    private readonly INetworkAdapter networkAdapter;
    private DateTimeOffset? lastDown;
    private Timer downEventTimer;

    public bool IsConnected => networkAdapter.IsConnected;
    public TimeSpan DownTime => lastDown == null ? TimeSpan.Zero : DateTime.UtcNow - lastDown.Value;
    public TimeSpan DownEventPeriod { get; } = TimeSpan.FromSeconds(30);

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