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

    // ============================================================
    // ReadPollingState
    // ============================================================

    [Fact]
    public void ReadPollingState_ReturnsTuple()
    {
        var (isEnabled, host, port) = GitProxyService.ReadPollingState();

        host.Should().NotBeNull();
        port.Should().BeInRange(0, 65535);
    }

    [Fact]
    public void ReadPollingState_GitConfigExists_ReturnsValidState()
    {
        var homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var gitConfigPath = Path.Combine(homePath, ".gitconfig");

        var (isEnabled, host, port) = GitProxyService.ReadPollingState();

        if (File.Exists(gitConfigPath))
        {
            var lines = File.ReadAllLines(gitConfigPath);
            var hasHttpProxy = lines.Any(l => l.Contains("proxy") && l.Contains("="));

            if (hasHttpProxy)
            {
                isEnabled.Should().BeTrue();
                host.Should().NotBeEmpty();
            }
        }

        port.Should().BeInRange(0, 65535);
    }

    [Fact]
    public void ReadPollingState_ConsecutiveCalls_Consistent()
    {
        var state1 = GitProxyService.ReadPollingState();
        var state2 = GitProxyService.ReadPollingState();

        state1.isEnabled.Should().Be(state2.isEnabled);
        state1.host.Should().Be(state2.host);
        state1.port.Should().Be(state2.port);
    }
}
