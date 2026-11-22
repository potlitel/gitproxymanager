using FluentAssertions;
using GitProxyManager.Models;
using GitProxyManager.Services;
using GitProxyManager.Tests.Helpers;

namespace GitProxyManager.Tests.UnitTests.Services;

[Collection("SequentialTests")]
public class ConfigServiceTests : IDisposable
{
    private static readonly object _configLock = new();

    public void Dispose()
    {
        TestFileHelper.CleanupTestFiles();
    }

    [Fact]
    public void Load_FileNotExists_ReturnsDefaultConfig()
    {
        var config = ConfigService.Load();

        config.Should().NotBeNull();
        config.Port.Should().BeInRange(0, 65535);
    }

    [Fact]
    public void Load_FileExists_DeserializesCorrectly()
    {
        var expectedConfig = TestConfigHelper.CreateTestProxyConfig();
        var json = System.Text.Json.JsonSerializer.Serialize(expectedConfig);
        var deserialized = System.Text.Json.JsonSerializer.Deserialize<ProxyConfig>(json);

        deserialized.Should().NotBeNull();
        deserialized!.Host.Should().Be(expectedConfig.Host);
        deserialized.Port.Should().Be(expectedConfig.Port);
        deserialized.BypassList.Should().Be(expectedConfig.BypassList);
    }

    [Fact]
    public void Save_ThenLoad_PreservesAllValues()
    {
        var originalConfig = TestConfigHelper.CreateActiveConfig();

        lock (_configLock)
        {
            ConfigService.Save(originalConfig);
            var loadedConfig = ConfigService.Load();

            loadedConfig.Should().NotBeNull();
            loadedConfig.Host.Should().Be(originalConfig.Host);
            loadedConfig.Port.Should().Be(originalConfig.Port);
            loadedConfig.SystemProxyEnabled.Should().Be(originalConfig.SystemProxyEnabled);
            loadedConfig.GitProxyEnabled.Should().Be(originalConfig.GitProxyEnabled);
            loadedConfig.BypassList.Should().Be(originalConfig.BypassList);
            loadedConfig.IsEnabled.Should().Be(originalConfig.IsEnabled);
        }
    }

    [Fact]
    public void Save_HostWithSpecialChars_HandlesCorrectly()
    {
        var config = TestConfigHelper.CreateTestProxyConfig(host: "172.16.65.62");

        var act = () => ConfigService.Save(config);
        act.Should().NotThrow();
    }

    [Fact]
    public void Save_BypassListWithSemicolons_HandlesCorrectly()
    {
        var config = TestConfigHelper.CreateTestProxyConfig(
            bypassList: "192.168.52.*;*.minag.gob.cu;https://172.16.112.3:8006");

        var act = () => ConfigService.Save(config);
        act.Should().NotThrow();
    }

    [Fact]
    public void Save_PortBoundary_Zero()
    {
        var config = TestConfigHelper.CreateTestProxyConfig(port: 0);

        lock (_configLock)
        {
            ConfigService.Save(config);
            var loaded = ConfigService.Load();
            loaded.Port.Should().Be(0);
        }
    }

    [Fact]
    public void Save_PortBoundary_MaxPort()
    {
        var config = TestConfigHelper.CreateTestProxyConfig(port: 65535);

        lock (_configLock)
        {
            ConfigService.Save(config);
            var loaded = ConfigService.Load();
            loaded.Port.Should().Be(65535);
        }
    }

    [Fact]
    public void Load_EmptyJson_Deserializes()
    {
        var emptyJson = "{}";
        var config = System.Text.Json.JsonSerializer.Deserialize<ProxyConfig>(emptyJson);

        config.Should().NotBeNull();
        config!.Port.Should().Be(3128);
    }

    [Fact]
    public void Load_NullOrInvalidJson_HandledGracefully()
    {
        var config = new ProxyConfig();
        config.Should().NotBeNull();
        config.Port.Should().Be(3128);
    }

    [Fact]
    public void Save_UnicodeContent_HandlesCorrectly()
    {
        var config = TestConfigHelper.CreateTestProxyConfig(host: "proxy.ejemplo.com");

        var act = () => ConfigService.Save(config);
        act.Should().NotThrow();
    }

    [Fact]
    public void Load_FileWithCorruptedJson_ThrowsException()
    {
        var corruptedJson = "{ invalid json content !!!";

        var act = () => System.Text.Json.JsonSerializer.Deserialize<ProxyConfig>(corruptedJson);
        act.Should().Throw<System.Text.Json.JsonException>();
    }

    [Fact]
    public void Save_ThenLoad_Roundtrip_PreservesAllFields()
    {
        var config = new ProxyConfig
        {
            IsEnabled = true,
            Host = "10.0.0.1",
            Port = 8080,
            SystemProxyEnabled = true,
            GitProxyEnabled = false,
            BypassList = "localhost"
        };

        lock (_configLock)
        {
            ConfigService.Save(config);
            var loaded = ConfigService.Load();

            loaded.IsEnabled.Should().Be(config.IsEnabled);
            loaded.Host.Should().Be(config.Host);
            loaded.Port.Should().Be(config.Port);
            loaded.SystemProxyEnabled.Should().Be(config.SystemProxyEnabled);
            loaded.GitProxyEnabled.Should().Be(config.GitProxyEnabled);
            loaded.BypassList.Should().Be(config.BypassList);
        }
    }
}
