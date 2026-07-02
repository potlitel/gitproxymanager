using FluentAssertions;
using GitProxyManager.Models;
using GitProxyManager.Services;

namespace GitProxyManager.Tests.UnitTests.Services;

[Collection("SequentialTests")]
public class ProxyStateServiceTests
{
    [Fact]
    public void NotifyStateChanged_RaisesEvent()
    {
        // Arrange
        var eventRaised = false;
        ProxyStateService.ProxyStateChanged += (_, _, _, _) => eventRaised = true;

        // Act
        ProxyStateService.NotifyStateChanged(false, false, "", 0);

        // Assert
        eventRaised.Should().BeTrue();

        // Cleanup
        ProxyStateService.ProxyStateChanged -= (_, _, _, _) => eventRaised = true;
    }

    [Fact]
    public void NotifyStateChanged_PassesCorrectParameters()
    {
        // Arrange
        bool? receivedSystemEnabled = null;
        bool? receivedGitEnabled = null;
        string? receivedHost = null;
        int? receivedPort = null;

        Action<bool, bool, string, int> handler = (sys, git, host, port) =>
        {
            receivedSystemEnabled = sys;
            receivedGitEnabled = git;
            receivedHost = host;
            receivedPort = port;
        };

        ProxyStateService.ProxyStateChanged += handler;

        // Act
        ProxyStateService.NotifyStateChanged(true, false, "192.168.1.1", 8080);

        // Assert
        receivedSystemEnabled.Should().BeTrue();
        receivedGitEnabled.Should().BeFalse();
        receivedHost.Should().Be("192.168.1.1");
        receivedPort.Should().Be(8080);

        // Cleanup
        ProxyStateService.ProxyStateChanged -= handler;
    }

    [Fact]
    public void NotifyStateChanged_MultipleSubscribers_AllNotified()
    {
        // Arrange
        var notificationCount = 0;
        Action<bool, bool, string, int> handler1 = (_, _, _, _) => notificationCount++;
        Action<bool, bool, string, int> handler2 = (_, _, _, _) => notificationCount++;
        Action<bool, bool, string, int> handler3 = (_, _, _, _) => notificationCount++;

        ProxyStateService.ProxyStateChanged += handler1;
        ProxyStateService.ProxyStateChanged += handler2;
        ProxyStateService.ProxyStateChanged += handler3;

        // Act
        ProxyStateService.NotifyStateChanged(false, false, "", 0);

        // Assert
        notificationCount.Should().Be(3);

        // Cleanup
        ProxyStateService.ProxyStateChanged -= handler1;
        ProxyStateService.ProxyStateChanged -= handler2;
        ProxyStateService.ProxyStateChanged -= handler3;
    }

    [Fact]
    public void NotifyStateChanged_NoSubscribers_NoException()
    {
        // Arrange & Act & Assert
        var act = () => ProxyStateService.NotifyStateChanged(false, false, "", 0);
        act.Should().NotThrow();
    }

    [Fact]
    public void NotifyStateChanged_SystemEnabledTrue_CorrectBool()
    {
        // Arrange
        bool? receivedSystemEnabled = null;
        Action<bool, bool, string, int> handler = (sys, _, _, _) => receivedSystemEnabled = sys;
        ProxyStateService.ProxyStateChanged += handler;

        // Act
        ProxyStateService.NotifyStateChanged(true, false, "", 0);

        // Assert
        receivedSystemEnabled.Should().BeTrue();

        // Cleanup
        ProxyStateService.ProxyStateChanged -= handler;
    }

    [Fact]
    public void NotifyStateChanged_GitEnabledTrue_CorrectBool()
    {
        // Arrange
        bool? receivedGitEnabled = null;
        Action<bool, bool, string, int> handler = (_, git, _, _) => receivedGitEnabled = git;
        ProxyStateService.ProxyStateChanged += handler;

        // Act
        ProxyStateService.NotifyStateChanged(false, true, "", 0);

        // Assert
        receivedGitEnabled.Should().BeTrue();

        // Cleanup
        ProxyStateService.ProxyStateChanged -= handler;
    }

    [Fact]
    public void NotifyStateChanged_HostPassed_CorrectString()
    {
        // Arrange
        string? receivedHost = null;
        Action<bool, bool, string, int> handler = (_, _, host, _) => receivedHost = host;
        ProxyStateService.ProxyStateChanged += handler;

        // Act
        ProxyStateService.NotifyStateChanged(false, false, "proxy.example.com", 0);

        // Assert
        receivedHost.Should().Be("proxy.example.com");

        // Cleanup
        ProxyStateService.ProxyStateChanged -= handler;
    }

    [Fact]
    public void NotifyStateChanged_PortPassed_CorrectInt()
    {
        // Arrange
        int? receivedPort = null;
        Action<bool, bool, string, int> handler = (_, _, _, port) => receivedPort = port;
        ProxyStateService.ProxyStateChanged += handler;

        // Act
        ProxyStateService.NotifyStateChanged(false, false, "", 3128);

        // Assert
        receivedPort.Should().Be(3128);

        // Cleanup
        ProxyStateService.ProxyStateChanged -= handler;
    }
}
