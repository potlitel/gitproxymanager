using FluentAssertions;
using GitProxyManager.Models;
using GitProxyManager.Services;
using GitProxyManager.Tests.Helpers;
using GitProxyManager.ViewModels;

namespace GitProxyManager.Tests.IntegrationTests;

[Collection("SequentialTests")]
public class ResetProxyFlowTests : IDisposable
{
    private readonly MainViewModel _viewModel;

    public ResetProxyFlowTests()
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
    public async Task Reset_ConfigSavedAsDefaults()
    {
        _viewModel.Host = "192.168.1.100";
        _viewModel.Port = 8080;
        await _viewModel.ApplyCommand.ExecuteAsync(null);

        await _viewModel.ResetCommand.ExecuteAsync(null);

        var config = ConfigService.Load();
        config.Host.Should().BeEmpty();
        config.Port.Should().Be(3128);
        config.SystemProxyEnabled.Should().BeFalse();
        config.GitProxyEnabled.Should().BeFalse();
        config.BypassList.Should().BeEmpty();
    }

    [Fact]
    public async Task Reset_AllPropertiesResetToDefaults()
    {
        _viewModel.Host = "192.168.1.100";
        _viewModel.Port = 9090;
        _viewModel.BypassList = "test.local";
        _viewModel.IsSystemEnabled = true;
        _viewModel.IsGitEnabled = true;

        await _viewModel.ResetCommand.ExecuteAsync(null);

        _viewModel.Host.Should().BeEmpty();
        _viewModel.Port.Should().Be(3128);
        _viewModel.BypassList.Should().BeEmpty();
        _viewModel.IsSystemEnabled.Should().BeFalse();
        _viewModel.IsGitEnabled.Should().BeFalse();
        _viewModel.MasterToggle.Should().BeFalse();
        _viewModel.CanToggle.Should().BeFalse();
    }

    [Fact]
    public async Task Reset_TrayIconUpdated()
    {
        var stateUpdated = false;
        Action<bool, bool, string, int> handler = (_, _, _, _) => stateUpdated = true;
        ProxyStateService.ProxyStateChanged += handler;

        await _viewModel.ResetCommand.ExecuteAsync(null);

        stateUpdated.Should().BeTrue();

        ProxyStateService.ProxyStateChanged -= handler;
    }

    [Fact]
    public async Task Reset_StatusMessageShowsDesactivados()
    {
        _viewModel.Host = "192.168.1.100";
        _viewModel.IsSystemEnabled = true;
        await _viewModel.ApplyCommand.ExecuteAsync(null);

        await _viewModel.ResetCommand.ExecuteAsync(null);

        _viewModel.StatusMessage.Should().Contain("desactivados");
    }
}
