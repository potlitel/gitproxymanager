using Xunit;

namespace GitProxyManager.Tests.Helpers;

/// <summary>
/// Prueba de UI que solo se ejecuta en Windows (FlaUI requiere UIA3).
/// </summary>
public sealed class WindowsOnlyFactAttribute : FactAttribute
{
    public WindowsOnlyFactAttribute()
    {
        if (!OperatingSystem.IsWindows())
            Skip = "Requiere Windows y UI Automation (FlaUI UIA3).";
    }
}
