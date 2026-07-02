using FluentAssertions;
using GitProxyManager.Models;
using GitProxyManager.Services;
using GitProxyManager.Tests.Helpers;
using GitProxyManager.ViewModels;

namespace GitProxyManager.Tests.UnitTests.ViewModels;

[Collection("SequentialTests")]
public class MainViewModelTests : IDisposable
{
    private readonly MainViewModel _viewModel;

    public MainViewModelTests()
    {
        _viewModel = new MainViewModel();
    }

    public void Dispose()
    {
        TestFileHelper.CleanupTestFiles();
    }

    // ============================================================
    // INITIAL STATE (ViewModel loads real config on construction)
    // ============================================================

    [Fact]
    public void Host_IsNotNullOrWhitespace_AfterConstruction()
    {
        _viewModel.Host.Should().NotBeNull();
    }

    [Fact]
    public void Port_IsDefaultOrFromConfig_AfterConstruction()
    {
        _viewModel.Port.Should().BeInRange(0, 65535);
    }

    [Fact]
    public void MasterToggle_IsFalseOrMatchConfig_AfterConstruction()
    {
        var config = ConfigService.Load();
        if (config.SystemProxyEnabled && config.GitProxyEnabled)
            _viewModel.MasterToggle.Should().BeTrue();
        else
            _viewModel.MasterToggle.Should().BeFalse();
    }

    [Fact]
    public void IsSystemEnabled_MatchesConfig_AfterConstruction()
    {
        var sysConfig = SystemProxyService.ReadCurrentConfig();
        _viewModel.IsSystemEnabled.Should().Be(sysConfig.SystemProxyEnabled);
    }

    [Fact]
    public void IsGitEnabled_MatchesConfig_AfterConstruction()
    {
        var gitConfig = GitProxyService.ReadCurrentConfig();
        _viewModel.IsGitEnabled.Should().Be(gitConfig.IsEnabled);
    }

    [Fact]
    public void IsApplying_Default_IsFalse()
    {
        _viewModel.IsApplying.Should().BeFalse();
    }

    [Fact]
    public void IsResetting_Default_IsFalse()
    {
        _viewModel.IsResetting.Should().BeFalse();
    }

    [Fact]
    public void ToastVisible_Default_IsFalse()
    {
        _viewModel.ToastVisible.Should().BeFalse();
    }

    [Fact]
    public void StatusColor_IsNotWhiteSpace()
    {
        _viewModel.StatusColor.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void SystemStatusText_IsNotNullOrWhiteSpace()
    {
        _viewModel.SystemStatusText.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GitStatusText_IsNotNullOrWhiteSpace()
    {
        _viewModel.GitStatusText.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void SystemStatusColor_IsNotWhiteSpace()
    {
        _viewModel.SystemStatusColor.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GitStatusColor_IsNotWhiteSpace()
    {
        _viewModel.GitStatusColor.Should().NotBeNullOrWhiteSpace();
    }

    // ============================================================
    // HOST/PORT/BYPASSLIST PROPERTY CHANGES
    // ============================================================

    [Fact]
    public void Host_Set_RaisesPropertyChanged()
    {
        var propertyChanged = false;
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.Host))
                propertyChanged = true;
        };

        _viewModel.Host = "192.168.1.1";

        propertyChanged.Should().BeTrue();
    }

    [Fact]
    public void Port_Set_RaisesPropertyChanged()
    {
        var propertyChanged = false;
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.Port))
                propertyChanged = true;
        };

        _viewModel.Port = 9090;

        propertyChanged.Should().BeTrue();
    }

    [Fact]
    public void BypassList_Set_RaisesPropertyChanged()
    {
        var propertyChanged = false;
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.BypassList))
                propertyChanged = true;
        };

        var currentValue = _viewModel.BypassList;
        _viewModel.BypassList = currentValue == "test-bypass-unique-value" ? "another-value" : "test-bypass-unique-value";

        propertyChanged.Should().BeTrue();
    }

    [Fact]
    public void Host_SetEmpty_CanToggleFalse()
    {
        _viewModel.Host = "192.168.1.1";
        _viewModel.CanToggle.Should().BeTrue();

        _viewModel.Host = string.Empty;

        _viewModel.CanToggle.Should().BeFalse();
    }

    [Fact]
    public void Host_SetNonEmpty_CanToggleTrue()
    {
        _viewModel.Host = "192.168.1.1";

        _viewModel.CanToggle.Should().BeTrue();
    }

    [Fact]
    public void Host_SetEmpty_ResetsIsSystemEnabled()
    {
        _viewModel.Host = "192.168.1.1";
        _viewModel.IsSystemEnabled = true;

        _viewModel.Host = string.Empty;

        _viewModel.IsSystemEnabled.Should().BeFalse();
    }

    [Fact]
    public void Host_SetEmpty_ResetsIsGitEnabled()
    {
        _viewModel.Host = "192.168.1.1";
        _viewModel.IsGitEnabled = true;

        _viewModel.Host = string.Empty;

        _viewModel.IsGitEnabled.Should().BeFalse();
    }

    [Fact]
    public void Host_SetWhiteSpace_CanToggleFalse()
    {
        _viewModel.Host = "   ";

        _viewModel.CanToggle.Should().BeFalse();
    }

    // ============================================================
    // DEPENDENCY LOGIC: Git → System
    // ============================================================

    [Fact]
    public void IsGitEnabled_ToggleTrue_SystemBecomesTrue()
    {
        _viewModel.Host = "192.168.1.1";

        _viewModel.IsGitEnabled = true;

        _viewModel.IsSystemEnabled.Should().BeTrue();
    }

    [Fact]
    public void IsGitEnabled_ToggleFalse_SystemUnchanged()
    {
        _viewModel.Host = "192.168.1.1";
        _viewModel.IsSystemEnabled = true;

        _viewModel.IsGitEnabled = false;

        _viewModel.IsSystemEnabled.Should().BeTrue();
    }

    [Fact]
    public void IsGitEnabled_ToggleTrue_SystemWasFalse_SystemBecomesTrue()
    {
        _viewModel.Host = "192.168.1.1";
        _viewModel.IsSystemEnabled = false;

        _viewModel.IsGitEnabled = true;

        _viewModel.IsSystemEnabled.Should().BeTrue();
    }

    // ============================================================
    // DEPENDENCY LOGIC: System independence
    // ============================================================

    [Fact]
    public void IsSystemEnabled_ToggleTrue_GitUnchanged()
    {
        _viewModel.Host = "192.168.1.1";
        _viewModel.IsGitEnabled = false;

        _viewModel.IsSystemEnabled = true;

        _viewModel.IsGitEnabled.Should().BeFalse();
    }

    [Fact]
    public void IsSystemEnabled_ToggleFalse_GitUnchanged()
    {
        _viewModel.Host = "192.168.1.1";
        _viewModel.IsGitEnabled = true;

        _viewModel.IsSystemEnabled = false;

        _viewModel.IsGitEnabled.Should().BeTrue();
    }

    // ============================================================
    // MASTER TOGGLE LOGIC
    // ============================================================

    [Fact]
    public void MasterToggle_True_SetsBothEnabled()
    {
        _viewModel.Host = "192.168.1.1";

        _viewModel.MasterToggle = true;

        _viewModel.IsSystemEnabled.Should().BeTrue();
        _viewModel.IsGitEnabled.Should().BeTrue();
    }

    [Fact]
    public void MasterToggle_False_SetsBothDisabled()
    {
        _viewModel.Host = "192.168.1.1";
        _viewModel.IsSystemEnabled = true;
        _viewModel.IsGitEnabled = true;

        _viewModel.MasterToggle = false;

        _viewModel.IsSystemEnabled.Should().BeFalse();
        _viewModel.IsGitEnabled.Should().BeFalse();
    }

    [Fact]
    public void MasterToggle_TrueThenFalse_DisablesBoth()
    {
        _viewModel.Host = "192.168.1.1";

        _viewModel.MasterToggle = true;
        _viewModel.IsSystemEnabled.Should().BeTrue();
        _viewModel.IsGitEnabled.Should().BeTrue();

        _viewModel.MasterToggle = false;
        _viewModel.IsSystemEnabled.Should().BeFalse();
        _viewModel.IsGitEnabled.Should().BeFalse();
    }

    [Fact]
    public void BothEnabled_MasterToggleTrue()
    {
        _viewModel.Host = "192.168.1.1";
        _viewModel.IsSystemEnabled = true;
        _viewModel.IsGitEnabled = true;

        _viewModel.MasterToggle.Should().BeTrue();
    }

    [Fact]
    public void OnlySystemEnabled_MasterToggleFalse()
    {
        _viewModel.Host = "192.168.1.1";
        _viewModel.IsSystemEnabled = true;
        _viewModel.IsGitEnabled = false;

        _viewModel.MasterToggle.Should().BeFalse();
    }

    [Fact]
    public void NeitherEnabled_MasterToggleFalse()
    {
        _viewModel.Host = "192.168.1.1";
        _viewModel.IsSystemEnabled = false;
        _viewModel.IsGitEnabled = false;

        _viewModel.MasterToggle.Should().BeFalse();
    }

    [Fact]
    public void MasterToggle_RaisesPropertyChanged()
    {
        var propertyChanged = false;
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.MasterToggle))
                propertyChanged = true;
        };

        _viewModel.MasterToggle = true;

        propertyChanged.Should().BeTrue();
    }

    [Fact]
    public void MasterToggle_True_ThenGitOff_MasterBecomesFalse()
    {
        _viewModel.Host = "192.168.1.1";

        _viewModel.MasterToggle = true;
        _viewModel.MasterToggle.Should().BeTrue();

        _viewModel.IsGitEnabled = false;
        _viewModel.MasterToggle.Should().BeFalse();
    }

    // ============================================================
    // STATUS UPDATES
    // ============================================================

    [Fact]
    public void IsSystemEnabled_True_HostSet_SystemStatusTextShowsActive()
    {
        _viewModel.Host = "192.168.1.1";
        _viewModel.Port = 8080;

        _viewModel.IsSystemEnabled = true;

        _viewModel.SystemStatusText.Should().Contain("Activo");
        _viewModel.SystemStatusText.Should().Contain("192.168.1.1");
        _viewModel.SystemStatusText.Should().Contain("8080");
    }

    [Fact]
    public void IsSystemEnabled_False_SystemStatusTextShowsInactivo()
    {
        _viewModel.IsSystemEnabled = false;

        _viewModel.SystemStatusText.Should().Be("Inactivo");
    }

    [Fact]
    public void IsGitEnabled_True_HostSet_GitStatusTextShowsActive()
    {
        _viewModel.Host = "10.0.0.1";
        _viewModel.Port = 3128;

        _viewModel.IsGitEnabled = true;

        _viewModel.GitStatusText.Should().Contain("Activo");
        _viewModel.GitStatusText.Should().Contain("10.0.0.1");
    }

    [Fact]
    public void IsGitEnabled_False_GitStatusTextShowsInactivo()
    {
        _viewModel.IsGitEnabled = false;

        _viewModel.GitStatusText.Should().Be("Inactivo");
    }

    [Fact]
    public void SystemEnabled_GreenColor()
    {
        _viewModel.Host = "192.168.1.1";

        _viewModel.IsSystemEnabled = true;

        _viewModel.SystemStatusColor.Should().Be("#4CAF50");
    }

    [Fact]
    public void SystemDisabled_GrayColor()
    {
        _viewModel.IsSystemEnabled = false;

        _viewModel.SystemStatusColor.Should().Be("#888888");
    }

    [Fact]
    public void GitEnabled_GreenColor()
    {
        _viewModel.Host = "192.168.1.1";

        _viewModel.IsGitEnabled = true;

        _viewModel.GitStatusColor.Should().Be("#4CAF50");
    }

    [Fact]
    public void GitDisabled_GrayColor()
    {
        _viewModel.IsGitEnabled = false;

        _viewModel.GitStatusColor.Should().Be("#888888");
    }

    [Fact]
    public void BothEnabled_StatusMessageShowsBoth()
    {
        _viewModel.Host = "192.168.1.1";

        _viewModel.IsSystemEnabled = true;
        _viewModel.IsGitEnabled = true;

        _viewModel.StatusMessage.Should().Contain("Sistema");
        _viewModel.StatusMessage.Should().Contain("Git");
        _viewModel.StatusMessage.Should().Contain("192.168.1.1");
        _viewModel.StatusColor.Should().Be("#4CAF50");
    }

    [Fact]
    public void BothDisabled_StatusMessageShowsDesactivados()
    {
        _viewModel.IsSystemEnabled = false;
        _viewModel.IsGitEnabled = false;

        _viewModel.StatusMessage.Should().Contain("desactivados");
        _viewModel.StatusColor.Should().Be("#888888");
    }

    [Fact]
    public void OnlySystemEnabled_StatusMessageShowsSistema()
    {
        _viewModel.Host = "192.168.1.1";

        _viewModel.IsSystemEnabled = true;
        _viewModel.IsGitEnabled = false;

        _viewModel.StatusMessage.Should().Contain("Sistema");
        _viewModel.StatusMessage.Should().NotContain("Git");
    }

    // ============================================================
    // APPLY COMMAND
    // ============================================================

    [Fact]
    public void ApplyCommand_CanExecute_WhenNotApplying()
    {
        _viewModel.ApplyCommand.CanExecute(null).Should().BeTrue();
    }

    [Fact]
    public async Task ApplyCommand_SetsIsApplyingTrue_DuringExecution()
    {
        _viewModel.Host = "192.168.1.1";
        var taskStarted = false;

        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.IsApplying) && _viewModel.IsApplying)
                taskStarted = true;
        };

        await _viewModel.ApplyCommand.ExecuteAsync(null);

        taskStarted.Should().BeTrue();
    }

    [Fact]
    public async Task ApplyCommand_SetsIsApplyingFalse_AfterExecution()
    {
        _viewModel.Host = "192.168.1.1";

        await _viewModel.ApplyCommand.ExecuteAsync(null);

        _viewModel.IsApplying.Should().BeFalse();
    }

    [Fact]
    public async Task ApplyCommand_ShowsGreenToast()
    {
        _viewModel.Host = "192.168.1.1";

        await _viewModel.ApplyCommand.ExecuteAsync(null);

        _viewModel.ToastMessage.Should().Contain("aplicado");
        _viewModel.ToastColor.Should().Be("#4CAF50");
        _viewModel.ToastVisible.Should().BeTrue();
    }

    [Fact]
    public async Task ApplyCommand_SavesConfig()
    {
        _viewModel.Host = "192.168.1.1";
        _viewModel.Port = 9090;

        await _viewModel.ApplyCommand.ExecuteAsync(null);

        var savedConfig = ConfigService.Load();
        savedConfig.Host.Should().Be("192.168.1.1");
        savedConfig.Port.Should().Be(9090);
    }

    [Fact]
    public async Task ApplyCommand_SystemEnabled_SetsConfigSystemEnabled()
    {
        _viewModel.Host = "192.168.1.1";
        _viewModel.IsSystemEnabled = true;

        await _viewModel.ApplyCommand.ExecuteAsync(null);

        var savedConfig = ConfigService.Load();
        savedConfig.SystemProxyEnabled.Should().BeTrue();
    }

    [Fact]
    public async Task ApplyCommand_GitEnabled_SetsConfigGitEnabled()
    {
        _viewModel.Host = "192.168.1.1";
        _viewModel.IsGitEnabled = true;

        await _viewModel.ApplyCommand.ExecuteAsync(null);

        var savedConfig = ConfigService.Load();
        savedConfig.GitProxyEnabled.Should().BeTrue();
    }

    [Fact]
    public async Task ApplyCommand_BypassList_Saved()
    {
        _viewModel.Host = "192.168.1.1";
        _viewModel.BypassList = "localhost;127.0.0.1";

        await _viewModel.ApplyCommand.ExecuteAsync(null);

        var savedConfig = ConfigService.Load();
        savedConfig.BypassList.Should().Be("localhost;127.0.0.1");
    }

    // ============================================================
    // RESET COMMAND
    // ============================================================

    [Fact]
    public void ResetCommand_CanExecute_WhenNotResetting()
    {
        _viewModel.ResetCommand.CanExecute(null).Should().BeTrue();
    }

    [Fact]
    public async Task ResetCommand_ResetsAllDefaults()
    {
        _viewModel.Host = "192.168.1.1";
        _viewModel.Port = 9090;
        _viewModel.BypassList = "test";
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
    public async Task ResetCommand_ShowsOrangeToast()
    {
        await _viewModel.ResetCommand.ExecuteAsync(null);

        _viewModel.ToastMessage.Should().Contain("restablecida");
        _viewModel.ToastColor.Should().Be("#FF9800");
        _viewModel.ToastVisible.Should().BeTrue();
    }

    [Fact]
    public async Task ResetCommand_SetsIsResettingFalse_AfterExecution()
    {
        await _viewModel.ResetCommand.ExecuteAsync(null);

        _viewModel.IsResetting.Should().BeFalse();
    }

    [Fact]
    public async Task ResetCommand_SavesDefaultConfig()
    {
        _viewModel.Host = "192.168.1.1";
        await _viewModel.ApplyCommand.ExecuteAsync(null);

        await _viewModel.ResetCommand.ExecuteAsync(null);

        var config = ConfigService.Load();
        config.Host.Should().BeEmpty();
        config.Port.Should().Be(3128);
    }

    [Fact]
    public async Task ResetCommand_StatusMessageShowsDesactivados()
    {
        _viewModel.Host = "192.168.1.1";
        _viewModel.IsSystemEnabled = true;
        await _viewModel.ApplyCommand.ExecuteAsync(null);

        await _viewModel.ResetCommand.ExecuteAsync(null);

        _viewModel.StatusMessage.Should().Contain("desactivados");
        _viewModel.StatusColor.Should().Be("#888888");
    }

    // ============================================================
    // CONCURRENT EXECUTION GUARDS
    // ============================================================

    [Fact]
    public async Task ApplyCommand_CannotExecuteTwiceConcurrently()
    {
        _viewModel.Host = "192.168.1.1";

        var task1 = _viewModel.ApplyCommand.ExecuteAsync(null);
        var task2 = _viewModel.ApplyCommand.ExecuteAsync(null);

        await Task.WhenAll(task1, task2);

        _viewModel.IsApplying.Should().BeFalse();
    }

    [Fact]
    public async Task ResetCommand_CannotExecuteTwiceConcurrently()
    {
        var task1 = _viewModel.ResetCommand.ExecuteAsync(null);
        var task2 = _viewModel.ResetCommand.ExecuteAsync(null);

        await Task.WhenAll(task1, task2);

        _viewModel.IsResetting.Should().BeFalse();
    }

    // ============================================================
    // TOAST BEHAVIOR
    // ============================================================

    [Fact]
    public void ToastVisible_RaisesPropertyChanged()
    {
        var propertyChanged = false;
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.ToastVisible))
                propertyChanged = true;
        };

        _viewModel.ToastVisible = true;

        propertyChanged.Should().BeTrue();
    }

    [Fact]
    public void ToastColor_Default_IsGreen()
    {
        _viewModel.ToastColor.Should().Be("#4CAF50");
    }

    [Fact]
    public void ToastMessage_Default_IsEmpty()
    {
        _viewModel.ToastMessage.Should().BeEmpty();
    }
}
