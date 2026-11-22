using FluentAssertions;
using GitProxyManager.Services;

namespace GitProxyManager.Tests.UnitTests.Services;

[Collection("SequentialTests")]
public class SystemProxyServiceTests
{
    [Fact]
    public void ReadCurrentConfig_ReturnsConfig()
    {
        var config = SystemProxyService.ReadCurrentConfig();

        config.Should().NotBeNull();
    }

    [Fact]
    public void ReadBypassList_ReturnsString()
    {
        var bypass = SystemProxyService.ReadBypassList();

        bypass.Should().NotBeNull();
    }

    [Fact]
    public void RemoveProxy_NoException()
    {
        var act = () => SystemProxyService.RemoveProxy();
        act.Should().NotThrow();
    }

    [Fact]
    public void ApplyProxy_EmptyHost_NoException()
    {
        var act = () => SystemProxyService.ApplyProxy("", 3128, "");
        act.Should().NotThrow();
    }

    [Fact]
    public void ApplyProxy_WithBypassList_NoException()
    {
        var act = () => SystemProxyService.ApplyProxy("192.168.1.1", 8080, "localhost;127.0.0.1");
        act.Should().NotThrow();
    }

    [Fact]
    public void ApplyProxy_EmptyBypassList_NoException()
    {
        var act = () => SystemProxyService.ApplyProxy("192.168.1.1", 8080, "");
        act.Should().NotThrow();
    }

    [Fact]
    public void ApplyProxy_NullBypassList_NoException()
    {
        var act = () => SystemProxyService.ApplyProxy("192.168.1.1", 8080, null!);
        act.Should().NotThrow();
    }

    [Fact]
    public void ApplyProxy_ThenRemove_ProxyDisabled()
    {
        SystemProxyService.ApplyProxy("192.168.1.1", 8080, "");
        SystemProxyService.RemoveProxy();

        var config = SystemProxyService.ReadCurrentConfig();
        config.SystemProxyEnabled.Should().BeFalse();
    }

    [Fact]
    public void ApplyProxy_SetsProxyServer()
    {
        SystemProxyService.ApplyProxy("172.16.65.62", 3128, "");

        var config = SystemProxyService.ReadCurrentConfig();
        config.Host.Should().Be("172.16.65.62");
        config.Port.Should().Be(3128);
        config.SystemProxyEnabled.Should().BeTrue();

        SystemProxyService.RemoveProxy();
    }

    [Fact]
    public void ReadBypassList_CleansLocalTag()
    {
        var bypass = SystemProxyService.ReadBypassList();

        bypass.Should().NotContain("<local>");
    }

    [Fact]
    public void ReadBypassList_CleansLoopbackTag()
    {
        var bypass = SystemProxyService.ReadBypassList();

        bypass.Should().NotContain("<-loopback>");
    }

    [Fact]
    public void ApplyProxy_HighPort_NoException()
    {
        var act = () => SystemProxyService.ApplyProxy("192.168.1.1", 65535, "");
        act.Should().NotThrow();

        SystemProxyService.RemoveProxy();
    }

    [Fact]
    public void ApplyProxy_LowPort_NoException()
    {
        var act = () => SystemProxyService.ApplyProxy("192.168.1.1", 1, "");
        act.Should().NotThrow();

        SystemProxyService.RemoveProxy();
    }

    [Fact]
    public void ReadCurrentConfig_AfterApply_ReadsCorrectValues()
    {
        SystemProxyService.ApplyProxy("10.0.0.1", 9090, "test.local");

        var config = SystemProxyService.ReadCurrentConfig();
        config.Host.Should().Be("10.0.0.1");
        config.Port.Should().Be(9090);
        config.SystemProxyEnabled.Should().BeTrue();

        SystemProxyService.RemoveProxy();
    }

    [Fact]
    public void ApplyProxy_BypassListWithLocal_AppendedCorrectly()
    {
        SystemProxyService.ApplyProxy("192.168.1.1", 8080, "test.example.com");

        var bypass = SystemProxyService.ReadBypassList();
        bypass.Should().Contain("test.example.com");

        SystemProxyService.RemoveProxy();
    }

    // ============================================================
    // ReadPollingState
    // ============================================================

    [Fact]
    public void ReadPollingState_ReturnsTuple()
    {
        var (isEnabled, host, port, bypassList) = SystemProxyService.ReadPollingState();

        host.Should().NotBeNull();
        port.Should().BeInRange(0, 65535);
        bypassList.Should().NotBeNull();
    }

    [Fact]
    public void ReadPollingState_AfterApplyProxy_ShowsEnabled()
    {
        SystemProxyService.ApplyProxy("192.168.1.1", 8080, "test.bypass");

        var (isEnabled, host, port, bypassList) = SystemProxyService.ReadPollingState();

        isEnabled.Should().BeTrue();
        host.Should().Be("192.168.1.1");
        port.Should().Be(8080);

        SystemProxyService.RemoveProxy();
    }

    [Fact]
    public void ReadPollingState_AfterRemove_ShowsDisabled()
    {
        SystemProxyService.ApplyProxy("192.168.1.1", 8080, "");
        SystemProxyService.RemoveProxy();

        var (isEnabled, _, _, _) = SystemProxyService.ReadPollingState();

        isEnabled.Should().BeFalse();
    }

    [Fact]
    public void ReadPollingState_BypassList_CleansLocalAndLoopback()
    {
        SystemProxyService.ApplyProxy("192.168.1.1", 8080, "custom.bypass");

        var (_, _, _, bypassList) = SystemProxyService.ReadPollingState();

        bypassList.Should().NotContain("<local>");
        bypassList.Should().NotContain("<-loopback>");

        SystemProxyService.RemoveProxy();
    }

    // ============================================================
    // CleanBypassList (now public)
    // ============================================================

    [Fact]
    public void CleanBypassList_NullInput_ReturnsEmpty()
    {
        var result = SystemProxyService.CleanBypassList(null);

        result.Should().BeEmpty();
    }

    [Fact]
    public void CleanBypassList_EmptyInput_ReturnsEmpty()
    {
        var result = SystemProxyService.CleanBypassList("");

        result.Should().BeEmpty();
    }

    [Fact]
    public void CleanBypassList_WhitespaceOnly_ReturnsEmpty()
    {
        var result = SystemProxyService.CleanBypassList("   ");

        result.Should().BeEmpty();
    }

    [Fact]
    public void CleanBypassList_RemovesLocalTag()
    {
        var result = SystemProxyService.CleanBypassList("localhost;<local>");

        result.Should().Be("localhost");
        result.Should().NotContain("<local>");
    }

    [Fact]
    public void CleanBypassList_RemovesLoopbackTag()
    {
        var result = SystemProxyService.CleanBypassList("custom.bypass;<-loopback>");

        result.Should().Be("custom.bypass");
        result.Should().NotContain("<-loopback>");
    }

    [Fact]
    public void CleanBypassList_RemovesBothTags()
    {
        var result = SystemProxyService.CleanBypassList("localhost;<local>;<-loopback>;127.0.0.1");

        result.Should().NotContain("<local>");
        result.Should().NotContain("<-loopback>");
        result.Should().Contain("localhost");
        result.Should().Contain("127.0.0.1");
    }

    [Fact]
    public void CleanBypassList_PreservesCustomEntries()
    {
        var result = SystemProxyService.CleanBypassList("192.168.52.*;*.minag.gob.cu;https://172.16.112.3:8006");

        result.Should().Contain("192.168.52.*");
        result.Should().Contain("*.minag.gob.cu");
        result.Should().Contain("https://172.16.112.3:8006");
    }

    [Fact]
    public void CleanBypassList_SemicolonSeparated_PreservesAll()
    {
        var result = SystemProxyService.CleanBypassList("a.com;b.com;c.com");

        result.Should().Be("a.com;b.com;c.com");
    }

    [Fact]
    public void CleanBypassList_OnlyTags_ReturnsEmpty()
    {
        var result = SystemProxyService.CleanBypassList("<local>;<-loopback>");

        result.Should().BeEmpty();
    }
}
