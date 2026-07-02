using FluentAssertions;
using GitProxyManager.Models;
using GitProxyManager.Services;

namespace GitProxyManager.Tests.UnitTests.Services;

[Collection("SequentialTests")]
public class GitProxyServiceTests
{
    [Fact]
    public void ReadCurrentConfig_ReturnsNonNull()
    {
        var config = GitProxyService.ReadCurrentConfig();
        config.Should().NotBeNull();
        config.Port.Should().BeInRange(0, 65535);
    }

    [Fact]
    public void ApplyProxy_FormatsUrlCorrectly()
    {
        var config = new ProxyConfig
        {
            Host = "172.16.65.62",
            Port = 3128
        };

        var act = () => GitProxyService.ApplyProxy(config);
        act.Should().NotThrow();

        GitProxyService.RemoveProxy();
    }

    [Fact]
    public void ApplyProxy_HostWithDots_HandlesCorrectly()
    {
        var config = new ProxyConfig
        {
            Host = "10.0.0.1",
            Port = 8080
        };

        var act = () => GitProxyService.ApplyProxy(config);
        act.Should().NotThrow();

        GitProxyService.RemoveProxy();
    }

    [Fact]
    public void RemoveProxy_NoProxyConfigured_NoException()
    {
        var act = () => GitProxyService.RemoveProxy();
        act.Should().NotThrow();
    }

    [Fact]
    public void ApplyProxy_ThenRemove_NoException()
    {
        var config = new ProxyConfig
        {
            Host = "192.168.1.100",
            Port = 9090
        };

        var act = () =>
        {
            GitProxyService.ApplyProxy(config);
            GitProxyService.RemoveProxy();
        };

        act.Should().NotThrow();
    }

    [Fact]
    public void ApplyProxy_EmptyHost_HandlesGracefully()
    {
        var config = new ProxyConfig
        {
            Host = "",
            Port = 3128
        };

        var act = () => GitProxyService.ApplyProxy(config);
        act.Should().NotThrow();
    }

    [Fact]
    public void ApplyProxy_WhitespacesInHost_HandlesCorrectly()
    {
        var config = new ProxyConfig
        {
            Host = "  192.168.1.1  ",
            Port = 8080
        };

        var act = () => GitProxyService.ApplyProxy(config);
        act.Should().NotThrow();

        GitProxyService.RemoveProxy();
    }

    [Fact]
    public void ApplyProxy_SetsBothHttpAndHttps()
    {
        var config = new ProxyConfig
        {
            Host = "192.168.1.1",
            Port = 3128
        };

        var act = () => GitProxyService.ApplyProxy(config);
        act.Should().NotThrow();

        GitProxyService.RemoveProxy();
    }

    [Fact]
    public void ApplyProxy_BigPort_HandlesCorrectly()
    {
        var config = new ProxyConfig
        {
            Host = "192.168.1.1",
            Port = 65535
        };

        var act = () => GitProxyService.ApplyProxy(config);
        act.Should().NotThrow();

        GitProxyService.RemoveProxy();
    }

    [Fact]
    public void ApplyProxy_SmallPort_HandlesCorrectly()
    {
        var config = new ProxyConfig
        {
            Host = "192.168.1.1",
            Port = 1
        };

        var act = () => GitProxyService.ApplyProxy(config);
        act.Should().NotThrow();

        GitProxyService.RemoveProxy();
    }
}
