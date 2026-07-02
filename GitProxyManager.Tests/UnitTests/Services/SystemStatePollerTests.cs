using FluentAssertions;
using GitProxyManager.Services;

namespace GitProxyManager.Tests.UnitTests.Services;

[Collection("SequentialTests")]
public class SystemStatePollerTests
{
    [Fact]
    public void Start_InitializesState()
    {
        var act = () => SystemStatePoller.Start();
        act.Should().NotThrow();

        SystemStatePoller.Stop();
    }

    [Fact]
    public void Stop_AfterStart_NoException()
    {
        SystemStatePoller.Start();

        var act = () => SystemStatePoller.Stop();
        act.Should().NotThrow();
    }

    [Fact]
    public void Stop_WithoutStart_NoException()
    {
        SystemStatePoller.Stop();

        var act = () => SystemStatePoller.Stop();
        act.Should().NotThrow();
    }

    [Fact]
    public void Start_CalledTwice_DoesNotThrow()
    {
        SystemStatePoller.Start();

        var act = () => SystemStatePoller.Start();
        act.Should().NotThrow();

        SystemStatePoller.Stop();
    }

    [Fact]
    public void Start_CreatesPollerLog()
    {
        var logPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "GitProxyManager",
            "poller.log");

        SystemStatePoller.Start();

        File.Exists(logPath).Should().BeTrue();
        var content = File.ReadAllText(logPath);
        content.Should().Contain("Poller started");
        content.Should().Contain("INIT");
        content.Should().Contain("Timer started");

        SystemStatePoller.Stop();
    }

    [Fact]
    public void Stop_LogsPollerStopped()
    {
        var logPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "GitProxyManager",
            "poller.log");

        SystemStatePoller.Start();
        SystemStatePoller.Stop();

        var content = File.ReadAllText(logPath);
        content.Should().Contain("Poller stopped");
    }

    [Fact]
    public void Start_LogsInitialState()
    {
        var logPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "GitProxyManager",
            "poller.log");

        SystemStatePoller.Start();

        var content = File.ReadAllText(logPath);
        content.Should().Contain("INIT sys=");
        content.Should().Contain("INIT git=");

        SystemStatePoller.Stop();
    }

    [Fact]
    public void Start_LogsSysEnabledAndDisabled()
    {
        var logPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "GitProxyManager",
            "poller.log");

        SystemStatePoller.Start();

        var content = File.ReadAllText(logPath);
        content.Should().MatchRegex(@"INIT sys=(True|False)");
        content.Should().MatchRegex(@"INIT git=(True|False)");

        SystemStatePoller.Stop();
    }
}
