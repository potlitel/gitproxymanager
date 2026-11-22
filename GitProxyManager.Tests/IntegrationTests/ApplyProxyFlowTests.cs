using FluentAssertions;
using GitProxyManager.Models;
using GitProxyManager.Services;
using GitProxyManager.Tests.Helpers;
using GitProxyManager.ViewModels;

namespace GitProxyManager.Tests.IntegrationTests;

[Collection("SequentialTests")]
public class ApplyProxyFlowTests : IDisposable
{
    private readonly MainViewModel _viewModel;

    public ApplyProxyFlowTests()
    {
        _viewModel = new MainViewModel();
    }

    public void Dispose()
    {
        TestFileHelper.CleanupTestFiles();
        try
        {
            SystemProxyService.RemoveProxy();
            GitProxyService.RemoveProxy();
        }
        catch { }
    }

    [Fact]
    public async Task Apply_SystemEnabled_CallsSystemProxyService()
    {
        _viewModel.Host = "192.168.1.100";
        _viewModel.Port = 8080;
        _viewModel.IsSystemEnabled = true;

        await _viewModel.ApplyCommand.ExecuteAsync(null);

        var savedConfig = ConfigService.Load();
        savedConfig.SystemProxyEnabled.Should().BeTrue();
        savedConfig.Host.Should().Be("192.168.1.100");
        savedConfig.Port.Should().Be(8080);
    }

    [Fact]
    public async Task Apply_GitEnabled_CallsGitProxyService()
    {
        _viewModel.Host = "192.168.1.100";
        _viewModel.Port = 8080;
        _viewModel.IsGitEnabled = true;

        await _viewModel.ApplyCommand.ExecuteAsync(null);

        var savedConfig = ConfigService.Load();
        savedConfig.GitProxyEnabled.Should().BeTrue();
        savedConfig.Host.Should().Be("192.168.1.100");
    }

    [Fact]
    public async Task Apply_BothEnabled_BothSetInConfig()
    {
        _viewModel.Host = "172.16.65.62";
        _viewModel.Port = 3128;
        _viewModel.IsSystemEnabled = true;
        _viewModel.IsGitEnabled = true;

        await _viewModel.ApplyCommand.ExecuteAsync(null);

        var savedConfig = ConfigService.Load();
        savedConfig.SystemProxyEnabled.Should().BeTrue();
        savedConfig.GitProxyEnabled.Should().BeTrue();
    }

    [Fact]
    public async Task Apply_ConfigSavedToFile()
    {
        _viewModel.Host = "192.168.1.100";
        _viewModel.Port = 8080;
        _viewModel.IsSystemEnabled = true;
        _viewModel.IsGitEnabled = true;
        _viewModel.BypassList = "localhost;127.0.0.1";

        await _viewModel.ApplyCommand.ExecuteAsync(null);

        var savedConfig = ConfigService.Load();
        savedConfig.Host.Should().Be("192.168.1.100");
        savedConfig.Port.Should().Be(8080);
        savedConfig.SystemProxyEnabled.Should().BeTrue();
        savedConfig.GitProxyEnabled.Should().BeTrue();
        savedConfig.BypassList.Should().Be("localhost;127.0.0.1");
    }

    [Fact]
    public async Task Apply_TrayIconStateUpdated()
    {
        var stateUpdated = false;
        Action<bool, bool, string, int, string> handler = (_, _, _, _, _) => stateUpdated = true;
        ProxyStateService.ProxyStateChanged += handler;

        _viewModel.Host = "192.168.1.100";
        _viewModel.IsSystemEnabled = true;

        await _viewModel.ApplyCommand.ExecuteAsync(null);

        stateUpdated.Should().BeTrue();

        ProxyStateService.ProxyStateChanged -= handler;
    }

    [Fact]
    public async Task Apply_BypassListWithSemicolons_SavedCorrectly()
    {
        _viewModel.Host = "192.168.1.100";
        _viewModel.BypassList = "192.168.52.*;*.minag.gob.cu;https://172.16.112.3:8006";

        await _viewModel.ApplyCommand.ExecuteAsync(null);

        var savedConfig = ConfigService.Load();
        savedConfig.BypassList.Should().Be("192.168.52.*;*.minag.gob.cu;https://172.16.112.3:8006");
    }
}
