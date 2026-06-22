using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GitProxyManager.Models;
using GitProxyManager.Services;

namespace GitProxyManager.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private ProxyConfig _config = new();

    [ObservableProperty]
    private bool _isEnabled;

    [ObservableProperty]
    private string _host = string.Empty;

    [ObservableProperty]
    private int _port = 3128;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _statusColor = "#888888";

    [ObservableProperty]
    private bool _canToggle;

    public MainViewModel()
    {
        LoadConfig();
    }

    private void LoadConfig()
    {
        _config = ConfigService.Load();
        IsEnabled = _config.IsEnabled;
        Host = _config.Host;
        Port = _config.Port;
        CanToggle = !string.IsNullOrWhiteSpace(Host);
        UpdateStatusMessage();
    }

    [RelayCommand]
    private void Apply()
    {
        _config = new ProxyConfig
        {
            IsEnabled = IsEnabled,
            Host = Host,
            Port = Port
        };

        if (IsEnabled && !string.IsNullOrWhiteSpace(Host))
        {
            GitProxyService.ApplyProxy(_config);
            StatusMessage = $"Proxy activo: {Host}:{Port}";
            StatusColor = "#4CAF50";
        }
        else
        {
            GitProxyService.RemoveProxy();
            StatusMessage = "Proxy desactivado";
            StatusColor = "#888888";
        }

        ConfigService.Save(_config);
    }

    [RelayCommand]
    private void Reset()
    {
        IsEnabled = false;
        Host = string.Empty;
        Port = 3128;
        CanToggle = false;
        GitProxyService.RemoveProxy();
        ConfigService.Save(new ProxyConfig());
        StatusMessage = "Configuración restablecida";
        StatusColor = "#FF9800";
    }

    [RelayCommand]
    private void ShowWindow()
    {
        var window = new MainWindow();
        window.Show();
    }

    partial void OnHostChanged(string value)
    {
        CanToggle = !string.IsNullOrWhiteSpace(value);
        if (!CanToggle && IsEnabled)
        {
            IsEnabled = false;
        }
        UpdateStatusMessage();
    }

    partial void OnIsEnabledChanged(bool value)
    {
        _config.IsEnabled = value;
        UpdateStatusMessage();
    }

    private void UpdateStatusMessage()
    {
        if (IsEnabled && !string.IsNullOrWhiteSpace(Host))
        {
            StatusMessage = $"Proxy activo: {Host}:{Port}";
            StatusColor = "#4CAF50";
        }
        else
        {
            StatusMessage = "Proxy desactivado";
            StatusColor = "#888888";
        }
    }
}