using FluentAssertions;
using GitProxyManager.Services;

namespace GitProxyManager.Tests.UnitTests.Services;

[Collection("SequentialTests")]
public class ProxyStateServiceTests
{
    [Fact]
    public void NotifyStateChanged_RaisesEvent()
    {
        var eventRaised = false;
        Action<bool, bool, string, int, string> handler = (_, _, _, _, _) => eventRaised = true;
        ProxyStateService.ProxyStateChanged += handler;

        ProxyStateService.NotifyStateChanged(false, false, "", 0, "");

        eventRaised.Should().BeTrue();
        ProxyStateService.ProxyStateChanged -= handler;
    }

    [Fact]
    public void NotifyStateChanged_PassesCorrectParameters()
    {
        bool? receivedSystemEnabled = null;
        bool? receivedGitEnabled = null;
        string? receivedHost = null;
        int? receivedPort = null;
        string? receivedBypass = null;

        Action<bool, bool, string, int, string> handler = (sys, git, host, port, bypass) =>
        {
            receivedSystemEnabled = sys;
            receivedGitEnabled = git;
            receivedHost = host;
            receivedPort = port;
            receivedBypass = bypass;
        };

        ProxyStateService.ProxyStateChanged += handler;

        ProxyStateService.NotifyStateChanged(true, false, "192.168.1.1", 8080, "localhost;127.0.0.1");

        receivedSystemEnabled.Should().BeTrue();
        receivedGitEnabled.Should().BeFalse();
        receivedHost.Should().Be("192.168.1.1");
        receivedPort.Should().Be(8080);
        receivedBypass.Should().Be("localhost;127.0.0.1");

        ProxyStateService.ProxyStateChanged -= handler;
    }

    [Fact]
    public void NotifyStateChanged_MultipleSubscribers_AllNotified()
    {
        var notificationCount = 0;
        Action<bool, bool, string, int, string> h1 = (_, _, _, _, _) => notificationCount++;
        Action<bool, bool, string, int, string> h2 = (_, _, _, _, _) => notificationCount++;
        Action<bool, bool, string, int, string> h3 = (_, _, _, _, _) => notificationCount++;

        ProxyStateService.ProxyStateChanged += h1;
        ProxyStateService.ProxyStateChanged += h2;
        ProxyStateService.ProxyStateChanged += h3;

        ProxyStateService.NotifyStateChanged(false, false, "", 0, "");

        notificationCount.Should().Be(3);

        ProxyStateService.ProxyStateChanged -= h1;
        ProxyStateService.ProxyStateChanged -= h2;
        ProxyStateService.ProxyStateChanged -= h3;
    }

    [Fact]
    public void NotifyStateChanged_NoSubscribers_NoException()
    {
        var act = () => ProxyStateService.NotifyStateChanged(false, false, "", 0, "");
        act.Should().NotThrow();
    }

    [Fact]
    public void NotifyStateChanged_BypassList_PassedCorrectly()
    {
        string? receivedBypass = null;
        Action<bool, bool, string, int, string> handler = (_, _, _, _, bypass) => receivedBypass = bypass;
        ProxyStateService.ProxyStateChanged += handler;

        ProxyStateService.NotifyStateChanged(true, true, "10.0.0.1", 3128, "192.168.52.*;*.minag.gob.cu");

        receivedBypass.Should().Be("192.168.52.*;*.minag.gob.cu");

        ProxyStateService.ProxyStateChanged -= handler;
    }

    [Fact]
    public void NotifyStateChanged_EmptyBypassList_PassedAsEmpty()
    {
        string? receivedBypass = null;
        Action<bool, bool, string, int, string> handler = (_, _, _, _, bypass) => receivedBypass = bypass;
        ProxyStateService.ProxyStateChanged += handler;

        ProxyStateService.NotifyStateChanged(false, false, "", 0, "");

        receivedBypass.Should().BeEmpty();

        ProxyStateService.ProxyStateChanged -= handler;
    }

    [Fact]
    public void NotifyStateChanged_SystemEnabledTrue_CorrectBool()
    {
        bool? receivedSystemEnabled = null;
        Action<bool, bool, string, int, string> handler = (sys, _, _, _, _) => receivedSystemEnabled = sys;
        ProxyStateService.ProxyStateChanged += handler;

        ProxyStateService.NotifyStateChanged(true, false, "", 0, "");

        receivedSystemEnabled.Should().BeTrue();

        ProxyStateService.ProxyStateChanged -= handler;
    }

    [Fact]
    public void NotifyStateChanged_GitEnabledTrue_CorrectBool()
    {
        bool? receivedGitEnabled = null;
        Action<bool, bool, string, int, string> handler = (_, git, _, _, _) => receivedGitEnabled = git;
        ProxyStateService.ProxyStateChanged += handler;

        ProxyStateService.NotifyStateChanged(false, true, "", 0, "");

        receivedGitEnabled.Should().BeTrue();

        ProxyStateService.ProxyStateChanged -= handler;
    }

    [Fact]
    public void NotifyStateChanged_HostPassed_CorrectString()
    {
        string? receivedHost = null;
        Action<bool, bool, string, int, string> handler = (_, _, host, _, _) => receivedHost = host;
        ProxyStateService.ProxyStateChanged += handler;

        ProxyStateService.NotifyStateChanged(false, false, "proxy.example.com", 0, "");

        receivedHost.Should().Be("proxy.example.com");

        ProxyStateService.ProxyStateChanged -= handler;
    }

    [Fact]
    public void NotifyStateChanged_PortPassed_CorrectInt()
    {
        int? receivedPort = null;
        Action<bool, bool, string, int, string> handler = (_, _, _, port, _) => receivedPort = port;
        ProxyStateService.ProxyStateChanged += handler;

        ProxyStateService.NotifyStateChanged(false, false, "", 3128, "");

        receivedPort.Should().Be(3128);

        ProxyStateService.ProxyStateChanged -= handler;
    }
}
