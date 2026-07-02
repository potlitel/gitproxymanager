namespace GitProxyManager.Tests.Helpers;

public static class TestFileHelper
{
    private static readonly string TestDirectory = Path.Combine(
        Path.GetTempPath(),
        "GitProxyManagerTests",
        Guid.NewGuid().ToString("N"));

    public static string GetTestConfigPath()
    {
        return Path.Combine(TestDirectory, "config.json");
    }

    public static string CreateTempJsonFile(string jsonContent)
    {
        var path = GetTestConfigPath();
        var directory = Path.GetDirectoryName(path)!;

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(path, jsonContent);
        return path;
    }

    public static string CreateTempJsonFile(object obj)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(obj, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });
        return CreateTempJsonFile(json);
    }

    public static void CleanupTestFiles()
    {
        if (Directory.Exists(TestDirectory))
        {
            try
            {
                Directory.Delete(TestDirectory, recursive: true);
            }
            catch
            {
                // Ignore cleanup errors in tests
            }
        }
    }

    public static void CleanupFile(string path)
    {
        if (File.Exists(path))
        {
            try
            {
                File.Delete(path);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}
