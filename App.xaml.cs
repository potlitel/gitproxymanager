using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GitProxyManager.Services;

namespace GitProxyManager;

public partial class App : Application
{
    private Hardcodet.Wpf.TaskbarNotification.TaskbarIcon? _trayIcon;
    private System.Drawing.Icon? _iconInactive;
    private System.Drawing.Icon? _iconActive;
    private System.Drawing.Icon? _iconPartial;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        LoadIcons();

        _trayIcon = new Hardcodet.Wpf.TaskbarNotification.TaskbarIcon
        {
            ToolTipText = "Git Proxy Manager",
            Icon = _iconInactive
        };

        _trayIcon.TrayMouseDoubleClick += (s, args) =>
        {
            ShowMainWindow();
        };

        var contextMenu = new ContextMenu
        {
            Background = new SolidColorBrush(Color.FromRgb(0x31, 0x32, 0x44)),
            Foreground = new SolidColorBrush(Color.FromRgb(0xCD, 0xD6, 0xF4)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(0x58, 0x5B, 0x7A)),
            BorderThickness = new Thickness(1)
        };

        var openItem = CreateMenuItem("Abrir");
        openItem.Click += (s, args) => ShowMainWindow();
        contextMenu.Items.Add(openItem);

        contextMenu.Items.Add(new Separator
        {
            Background = new SolidColorBrush(Color.FromRgb(0x58, 0x5B, 0x7A))
        });

        var systemItem = new MenuItem
        {
            Header = "Proxy del sistema",
            IsCheckable = true,
            Foreground = new SolidColorBrush(Color.FromRgb(0xCD, 0xD6, 0xF4))
        };
        systemItem.Checked += (s, args) => ToggleSystemProxy(true);
        systemItem.Unchecked += (s, args) => ToggleSystemProxy(false);
        contextMenu.Items.Add(systemItem);

        var gitItem = new MenuItem
        {
            Header = "Proxy de Git",
            IsCheckable = true,
            Foreground = new SolidColorBrush(Color.FromRgb(0xCD, 0xD6, 0xF4))
        };
        gitItem.Checked += (s, args) => ToggleGitProxy(true);
        gitItem.Unchecked += (s, args) => ToggleGitProxy(false);
        contextMenu.Items.Add(gitItem);

        contextMenu.Items.Add(new Separator
        {
            Background = new SolidColorBrush(Color.FromRgb(0x58, 0x5B, 0x7A))
        });

        var exitItem = CreateMenuItem("Salir");
        exitItem.Click += (s, args) =>
        {
            _trayIcon.Dispose();
            Shutdown();
        };
        contextMenu.Items.Add(exitItem);

        _trayIcon.ContextMenu = contextMenu;

        ProxyStateService.ProxyStateChanged += OnProxyStateChanged;

        var sysConfig = SystemProxyService.ReadCurrentConfig();
        var gitConfig = GitProxyService.ReadCurrentConfig();
        UpdateTrayIcon(sysConfig.SystemProxyEnabled, gitConfig.IsEnabled, sysConfig.Host, sysConfig.Port);
    }

    private void OnProxyStateChanged(bool isSystemEnabled, bool isGitEnabled, string host, int port)
    {
        UpdateTrayIcon(isSystemEnabled, isGitEnabled, host, port);
    }

    private void ShowMainWindow()
    {
        foreach (Window window in Current.Windows)
        {
            if (window is MainWindow mainWindow)
            {
                mainWindow.Show();
                mainWindow.Activate();
                mainWindow.BringIntoView();
                return;
            }
        }

        var newWindow = new MainWindow();
        newWindow.Show();
    }

    private void LoadIcons()
    {
        try
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            var iconStream = assembly.GetManifestResourceStream("GitProxyManager.Resources.Icons.app-icon.ico");
            _iconInactive = iconStream != null ? new System.Drawing.Icon(iconStream) : CreateDefaultIcon();

            var iconActiveStream = assembly.GetManifestResourceStream("GitProxyManager.Resources.Icons.app-icon-active.ico");
            _iconActive = iconActiveStream != null ? new System.Drawing.Icon(iconActiveStream) : _iconInactive;

            _iconPartial = _iconActive;
        }
        catch
        {
            _iconInactive = CreateDefaultIcon();
            _iconActive = _iconInactive;
            _iconPartial = _iconInactive;
        }
    }

    private System.Drawing.Icon CreateDefaultIcon()
    {
        var bitmap = new System.Drawing.Bitmap(32, 32);
        var graphics = System.Drawing.Graphics.FromImage(bitmap);
        graphics.Clear(System.Drawing.Color.Transparent);
        var brush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(137, 180, 250));
        graphics.FillEllipse(brush, 0, 0, 32, 32);
        var icon = System.Drawing.Icon.FromHandle(bitmap.GetHicon());
        graphics.Dispose();
        bitmap.Dispose();
        return icon;
    }

    private void UpdateTrayIcon(bool isSystemEnabled, bool isGitEnabled, string host, int port)
    {
        if (_trayIcon == null) return;

        if (isSystemEnabled && isGitEnabled)
        {
            _trayIcon.Icon = _iconActive;
            _trayIcon.ToolTipText = $"Git Proxy Manager - Ambos activos ({host}:{port})";
        }
        else if (isSystemEnabled || isGitEnabled)
        {
            _trayIcon.Icon = _iconPartial;
            var active = isSystemEnabled ? "Sistema" : "Git";
            _trayIcon.ToolTipText = $"Git Proxy Manager - {active} activo ({host}:{port})";
        }
        else
        {
            _trayIcon.Icon = _iconInactive;
            _trayIcon.ToolTipText = "Git Proxy Manager - Sin proxy activo";
        }
    }

    private static MenuItem CreateMenuItem(string header)
    {
        return new MenuItem
        {
            Header = header,
            Foreground = new SolidColorBrush(Color.FromRgb(0xCD, 0xD6, 0xF4)),
            Background = Brushes.Transparent,
            Padding = new Thickness(20, 6, 20, 6)
        };
    }

    private void ToggleSystemProxy(bool enable)
    {
        var config = ConfigService.Load();
        if (enable && !string.IsNullOrWhiteSpace(config.Host))
            SystemProxyService.ApplyProxy(config.Host, config.Port, config.BypassList);
        else
            SystemProxyService.RemoveProxy();
    }

    private void ToggleGitProxy(bool enable)
    {
        var config = ConfigService.Load();
        if (enable && !string.IsNullOrWhiteSpace(config.Host))
            GitProxyService.ApplyProxy(config);
        else
            GitProxyService.RemoveProxy();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        ProxyStateService.ProxyStateChanged -= OnProxyStateChanged;
        _trayIcon?.Dispose();
        base.OnExit(e);
    }
}