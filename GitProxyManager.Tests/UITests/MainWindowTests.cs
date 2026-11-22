using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using FluentAssertions;

namespace GitProxyManager.Tests.UITests;

public class MainWindowTests : IDisposable
{
    private Application? _app;
    private UIA3Automation? _automation;

    public void Dispose()
    {
        _app?.Close();
        _app?.Dispose();
        _automation?.Dispose();
    }

    private (Application app, UIA3Automation automation, Window mainWindow) LaunchApp()
    {
        var appPath = GetAppPath();
        var automation = new UIA3Automation();
        var app = Application.Launch(appPath);

        var mainWindow = app.GetMainWindow(automation, TimeSpan.FromSeconds(10));
        mainWindow.Should().NotBeNull("Main window should appear after launch");

        return (app, automation, mainWindow);
    }

    private string GetAppPath()
    {
        var projectDir = Directory.GetCurrentDirectory();
        while (projectDir != null && !File.Exists(Path.Combine(projectDir, "GitProxyManager.csproj")))
        {
            projectDir = Directory.GetParent(projectDir)?.FullName;
        }

        if (projectDir == null)
            throw new DirectoryNotFoundException("Could not find project directory");

        var binPath = Path.Combine(projectDir, "bin", "Debug", "net10.0-windows");
        if (!Directory.Exists(binPath))
            binPath = Path.Combine(projectDir, "bin", "Release", "net10.0-windows");

        return Path.Combine(binPath, "GitProxyManager.exe");
    }

    [Fact]
    public void MainWindow_Opens_TitleIsCorrect()
    {
        // Arrange & Act
        (_app, _automation, var mainWindow) = LaunchApp();

        // Assert
        mainWindow.Title.Should().Be("Git Proxy Manager");
    }

    [Fact]
    public void MainWindow_IsVisible_OnLaunch()
    {
        // Arrange & Act
        (_app, _automation, var mainWindow) = LaunchApp();

        // Assert
        mainWindow.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public void MainWindow_ContainsMasterToggle()
    {
        // Arrange & Act
        (_app, _automation, var mainWindow) = LaunchApp();

        // Assert
        var masterToggle = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("MasterToggleProxy"));
        masterToggle.Should().NotBeNull("Master toggle button should exist");
    }

    [Fact]
    public void MainWindow_ContainsSystemToggle()
    {
        // Arrange & Act
        (_app, _automation, var mainWindow) = LaunchApp();

        // Assert
        var systemToggle = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("SystemToggleProxy"));
        systemToggle.Should().NotBeNull("System toggle button should exist");
    }

    [Fact]
    public void MainWindow_ContainsGitToggle()
    {
        // Arrange & Act
        (_app, _automation, var mainWindow) = LaunchApp();

        // Assert
        var gitToggle = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("GitToggleProxy"));
        gitToggle.Should().NotBeNull("Git toggle button should exist");
    }

    [Fact]
    public void MainWindow_ContainsApplyButton()
    {
        // Arrange & Act
        (_app, _automation, var mainWindow) = LaunchApp();

        // Assert
        var buttons = mainWindow.FindAllDescendants(cf => cf.ByControlType(ControlType.Button));
        var applyButton = buttons.FirstOrDefault(b => b.Name.Contains("Aplicar"));
        applyButton.Should().NotBeNull("Apply button should exist");
    }

    [Fact]
    public void MainWindow_ContainsResetButton()
    {
        // Arrange & Act
        (_app, _automation, var mainWindow) = LaunchApp();

        // Assert
        var buttons = mainWindow.FindAllDescendants(cf => cf.ByControlType(ControlType.Button));
        var resetButton = buttons.FirstOrDefault(b => b.Name.Contains("Restablecer"));
        resetButton.Should().NotBeNull("Reset button should exist");
    }

    [Fact]
    public void MainWindow_ContainsHostTextBox()
    {
        // Arrange & Act
        (_app, _automation, var mainWindow) = LaunchApp();

        // Assert
        var textBoxes = mainWindow.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
        textBoxes.Should().HaveCountGreaterOrEqualTo(2, "Should have at least Host and Port text boxes");
    }

    [Fact]
    public void MainWindow_TogglesStartDisabled_WhenNoHost()
    {
        // Arrange & Act
        (_app, _automation, var mainWindow) = LaunchApp();

        // Assert - toggles should be disabled when no host is set
        var systemToggle = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("SystemToggleProxy"));
        var gitToggle = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("GitToggleProxy"));

        systemToggle?.As<ToggleButton>()?.IsEnabled.Should().BeFalse();
        gitToggle?.As<ToggleButton>()?.IsEnabled.Should().BeFalse();
    }
}
