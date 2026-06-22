namespace GitProxyManager.Models;

public class ProxyConfig
{
    public bool IsEnabled { get; set; }
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 3128;
}