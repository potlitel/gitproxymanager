using System.Diagnostics;
using System.IO;
using GitProxyManager.Models;

namespace GitProxyManager.Services;

public static class GitProxyService
{
    public static void ApplyProxy(ProxyConfig config)
    {
        var proxy = $"http://{config.Host}:{config.Port}";
        RunGitConfig("http.proxy", proxy);
        RunGitConfig("https.proxy", proxy);
    }

    public static void RemoveProxy()
    {
        RunGitConfigUnset("http.proxy");
        RunGitConfigUnset("https.proxy");
    }

    public static ProxyConfig ReadCurrentConfig()
    {
        var config = new ProxyConfig();
        var httpProxy = RunGitConfigGet("http.proxy");

        if (!string.IsNullOrWhiteSpace(httpProxy))
        {
            config.IsEnabled = true;
            var cleaned = httpProxy.Replace("http://", "").Replace("https://", "");
            var parts = cleaned.Split(':');
            config.Host = parts.Length > 0 ? parts[0] : string.Empty;
            config.Port = parts.Length > 1 && int.TryParse(parts[1], out var port) ? port : 3128;
        }

        return config;
    }

    public static (bool isEnabled, string host, int port) ReadPollingState()
    {
        try
    {
        var homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var gitConfigPath = Path.Combine(homePath, ".gitconfig");

        if (!File.Exists(gitConfigPath))
            return (false, string.Empty, 3128);

        var lines = File.ReadAllLines(gitConfigPath);
        var inHttpSection = false;

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();

            if (line.StartsWith("[") && line.Contains("http"))
            {
                inHttpSection = true;
                continue;
            }

            if (line.StartsWith("[") && !line.Contains("http"))
            {
                inHttpSection = false;
                continue;
            }

            if (inHttpSection && line.Contains("proxy"))
            {
                var eqIndex = line.IndexOf('=');
                if (eqIndex > 0)
                {
                    var value = line[(eqIndex + 1)..].Trim().Trim('"');
                    var cleaned = value.Replace("http://", "").Replace("https://", "");
                    var parts = cleaned.Split(':');
                    var host = parts.Length > 0 ? parts[0] : string.Empty;
                    var port = parts.Length > 1 && int.TryParse(parts[1], out var p) ? p : 3128;
                    return (true, host, port);
                }
            }
        }

        return (false, string.Empty, 3128);
    }
    catch
    {
        var httpProxy = RunGitConfigGet("http.proxy");
        if (!string.IsNullOrWhiteSpace(httpProxy))
        {
            var cleaned = httpProxy.Replace("http://", "").Replace("https://", "");
            var parts = cleaned.Split(':');
            var host = parts.Length > 0 ? parts[0] : string.Empty;
            var port = parts.Length > 1 && int.TryParse(parts[1], out var p) ? p : 3128;
            return (true, host, port);
        }
        return (false, string.Empty, 3128);
    }
    }

    private static void RunGitConfig(string key, string value)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = $"config --global {key} \"{value}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        process.WaitForExit();
    }

    private static void RunGitConfigUnset(string key)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = $"config --global --unset {key}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        process.WaitForExit();
    }

    private static string RunGitConfigGet(string key)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = $"config --global --get {key}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        var output = process.StandardOutput.ReadToEnd().Trim();
        process.WaitForExit();
        return output;
    }
}