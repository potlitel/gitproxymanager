namespace GitProxyManager.Services;

public static class ProxyStateService
{
    public static event Action<bool, string, int>? ProxyStateChanged;

    public static void NotifyStateChanged(bool isEnabled, string host, int port)
    {
        ProxyStateChanged?.Invoke(isEnabled, host, port);
    }
}
