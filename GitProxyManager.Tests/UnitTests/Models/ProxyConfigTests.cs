using FluentAssertions;
using GitProxyManager.Models;

namespace GitProxyManager.Tests.UnitTests.Models;

[Collection("SequentialTests")]
public class ProxyConfigTests
{
    [Fact]
    public void DefaultConfig_HasCorrectDefaultValues()
    {
        var config = new ProxyConfig();

        config.Host.Should().Be("172.16.65.62");
        config.Port.Should().Be(3128);
        config.BypassList.Should().Be("192.168.52.*;*.minag.gob.cu;https://172.16.112.3:8006");
        config.IsEnabled.Should().BeFalse();
        config.SystemProxyEnabled.Should().BeFalse();
        config.GitProxyEnabled.Should().BeFalse();
    }

    [Fact]
    public void DefaultConfig_HostIsNotDefaultProxy()
    {
        var config = new ProxyConfig();
        config.Host.Should().NotBe("proxy.ejemplo.com");
        config.Host.Should().Be("172.16.65.62");
    }

    [Fact]
    public void DefaultConfig_BypassListContainsCubanDomains()
    {
        var config = new ProxyConfig();
        config.BypassList.Should().Contain("minag.gob.cu");
        config.BypassList.Should().Contain("192.168.52.*");
        config.BypassList.Should().Contain("172.16.112.3");
    }

    [Fact]
    public void Config_CanSetAllProperties()
    {
        var config = new ProxyConfig
        {
            IsEnabled = true,
            Host = "10.0.0.1",
            Port = 9090,
            SystemProxyEnabled = true,
            GitProxyEnabled = true,
            BypassList = "*.custom.local"
        };

        config.IsEnabled.Should().BeTrue();
        config.Host.Should().Be("10.0.0.1");
        config.Port.Should().Be(9090);
        config.SystemProxyEnabled.Should().BeTrue();
        config.GitProxyEnabled.Should().BeTrue();
        config.BypassList.Should().Be("*.custom.local");
    }

    [Fact]
    public void Config_DefaultValues_DontInterfereWithEachOther()
    {
        var config1 = new ProxyConfig();
        var config2 = new ProxyConfig();

        config1.Host.Should().Be(config2.Host);
        config1.Port.Should().Be(config2.Port);
        config1.BypassList.Should().Be(config2.BypassList);
    }

    [Fact]
    public void Config_Serialization_PreservesDefaults()
    {
        var config = new ProxyConfig();
        var json = System.Text.Json.JsonSerializer.Serialize(config);
        var deserialized = System.Text.Json.JsonSerializer.Deserialize<ProxyConfig>(json);

        deserialized.Should().NotBeNull();
        deserialized!.Host.Should().Be("172.16.65.62");
        deserialized.Port.Should().Be(3128);
        deserialized.BypassList.Should().Be("192.168.52.*;*.minag.gob.cu;https://172.16.112.3:8006");
    }

    [Fact]
    public void Config_EmptyJson_DeserializesWithDefaults()
    {
        var json = "{}";
        var config = System.Text.Json.JsonSerializer.Deserialize<ProxyConfig>(json);

        config.Should().NotBeNull();
        config!.Host.Should().Be("172.16.65.62");
        config.Port.Should().Be(3128);
        config.BypassList.Should().Be("192.168.52.*;*.minag.gob.cu;https://172.16.112.3:8006");
    }

    [Fact]
    public void Config_PartialJson_DefaultsApplied()
    {
        var json = """{"Host": "10.0.0.1"}""";
        var config = System.Text.Json.JsonSerializer.Deserialize<ProxyConfig>(json);

        config.Should().NotBeNull();
        config!.Host.Should().Be("10.0.0.1");
        config.Port.Should().Be(3128);
        config.BypassList.Should().Be("192.168.52.*;*.minag.gob.cu;https://172.16.112.3:8006");
    }
}
