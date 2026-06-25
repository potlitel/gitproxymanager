namespace GitProxyManager.Services;

public static class ProxyStateService
{
    public static event Action<bool, bool, string, int>? ProxyStateChanged;

    public static void NotifyStateChanged(bool isSystemEnabled, bool isGitEnabled, string host, int port)
    {
        ProxyStateChanged?.Invoke(isSystemEnabled, isGitEnabled, host, port);
    }
}