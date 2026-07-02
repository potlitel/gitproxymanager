using System.IO;
using System.Windows.Threading;

namespace GitProxyManager.Services;

public static class SystemStatePoller
{
    private static DispatcherTimer? _timer;
    private static bool _isRunning;

    private static bool _lastSysEnabled;
    private static bool _lastGitEnabled;
    private static string _lastSysHost = string.Empty;
    private static int _lastSysPort = 3128;
    private static string _lastGitHost = string.Empty;
    private static int _lastGitPort = 3128;
    private static string _lastBypass = string.Empty;
    private static bool _initialized;

    private static readonly string LogPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "GitProxyManager",
        "poller.log");

    private static void Log(string message)
    {
        try
        {
            var dir = Path.GetDirectoryName(LogPath)!;
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.AppendAllText(LogPath, $"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
        }
        catch { }
    }

    public static void Start()
    {
        if (_isRunning) return;

        try { File.WriteAllText(LogPath, $"=== Poller started {DateTime.Now} ==={Environment.NewLine}"); } catch { }

        var sysState = SystemProxyService.ReadPollingState();
        var gitState = GitProxyService.ReadPollingState();

        Log($"INIT sys={sysState.isEnabled} host={sysState.host}:{sysState.port} bypass={sysState.bypassList}");
        Log($"INIT git={gitState.isEnabled} host={gitState.host}:{gitState.port}");

        _lastSysEnabled = sysState.isEnabled;
        _lastGitEnabled = gitState.isEnabled;
        _lastSysHost = sysState.host;
        _lastSysPort = sysState.port;
        _lastGitHost = gitState.host;
        _lastGitPort = gitState.port;
        _lastBypass = sysState.bypassList;
        _initialized = true;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(5)
        };
        _timer.Tick += OnTick;
        _timer.Start();
        _isRunning = true;

        Log("Timer started (5s interval)");
    }

    public static void Stop()
    {
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Tick -= OnTick;
            _timer = null;
        }
        _isRunning = false;
        Log("Poller stopped");
    }

    private static void OnTick(object? sender, EventArgs e)
    {
        if (!_initialized) return;

        try
        {
            var sysState = SystemProxyService.ReadPollingState();
            var gitState = GitProxyService.ReadPollingState();

            var sysChanged = sysState.isEnabled != _lastSysEnabled
                          || sysState.host != _lastSysHost
                          || sysState.port != _lastSysPort;

            var gitChanged = gitState.isEnabled != _lastGitEnabled
                          || gitState.host != _lastGitHost
                          || gitState.port != _lastGitPort;

            var bypassChanged = sysState.bypassList != _lastBypass;

            Log($"TICK sys={sysState.isEnabled}(was={_lastSysEnabled}) git={gitState.isEnabled}(was={_lastGitEnabled}) host={sysState.host}:{sysState.port} bypass={sysState.bypassList} | changed={sysChanged || gitChanged || bypassChanged}");

            if (sysChanged || gitChanged || bypassChanged)
            {
                Log($">>> CHANGE DETECTED: sysChanged={sysChanged} gitChanged={gitChanged} bypassChanged={bypassChanged}");

                _lastSysEnabled = sysState.isEnabled;
                _lastGitEnabled = gitState.isEnabled;
                _lastSysHost = sysState.host;
                _lastSysPort = sysState.port;
                _lastGitHost = gitState.host;
                _lastGitPort = gitState.port;
                _lastBypass = sysState.bypassList;

                var host = sysState.isEnabled ? sysState.host : gitState.host;
                var port = sysState.isEnabled ? sysState.port : gitState.port;

                Log($">>> FIRES EVENT: sys={sysState.isEnabled} git={gitState.isEnabled} host={host}:{port}");

                ProxyStateService.NotifyStateChanged(
                    sysState.isEnabled,
                    gitState.isEnabled,
                    host,
                    port,
                    sysState.bypassList);

                Log(">>> EVENT FIRED OK");
            }
        }
        catch (Exception ex)
        {
            Log($"EXCEPTION: {ex}");
        }
    }
}
