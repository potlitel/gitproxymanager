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

        var toggleItem = new MenuItem
        {
            Header = "Habilitar Proxy",
            IsCheckable = true,
            Foreground = new SolidColorBrush(Color.FromRgb(0xCD, 0xD6, 0xF4))
        };
        toggleItem.Checked += (s, args) => ToggleProxy(true);
        toggleItem.Unchecked += (s, args) => ToggleProxy(false);
        contextMenu.Items.Add(toggleItem);

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

        var config = GitProxyService.ReadCurrentConfig();
        UpdateTrayIcon(config.IsEnabled, config.Host, config.Port);
    }

    private void OnProxyStateChanged(bool isEnabled, string host, int port)
    {
        UpdateTrayIcon(isEnabled, host, port);
    }

    private void ShowMainWindow()
    {
        foreach (Window window in Current.Windows)
        {
            if (window is MainWindow mainWindow)
            {
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
        }
        catch
        {
            _iconInactive = CreateDefaultIcon();
            _iconActive = _iconInactive;
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

    private void UpdateTrayIcon(bool isEnabled, string host, int port)
    {
        if (_trayIcon == null) return;

        _trayIcon.Icon = isEnabled ? _iconActive : _iconInactive;
        _trayIcon.ToolTipText = isEnabled
            ? $"Git Proxy Manager - Activo ({host}:{port})"
            : "Git Proxy Manager - Inactivo";
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

    private void ToggleProxy(bool enable)
    {
        if (enable)
        {
            var config = ConfigService.Load();
            if (!string.IsNullOrWhiteSpace(config.Host))
            {
                GitProxyService.ApplyProxy(config);
                UpdateTrayIcon(true, config.Host, config.Port);
            }
        }
        else
        {
            GitProxyService.RemoveProxy();
            UpdateTrayIcon(false, string.Empty, 0);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        ProxyStateService.ProxyStateChanged -= OnProxyStateChanged;
        _trayIcon?.Dispose();
        base.OnExit(e);
    }
}
