using Meadow.Hardware;
using System;
using System.Threading;

namespace Meadow.Devices;

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
        this.networkAdapter = networkAdapter;

        networkAdapter.NetworkConnected += OnNetworkConnected;
        networkAdapter.NetworkDisconnected += OnNetworkDisconnected;

        downEventTimer = new Timer(DownEventTimerProc, null, -1, -1);
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

    private void OnNetworkConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
    {
        lastDown = null;
        ConnectionStateChanged?.Invoke(this, true);
    }
}
