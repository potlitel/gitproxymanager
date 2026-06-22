using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GitProxyManager.Services;

namespace GitProxyManager;

public partial class App : Application
{
    private Hardcodet.Wpf.TaskbarNotification.TaskbarIcon? _trayIcon;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _trayIcon = new Hardcodet.Wpf.TaskbarNotification.TaskbarIcon
        {
            ToolTipText = "Git Proxy Manager",
            Icon = new System.Drawing.Icon(
                System.Reflection.Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("GitProxyManager.Resources.Icons.app-icon.ico")
                ?? throw new FileNotFoundException("Icon not found"))
        };

        _trayIcon.TrayMouseDoubleClick += (s, args) =>
        {
            var window = new MainWindow();
            window.Show();
        };

        var contextMenu = new ContextMenu
        {
            Background = new SolidColorBrush(Color.FromRgb(0x31, 0x32, 0x44)),
            Foreground = new SolidColorBrush(Color.FromRgb(0xCD, 0xD6, 0xF4)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(0x58, 0x5B, 0x7A)),
            BorderThickness = new Thickness(1)
        };

        var openItem = CreateMenuItem("Abrir");
        openItem.Click += (s, args) =>
        {
            var window = new MainWindow();
            window.Show();
        };
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

        var config = GitProxyService.ReadCurrentConfig();
        _trayIcon.ToolTipText = config.IsEnabled
            ? $"Git Proxy Manager - Activo ({config.Host}:{config.Port})"
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
                if (_trayIcon != null)
                    _trayIcon.ToolTipText = $"Git Proxy Manager - Activo ({config.Host}:{config.Port})";
            }
        }
        else
        {
            GitProxyService.RemoveProxy();
            if (_trayIcon != null)
                _trayIcon.ToolTipText = "Git Proxy Manager - Inactivo";
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _trayIcon?.Dispose();
        base.OnExit(e);
    }
}