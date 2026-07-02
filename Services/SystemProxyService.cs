using Microsoft.Win32;
using GitProxyManager.Models;

namespace GitProxyManager.Services;

public static class SystemProxyService
{
    private const string InternetSettingsKey = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings";

    public static void ApplyProxy(string host, int port, string bypassList)
    {
        using var key = Registry.CurrentUser.OpenSubKey(InternetSettingsKey, writable: true);
        if (key == null) return;

        key.SetValue("ProxyEnable", 1, RegistryValueKind.DWord);
        key.SetValue("ProxyServer", $"{host}:{port}", RegistryValueKind.String);

        var finalBypass = string.IsNullOrWhiteSpace(bypassList)
            ? "<local>"
            : $"{bypassList};<local>";
        key.SetValue("ProxyOverride", finalBypass, RegistryValueKind.String);

        RefreshInternetSettings();
    }

    public static void RemoveProxy()
    {
        using var key = Registry.CurrentUser.OpenSubKey(InternetSettingsKey, writable: true);
        if (key == null) return;

        key.SetValue("ProxyEnable", 0, RegistryValueKind.DWord);
        RefreshInternetSettings();
    }

    public static ProxyConfig ReadCurrentConfig()
    {
        var config = new ProxyConfig();

        using var key = Registry.CurrentUser.OpenSubKey(InternetSettingsKey);
        if (key == null) return config;

        var proxyEnable = key.GetValue("ProxyEnable");
        if (proxyEnable is int enabled && enabled == 1)
        {
            config.SystemProxyEnabled = true;

            var proxyServer = key.GetValue("ProxyServer") as string;
            if (!string.IsNullOrWhiteSpace(proxyServer))
            {
                var cleaned = proxyServer.Replace("http://", "").Replace("https://", "");
                var parts = cleaned.Split(':');
                config.Host = parts.Length > 0 ? parts[0] : string.Empty;
                config.Port = parts.Length > 1 && int.TryParse(parts[1], out var port) ? port : 3128;
            }

            var bypass = key.GetValue("ProxyOverride") as string;
            config.BypassList = CleanBypassList(bypass);
        }

        return config;
    }

    public static string CleanBypassList(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return string.Empty;

        var parts = raw.Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Trim())
            .Where(p => p != "<local>" && p != "<-loopback>")
            .ToList();

        return string.Join(";", parts);
    }

    public static string ReadBypassList()
    {
        using var key = Registry.CurrentUser.OpenSubKey(InternetSettingsKey);
        if (key == null) return string.Empty;

        var bypass = key.GetValue("ProxyOverride") as string;
        return CleanBypassList(bypass);
    }

    public static (bool isEnabled, string host, int port, string bypassList) ReadPollingState()
    {
        using var key = Registry.CurrentUser.OpenSubKey(InternetSettingsKey);
        if (key == null) return (false, string.Empty, 3128, string.Empty);

        var proxyEnable = key.GetValue("ProxyEnable");
        var isEnabled = proxyEnable is int val && val == 1;

        var host = string.Empty;
        var port = 3128;

        var proxyServer = key.GetValue("ProxyServer") as string;
        if (!string.IsNullOrWhiteSpace(proxyServer))
        {
            var cleaned = proxyServer.Replace("http://", "").Replace("https://", "");
            var parts = cleaned.Split(':');
            host = parts.Length > 0 ? parts[0] : string.Empty;
            port = parts.Length > 1 && int.TryParse(parts[1], out var p) ? p : 3128;
        }

        var rawBypass = key.GetValue("ProxyOverride") as string;
        var bypassList = CleanBypassList(rawBypass);

        return (isEnabled, host, port, bypassList);
    }

    private static void RefreshInternetSettings()
    {
        const int optionInternetOptionPerConnectionOption = 75;
        InternetSetOption(IntPtr.Zero, optionInternetOptionPerConnectionOption, IntPtr.Zero, 0);
    }

    [System.Runtime.InteropServices.DllImport("wininet.dll", SetLastError = true)]
    private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
}
