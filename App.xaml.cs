using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Threading;
using GitProxyManager.Services;
using GitProxyManager.ViewModels;

namespace GitProxyManager;

public partial class App : Application
{
    private MainViewModel? _viewModel;
    private TrayIcon? _trayIcon;
    private NativeMenuItem? _systemItem;
    private NativeMenuItem? _gitItem;
    private WindowIcon? _iconInactive;
    private WindowIcon? _iconActive;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        _viewModel = new MainViewModel();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            lifetime.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            lifetime.Exit += OnExit;

            var window = new MainWindow(_viewModel);
            lifetime.MainWindow = window;
            window.Show();
        }

        _iconInactive = LoadIcon("app-icon");
        _iconActive = LoadIcon("app-icon-active");

        SetupTrayIcon();

        ProxyStateService.ProxyStateChanged += OnProxyStateChanged;
        SystemStatePoller.Start();

        base.OnFrameworkInitializationCompleted();
    }

    private static WindowIcon LoadIcon(string name)
        => new(AssetLoader.Open(new Uri($"avares://GitProxyManager/Resources/Icons/{name}.png")));

    private void SetupTrayIcon()
    {
        _trayIcon = new TrayIcon
        {
            Icon = _iconInactive,
            ToolTipText = "Git Proxy Manager"
        };
        _trayIcon.Clicked += (_, _) => ShowMainWindow();

        var openItem = new NativeMenuItem("Abrir");
        openItem.Click += (_, _) => ShowMainWindow();

        _systemItem = new NativeMenuItem("Proxy del sistema")
        {
            ToggleType = MenuItemToggleType.CheckBox
        };
        _systemItem.Click += (_, _) =>
        {
            if (_viewModel == null) return;
            var enable = !_viewModel.IsSystemEnabled;
            ToggleSystemProxy(enable);
            _viewModel.SetSystemProxyEnabled(enable);
        };

        _gitItem = new NativeMenuItem("Proxy de Git")
        {
            ToggleType = MenuItemToggleType.CheckBox
        };
        _gitItem.Click += (_, _) =>
        {
            if (_viewModel == null) return;
            var enable = !_viewModel.IsGitEnabled;
            ToggleGitProxy(enable);
            _viewModel.SetGitProxyEnabled(enable);
        };

        var exitItem = new NativeMenuItem("Salir");
        exitItem.Click += (_, _) => ExitApplication();

        var menu = new NativeMenu();
        menu.Add(openItem);
        menu.Add(new NativeMenuItemSeparator());
        menu.Add(_systemItem);
        menu.Add(_gitItem);
        menu.Add(new NativeMenuItemSeparator());
        menu.Add(exitItem);
        _trayIcon.Menu = menu;

        TrayIcon.SetIcons(this, new TrayIcons { _trayIcon });

        if (_viewModel != null)
        {
            _viewModel.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName is nameof(MainViewModel.IsSystemEnabled)
                    or nameof(MainViewModel.IsGitEnabled)
                    or nameof(MainViewModel.Host)
                    or nameof(MainViewModel.Port))
                {
                    UpdateTrayIcon();
                }
            };
        }

        UpdateTrayIcon();
    }

    private void OnProxyStateChanged(bool isSystemEnabled, bool isGitEnabled, string host, int port, string bypassList)
    {
        Dispatcher.UIThread.Post(UpdateTrayIcon);
    }

    private void UpdateTrayIcon()
    {
        if (_trayIcon == null || _viewModel == null) return;

        var systemEnabled = _viewModel.IsSystemEnabled;
        var gitEnabled = _viewModel.IsGitEnabled;
        var host = _viewModel.Host;
        var port = _viewModel.Port;

        _trayIcon.Icon = systemEnabled || gitEnabled ? _iconActive : _iconInactive;

        _trayIcon.ToolTipText = (systemEnabled, gitEnabled) switch
        {
            (true, true) => $"Git Proxy Manager - Ambos activos ({host}:{port})",
            (true, false) => $"Git Proxy Manager - Sistema activo ({host}:{port})",
            (false, true) => $"Git Proxy Manager - Git activo ({host}:{port})",
            _ => "Git Proxy Manager - Sin proxy activo"
        };

        if (_systemItem != null) _systemItem.IsChecked = systemEnabled;
        if (_gitItem != null) _gitItem.IsChecked = gitEnabled;
    }

    private static void ToggleSystemProxy(bool enable)
    {
        var config = ConfigService.Load();
        if (enable && !string.IsNullOrWhiteSpace(config.Host))
            SystemProxyService.ApplyProxy(config.Host, config.Port, config.BypassList);
        else
            SystemProxyService.RemoveProxy();
    }

    private static void ToggleGitProxy(bool enable)
    {
        var config = ConfigService.Load();
        if (enable && !string.IsNullOrWhiteSpace(config.Host))
            GitProxyService.ApplyProxy(config);
        else
            GitProxyService.RemoveProxy();
    }

    private void ShowMainWindow()
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime lifetime) return;

        var window = lifetime.MainWindow as MainWindow;
        if (window == null)
        {
            window = new MainWindow(_viewModel);
            lifetime.MainWindow = window;
        }

        if (!window.IsVisible) window.Show();
        window.Activate();
    }

    private void ExitApplication()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            foreach (var window in lifetime.Windows.OfType<MainWindow>())
                window.AllowClose = true;

            lifetime.Shutdown();
        }
    }

    private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        SystemStatePoller.Stop();
        ProxyStateService.ProxyStateChanged -= OnProxyStateChanged;
        _trayIcon?.Dispose();
    }
}
