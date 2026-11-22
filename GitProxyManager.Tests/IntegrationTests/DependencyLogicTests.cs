using FluentAssertions;
using GitProxyManager.Models;
using GitProxyManager.Services;
using GitProxyManager.Tests.Helpers;
using GitProxyManager.ViewModels;

namespace GitProxyManager.Tests.IntegrationTests;

[Collection("SequentialTests")]
public class DependencyLogicTests : IDisposable
{
    private readonly MainViewModel _viewModel;

    public DependencyLogicTests()
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
    public async Task ActivateGit_ActivatesSystemToo()
    {
        _viewModel.Host = "192.168.1.100";

        _viewModel.IsGitEnabled = true;
        await _viewModel.ApplyCommand.ExecuteAsync(null);

        var savedConfig = ConfigService.Load();
        savedConfig.GitProxyEnabled.Should().BeTrue();
        savedConfig.SystemProxyEnabled.Should().BeTrue();
    }

    [Fact]
    public void DeactivateSystem_DeactivatesGitToo_ViaViewModel()
    {
        _viewModel.Host = "192.168.1.100";
        _viewModel.IsGitEnabled = true;
        _viewModel.IsSystemEnabled.Should().BeTrue();

        _viewModel.IsSystemEnabled = false;

        _viewModel.IsGitEnabled.Should().BeTrue();
    }

    [Fact]
    public async Task MasterToggleFullCycle_CorrectStates()
    {
        _viewModel.Host = "192.168.1.100";

        _viewModel.MasterToggle = true;
        _viewModel.IsSystemEnabled.Should().BeTrue();
        _viewModel.IsGitEnabled.Should().BeTrue();

        await _viewModel.ApplyCommand.ExecuteAsync(null);

        _viewModel.MasterToggle = false;
        _viewModel.IsSystemEnabled.Should().BeFalse();
        _viewModel.IsGitEnabled.Should().BeFalse();

        await _viewModel.ApplyCommand.ExecuteAsync(null);

        var config = ConfigService.Load();
        config.SystemProxyEnabled.Should().BeFalse();
        config.GitProxyEnabled.Should().BeFalse();
    }

    [Fact]
    public void MixedActivation_OnlyActiveOne_MasterFalse()
    {
        _viewModel.Host = "192.168.1.100";

        _viewModel.IsSystemEnabled = true;
        _viewModel.IsGitEnabled = false;

        _viewModel.MasterToggle.Should().BeFalse();
    }

    [Fact]
    public void GitOn_SystemAutoOn_MasterBecomesTrue()
    {
        _viewModel.Host = "192.168.1.100";

        _viewModel.IsGitEnabled = true;

        _viewModel.IsSystemEnabled.Should().BeTrue();
        _viewModel.IsGitEnabled.Should().BeTrue();
        _viewModel.MasterToggle.Should().BeTrue();
    }
}
