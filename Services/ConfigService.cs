using System.IO;
using System.Text.Json;
using GitProxyManager.Models;

namespace GitProxyManager.Services;

public static class ConfigService
{
    private static readonly string ConfigPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "GitProxyManager",
        "config.json");

    public static ProxyConfig Load()
    {
        if (!File.Exists(ConfigPath))
            return new ProxyConfig();

        var json = File.ReadAllText(ConfigPath);
        return JsonSerializer.Deserialize<ProxyConfig>(json) ?? new ProxyConfig();
    }

    public static void Save(ProxyConfig config)
    {
        var directory = Path.GetDirectoryName(ConfigPath)!;
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConfigPath, json);
    }
}