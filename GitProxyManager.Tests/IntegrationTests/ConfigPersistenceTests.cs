using FluentAssertions;
using GitProxyManager.Models;
using GitProxyManager.Services;
using GitProxyManager.Tests.Helpers;

namespace GitProxyManager.Tests.IntegrationTests;

[Collection("SequentialTests")]
public class ConfigPersistenceTests : IDisposable
{
    public void Dispose()
    {
        TestFileHelper.CleanupTestFiles();
    }

    [Fact]
    public void ConfigRoundtrip_SaveLoad_PreservesAllFields()
    {
        var originalConfig = new ProxyConfig
        {
            IsEnabled = true,
            Host = "172.16.65.62",
            Port = 3128,
            SystemProxyEnabled = true,
            GitProxyEnabled = true,
            BypassList = "192.168.52.*;*.minag.gob.cu;https://172.16.112.3:8006"
        };

        ConfigService.Save(originalConfig);
        var loadedConfig = ConfigService.Load();

        loadedConfig.IsEnabled.Should().Be(originalConfig.IsEnabled);
        loadedConfig.Host.Should().Be(originalConfig.Host);
        loadedConfig.Port.Should().Be(originalConfig.Port);
        loadedConfig.SystemProxyEnabled.Should().Be(originalConfig.SystemProxyEnabled);
        loadedConfig.GitProxyEnabled.Should().Be(originalConfig.GitProxyEnabled);
        loadedConfig.BypassList.Should().Be(originalConfig.BypassList);
    }

    [Fact]
    public void ConfigRoundtrip_MultipleSavesLastWins()
    {
        var config1 = new ProxyConfig
        {
            Host = "192.168.1.1",
            Port = 8080
        };

        var config2 = new ProxyConfig
        {
            Host = "10.0.0.1",
            Port = 9090
        };

        ConfigService.Save(config1);
        ConfigService.Save(config2);
        var loaded = ConfigService.Load();

        loaded.Host.Should().Be("10.0.0.1");
        loaded.Port.Should().Be(9090);
    }

    [Fact]
    public void ConfigLoad_ReturnsValidConfig()
    {
        var config = ConfigService.Load();

        config.Should().NotBeNull();
        config.Port.Should().BeInRange(0, 65535);
    }

    [Fact]
    public void Save_InvalidCharsInHost_NoException()
    {
        var config = new ProxyConfig { Host = "test-host_123" };

        var act = () => ConfigService.Save(config);
        act.Should().NotThrow();
    }
}
