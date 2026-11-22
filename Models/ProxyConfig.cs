namespace GitProxyManager.Models;

public class ProxyConfig
{
    public bool IsEnabled { get; set; }
    public string Host { get; set; } = "172.16.65.62";
    public int Port { get; set; } = 3128;
    public bool SystemProxyEnabled { get; set; }
    public bool GitProxyEnabled { get; set; }
    public string BypassList { get; set; } = "192.168.52.*;*.minag.gob.cu;https://172.16.112.3:8006";
}