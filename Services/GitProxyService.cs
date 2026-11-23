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
        => RunGit("config", "--global", key, value);

    private static void RunGitConfigUnset(string key)
        => RunGit("config", "--global", "--unset", key);

    private static string RunGitConfigGet(string key)
        => RunGit("config", "--global", "--get", key);

    private static string RunGit(params string[] args)
    {
        try
        {
            var psi = new ProcessStartInfo("git")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            foreach (var arg in args) psi.ArgumentList.Add(arg);

            using var process = Process.Start(psi);
            if (process == null) return string.Empty;

            var output = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();
            return output;
        }
        catch
        {
            return string.Empty;
        }
    }
}