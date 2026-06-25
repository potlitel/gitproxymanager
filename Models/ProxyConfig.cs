namespace GitProxyManager.Models;

public class ProxyConfig
{
    public bool IsEnabled { get; set; }
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 3128;
    public bool SystemProxyEnabled { get; set; }
    public bool GitProxyEnabled { get; set; }
    public string BypassList { get; set; } = string.Empty;
}