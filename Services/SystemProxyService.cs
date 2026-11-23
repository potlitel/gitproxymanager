using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text.Json;
using Avalonia;
using GitProxyManager.Models;
using Microsoft.Win32;

namespace GitProxyManager.Services;

/// <summary>
/// Gestiona el proxy del sistema. En Windows usa el registro de Internet Settings;
/// en Linux usa gsettings (GNOME) y mantiene un estado local para que la lectura
/// sea determinista incluso en sesiones sin escritorio.
/// </summary>
public static class SystemProxyService
{
    private const string InternetSettingsKey = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings";

    private static readonly string StatePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "GitProxyManager",
        "system-proxy.json");

    public static void ApplyProxy(string host, int port, string bypassList)
    {
        if (string.IsNullOrWhiteSpace(host)) return;

        var cleanHost = host.Trim();
        var cleanBypass = bypassList ?? string.Empty;

        WriteState(new SystemProxyState
        {
            Enabled = true,
            Host = cleanHost,
            Port = port,
            BypassList = cleanBypass
        });

        if (OperatingSystem.IsWindows())
            ApplyWindowsProxy(cleanHost, port, cleanBypass);
        else if (Application.Current != null)
            ApplyGnomeProxy(cleanHost, port, cleanBypass);
    }

    public static void RemoveProxy()
    {
        WriteState(new SystemProxyState());

        if (OperatingSystem.IsWindows())
            RemoveWindowsProxy();
        else if (Application.Current != null)
            RemoveGnomeProxy();
    }

    public static ProxyConfig ReadCurrentConfig()
    {
        var state = ReadState();
        var config = new ProxyConfig
        {
            SystemProxyEnabled = state.Enabled,
            Host = state.Host,
            Port = state.Port,
            BypassList = CleanBypassList(state.BypassList)
        };

        if (OperatingSystem.IsWindows())
        {
            var windows = ReadWindowsState();
            if (windows != null)
            {
                config.SystemProxyEnabled = windows.Value.isEnabled;
                if (windows.Value.isEnabled)
                {
                    config.Host = windows.Value.host;
                    config.Port = windows.Value.port;
                    config.BypassList = CleanBypassList(windows.Value.bypassList);
                }
            }
        }
        else if (Application.Current != null)
        {
            var gnome = ReadGnomeState();
            if (gnome != null)
            {
                config.SystemProxyEnabled = gnome.Value.isEnabled;
                if (gnome.Value.isEnabled)
                {
                    config.Host = gnome.Value.host;
                    config.Port = gnome.Value.port;
                    config.BypassList = CleanBypassList(gnome.Value.bypassList);
                }
            }
        }

        return config;
    }

    public static string CleanBypassList(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return string.Empty;

        var parts = raw.Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Trim())
            .Where(p => p.Length > 0 && p != "<local>" && p != "<-loopback>")
            .ToList();

        return string.Join(";", parts);
    }

    public static string ReadBypassList() => CleanBypassList(ReadCurrentConfig().BypassList);

    public static (bool isEnabled, string host, int port, string bypassList) ReadPollingState()
    {
        var config = ReadCurrentConfig();
        return (config.SystemProxyEnabled, config.Host, config.Port, config.BypassList);
    }

    // ------------------------- Estado local -------------------------

    private sealed class SystemProxyState
    {
        public bool Enabled { get; set; }
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 3128;
        public string BypassList { get; set; } = string.Empty;
    }

    private static SystemProxyState ReadState()
    {
        try
        {
            if (!File.Exists(StatePath)) return new SystemProxyState();
            var json = File.ReadAllText(StatePath);
            return JsonSerializer.Deserialize<SystemProxyState>(json) ?? new SystemProxyState();
        }
        catch
        {
            return new SystemProxyState();
        }
    }

    private static void WriteState(SystemProxyState state)
    {
        try
        {
            var directory = Path.GetDirectoryName(StatePath)!;
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            File.WriteAllText(StatePath, JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch
        {
            // El estado en disco es best-effort.
        }
    }

    // ------------------------- Windows -------------------------

    [SupportedOSPlatform("windows")]
    private static void ApplyWindowsProxy(string host, int port, string bypassList)
    {
        using var key = Registry.CurrentUser.OpenSubKey(InternetSettingsKey, writable: true);
        if (key == null) return;

        key.SetValue("ProxyEnable", 1, RegistryValueKind.DWord);
        key.SetValue("ProxyServer", $"{host}:{port}", RegistryValueKind.String);
        key.SetValue("ProxyOverride", $"{bypassList};<local>", RegistryValueKind.String);
        RefreshInternetSettings();
    }

    [SupportedOSPlatform("windows")]
    private static void RemoveWindowsProxy()
    {
        using var key = Registry.CurrentUser.OpenSubKey(InternetSettingsKey, writable: true);
        if (key == null) return;

        key.SetValue("ProxyEnable", 0, RegistryValueKind.DWord);
        RefreshInternetSettings();
    }

    [SupportedOSPlatform("windows")]
    private static (bool isEnabled, string host, int port, string bypassList)? ReadWindowsState()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(InternetSettingsKey);
            if (key == null) return null;

            var isEnabled = key.GetValue("ProxyEnable") is int enabled && enabled == 1;
            var server = key.GetValue("ProxyServer") as string ?? string.Empty;
            var (host, port) = ParseHostPort(server);
            var bypass = key.GetValue("ProxyOverride") as string ?? string.Empty;
            return (isEnabled, host, port, bypass);
        }
        catch
        {
            return null;
        }
    }

    [SupportedOSPlatform("windows")]
    private static void RefreshInternetSettings()
    {
        const int optionInternetOptionPerConnectionOption = 75;
        InternetSetOption(IntPtr.Zero, optionInternetOptionPerConnectionOption, IntPtr.Zero, 0);
    }

    [SupportedOSPlatform("windows")]
    [System.Runtime.InteropServices.DllImport("wininet.dll", SetLastError = true)]
    private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);

    // ------------------------- Linux / GNOME -------------------------

    private static void ApplyGnomeProxy(string host, int port, string bypassList)
    {
        RunGsettings("set", "org.gnome.system.proxy", "mode", "manual");
        RunGsettings("set", "org.gnome.system.proxy.http", "host", host);
        RunGsettings("set", "org.gnome.system.proxy.http", "port", port.ToString());
        RunGsettings("set", "org.gnome.system.proxy.https", "host", host);
        RunGsettings("set", "org.gnome.system.proxy.https", "port", port.ToString());

        var entries = CleanBypassList(bypassList)
            .Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(EscapeGVariantString)
            .ToList();
        if (entries.Count == 0)
        {
            entries.Add("'localhost'");
            entries.Add("'127.0.0.0/8'");
        }
        RunGsettings("set", "org.gnome.system.proxy", "ignore-hosts",
            $"[{string.Join(", ", entries)}]");
    }

    private static void RemoveGnomeProxy()
    {
        RunGsettings("set", "org.gnome.system.proxy", "mode", "none");
    }

    private static (bool isEnabled, string host, int port, string bypassList)? ReadGnomeState()
    {
        var mode = RunGsettings("get", "org.gnome.system.proxy", "mode");
        if (mode == null) return null;

        var isEnabled = mode.Contains("manual", StringComparison.OrdinalIgnoreCase);
        if (!isEnabled) return (false, string.Empty, 3128, string.Empty);

        var host = UnquoteGVariant(RunGsettings("get", "org.gnome.system.proxy.http", "host")) ?? string.Empty;
        var portText = RunGsettings("get", "org.gnome.system.proxy.http", "port") ?? string.Empty;
        var port = portText.Any(char.IsDigit)
            ? int.Parse(new string(portText.Where(char.IsDigit).ToArray()))
            : 3128;
        var ignoreHosts = RunGsettings("get", "org.gnome.system.proxy", "ignore-hosts") ?? string.Empty;

        var bypassList = string.Join(";", ignoreHosts
            .Trim('[', ']')
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(UnquoteGVariant)
            .Where(p => !string.IsNullOrWhiteSpace(p)));

        return (true, host, port, bypassList);
    }

    private static string EscapeGVariantString(string value)
        => $"'{value.Replace("\\", "\\\\").Replace("'", "\\'")}'";

    private static string? UnquoteGVariant(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;
        var value = raw.Trim();
        if (value.Length >= 2 && value[0] == '\'' && value[^1] == '\'')
            return value[1..^1].Replace("\\'", "'").Replace("\\\\", "\\");
        return value;
    }

    private static string? RunGsettings(params string[] args)
    {
        try
        {
            var psi = new ProcessStartInfo("gsettings")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            foreach (var arg in args) psi.ArgumentList.Add(arg);

            using var process = Process.Start(psi);
            if (process == null) return null;

            var output = process.StandardOutput.ReadToEnd().Trim();
            if (!process.WaitForExit(3000))
            {
                process.Kill();
                return null;
            }

            return process.ExitCode == 0 ? output : null;
        }
        catch
        {
            return null;
        }
    }

    private static (string host, int port) ParseHostPort(string value)
    {
        var cleaned = value.Replace("http://", "").Replace("https://", "");
        var parts = cleaned.Split(':');
        var host = parts.Length > 0 ? parts[0] : string.Empty;
        var port = parts.Length > 1 && int.TryParse(parts[1], out var p) ? p : 3128;
        return (host, port);
    }
}
