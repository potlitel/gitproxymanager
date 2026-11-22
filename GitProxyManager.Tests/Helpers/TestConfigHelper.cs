using GitProxyManager.Models;

namespace GitProxyManager.Tests.Helpers;

public static class TestConfigHelper
{
    public static ProxyConfig CreateTestProxyConfig(
        string host = "192.168.1.100",
        int port = 8080,
        bool systemProxyEnabled = false,
        bool gitProxyEnabled = false,
        string bypassList = "localhost;127.0.0.1")
    {
        return new ProxyConfig
        {
            Host = host,
            Port = port,
            SystemProxyEnabled = systemProxyEnabled,
            GitProxyEnabled = gitProxyEnabled,
            BypassList = bypassList,
            IsEnabled = systemProxyEnabled || gitProxyEnabled
        };
    }

    public static ProxyConfig CreateDefaultConfig()
    {
        return new ProxyConfig();
    }

    public static ProxyConfig CreateActiveConfig()
    {
        return new ProxyConfig
        {
            IsEnabled = true,
            Host = "172.16.65.62",
            Port = 3128,
            SystemProxyEnabled = true,
            GitProxyEnabled = true,
            BypassList = "192.168.52.*;*.example.com"
        };
    }
}
