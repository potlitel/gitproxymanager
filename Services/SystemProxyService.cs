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

        if (!string.IsNullOrWhiteSpace(bypassList))
            key.SetValue("ProxyOverride", bypassList, RegistryValueKind.String);
        else
            key.DeleteValue("ProxyOverride", throwOnMissingValue: false);

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
            config.BypassList = bypass ?? string.Empty;
        }

        return config;
    }

    private static void RefreshInternetSettings()
    {
        const int optionInternetOptionPerConnectionOption = 75;
        InternetSetOption(IntPtr.Zero, optionInternetOptionPerConnectionOption, IntPtr.Zero, 0);
    }

    [System.Runtime.InteropServices.DllImport("wininet.dll", SetLastError = true)]
    private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
}