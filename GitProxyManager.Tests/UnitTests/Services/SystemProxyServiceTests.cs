using FluentAssertions;
using GitProxyManager.Services;

namespace GitProxyManager.Tests.UnitTests.Services;

[Collection("SequentialTests")]
public class SystemProxyServiceTests
{
    [Fact]
    public void ReadCurrentConfig_ReturnsConfig()
    {
        // Act
        var config = SystemProxyService.ReadCurrentConfig();

        // Assert
        config.Should().NotBeNull();
    }

    [Fact]
    public void ReadBypassList_ReturnsString()
    {
        // Act
        var bypass = SystemProxyService.ReadBypassList();

        // Assert
        bypass.Should().NotBeNull();
    }

    [Fact]
    public void RemoveProxy_NoException()
    {
        // Act & Assert
        var act = () => SystemProxyService.RemoveProxy();
        act.Should().NotThrow();
    }

    [Fact]
    public void ApplyProxy_EmptyHost_NoException()
    {
        // Act & Assert
        var act = () => SystemProxyService.ApplyProxy("", 3128, "");
        act.Should().NotThrow();
    }

    [Fact]
    public void ApplyProxy_WithBypassList_NoException()
    {
        // Act & Assert
        var act = () => SystemProxyService.ApplyProxy("192.168.1.1", 8080, "localhost;127.0.0.1");
        act.Should().NotThrow();
    }

    [Fact]
    public void ApplyProxy_EmptyBypassList_NoException()
    {
        // Act & Assert
        var act = () => SystemProxyService.ApplyProxy("192.168.1.1", 8080, "");
        act.Should().NotThrow();
    }

    [Fact]
    public void ApplyProxy_NullBypassList_NoException()
    {
        // Act & Assert
        var act = () => SystemProxyService.ApplyProxy("192.168.1.1", 8080, null!);
        act.Should().NotThrow();
    }

    [Fact]
    public void ApplyProxy_ThenRemove_ProxyDisabled()
    {
        // Arrange & Act
        SystemProxyService.ApplyProxy("192.168.1.1", 8080, "");
        SystemProxyService.RemoveProxy();

        // Assert
        var config = SystemProxyService.ReadCurrentConfig();
        config.SystemProxyEnabled.Should().BeFalse();
    }

    [Fact]
    public void ApplyProxy_SetsProxyServer()
    {
        // Arrange & Act
        SystemProxyService.ApplyProxy("172.16.65.62", 3128, "");

        // Assert
        var config = SystemProxyService.ReadCurrentConfig();
        config.Host.Should().Be("172.16.65.62");
        config.Port.Should().Be(3128);
        config.SystemProxyEnabled.Should().BeTrue();

        // Cleanup
        SystemProxyService.RemoveProxy();
    }

    [Fact]
    public void ReadBypassList_CleansLocalTag()
    {
        // This test verifies that ReadBypassList removes <local> from the result
        // Act
        var bypass = SystemProxyService.ReadBypassList();

        // Assert - <local> should not be in the result
        bypass.Should().NotContain("<local>");
    }

    [Fact]
    public void ReadBypassList_CleansLoopbackTag()
    {
        // Act
        var bypass = SystemProxyService.ReadBypassList();

        // Assert - <-loopback> should not be in the result
        bypass.Should().NotContain("<-loopback>");
    }

    [Fact]
    public void ApplyProxy_HighPort_NoException()
    {
        // Act & Assert
        var act = () => SystemProxyService.ApplyProxy("192.168.1.1", 65535, "");
        act.Should().NotThrow();

        // Cleanup
        SystemProxyService.RemoveProxy();
    }

    [Fact]
    public void ApplyProxy_LowPort_NoException()
    {
        // Act & Assert
        var act = () => SystemProxyService.ApplyProxy("192.168.1.1", 1, "");
        act.Should().NotThrow();

        // Cleanup
        SystemProxyService.RemoveProxy();
    }

    [Fact]
    public void ReadCurrentConfig_AfterApply_ReadsCorrectValues()
    {
        // Arrange & Act
        SystemProxyService.ApplyProxy("10.0.0.1", 9090, "test.local");

        // Assert
        var config = SystemProxyService.ReadCurrentConfig();
        config.Host.Should().Be("10.0.0.1");
        config.Port.Should().Be(9090);
        config.SystemProxyEnabled.Should().BeTrue();

        // Cleanup
        SystemProxyService.RemoveProxy();
    }

    [Fact]
    public void ApplyProxy_BypassListWithLocal_AppendedCorrectly()
    {
        // Arrange & Act
        SystemProxyService.ApplyProxy("192.168.1.1", 8080, "test.example.com");

        // Assert - <local> should be appended
        var bypass = SystemProxyService.ReadBypassList();
        bypass.Should().Contain("test.example.com");

        // Cleanup
        SystemProxyService.RemoveProxy();
    }
}
