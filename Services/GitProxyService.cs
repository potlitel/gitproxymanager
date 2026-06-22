using System.Diagnostics;
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