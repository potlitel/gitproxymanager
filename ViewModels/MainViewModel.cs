using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GitProxyManager.Models;
using GitProxyManager.Services;

namespace GitProxyManager.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private ProxyConfig _config = new();
    private bool _isUpdatingMaster;

    [ObservableProperty]
    private string _host = string.Empty;

    [ObservableProperty]
    private int _port = 3128;

    [ObservableProperty]
    private string _bypassList = string.Empty;

    [ObservableProperty]
    private bool _masterToggle;

    [ObservableProperty]
    private bool _isSystemEnabled;

    [ObservableProperty]
    private bool _isGitEnabled;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _statusColor = "#888888";

    [ObservableProperty]
    private bool _canToggle;

    [ObservableProperty]
    private string _systemStatusText = "Inactivo";

    [ObservableProperty]
    private string _gitStatusText = "Inactivo";

    [ObservableProperty]
    private string _systemStatusColor = "#888888";

    [ObservableProperty]
    private string _gitStatusColor = "#888888";

    public MainViewModel()
    {
        LoadConfig();
    }

    private void LoadConfig()
    {
        var gitConfig = GitProxyService.ReadCurrentConfig();
        var sysConfig = SystemProxyService.ReadCurrentConfig();
        var savedConfig = ConfigService.Load();

        Host = !string.IsNullOrWhiteSpace(savedConfig.Host) ? savedConfig.Host : sysConfig.Host;
        Port = savedConfig.Port != 3128 ? savedConfig.Port : sysConfig.Port;
        BypassList = !string.IsNullOrWhiteSpace(savedConfig.BypassList) ? savedConfig.BypassList : sysConfig.BypassList;

        IsSystemEnabled = sysConfig.SystemProxyEnabled;
        IsGitEnabled = gitConfig.IsEnabled;

        CanToggle = !string.IsNullOrWhiteSpace(Host);
        UpdateStatuses();
        NotifyState();
    }

    [RelayCommand]
    private void Apply()
    {
        _config = new ProxyConfig
        {
            Host = Host,
            Port = Port,
            BypassList = BypassList,
            SystemProxyEnabled = IsSystemEnabled,
            GitProxyEnabled = IsGitEnabled,
            IsEnabled = IsSystemEnabled || IsGitEnabled
        };

        if (IsSystemEnabled && !string.IsNullOrWhiteSpace(Host))
            SystemProxyService.ApplyProxy(Host, Port, BypassList);
        else
            SystemProxyService.RemoveProxy();

        if (IsGitEnabled && !string.IsNullOrWhiteSpace(Host))
            GitProxyService.ApplyProxy(_config);
        else
            GitProxyService.RemoveProxy();

        ConfigService.Save(_config);
        UpdateStatuses();
        NotifyState();
    }

    [RelayCommand]
    private void Reset()
    {
        IsSystemEnabled = false;
        IsGitEnabled = false;
        MasterToggle = false;
        Host = string.Empty;
        Port = 3128;
        BypassList = string.Empty;
        CanToggle = false;

        SystemProxyService.RemoveProxy();
        GitProxyService.RemoveProxy();
        ConfigService.Save(new ProxyConfig());

        StatusMessage = "Configuración restablecida";
        StatusColor = "#FF9800";
        UpdateStatuses();
        NotifyState();
    }

    partial void OnHostChanged(string value)
    {
        CanToggle = !string.IsNullOrWhiteSpace(value);
        if (!CanToggle)
        {
            IsSystemEnabled = false;
            IsGitEnabled = false;
        }
        UpdateStatuses();
    }

    partial void OnIsSystemEnabledChanged(bool value)
    {
        if (_isUpdatingMaster) return;
        UpdateMasterState();
        UpdateStatuses();
    }

    partial void OnIsGitEnabledChanged(bool value)
    {
        if (_isUpdatingMaster) return;
        UpdateMasterState();
        UpdateStatuses();
    }

    partial void OnMasterToggleChanged(bool value)
    {
        _isUpdatingMaster = true;
        IsSystemEnabled = value;
        IsGitEnabled = value;
        _isUpdatingMaster = false;
        UpdateStatuses();
    }

    private void UpdateMasterState()
    {
        _isUpdatingMaster = true;
        MasterToggle = IsSystemEnabled && IsGitEnabled;
        _isUpdatingMaster = false;
    }

    private void UpdateStatuses()
    {
        if (IsSystemEnabled && !string.IsNullOrWhiteSpace(Host))
        {
            SystemStatusText = $"Activo → {Host}:{Port}";
            SystemStatusColor = "#4CAF50";
        }
        else
        {
            SystemStatusText = "Inactivo";
            SystemStatusColor = "#888888";
        }

        if (IsGitEnabled && !string.IsNullOrWhiteSpace(Host))
        {
            GitStatusText = $"Activo → {Host}:{Port}";
            GitStatusColor = "#4CAF50";
        }
        else
        {
            GitStatusText = "Inactivo";
            GitStatusColor = "#888888";
        }

        if (IsSystemEnabled || IsGitEnabled)
        {
            var activeList = new List<string>();
            if (IsSystemEnabled) activeList.Add("Sistema");
            if (IsGitEnabled) activeList.Add("Git");
            StatusMessage = $"{string.Join(" + ", activeList)} activo(s): {Host}:{Port}";
            StatusColor = "#4CAF50";
        }
        else
        {
            StatusMessage = "Ambos proxies desactivados";
            StatusColor = "#888888";
        }
    }

    private void NotifyState()
    {
        ProxyStateService.NotifyStateChanged(IsSystemEnabled, IsGitEnabled, Host, Port);
    }
}