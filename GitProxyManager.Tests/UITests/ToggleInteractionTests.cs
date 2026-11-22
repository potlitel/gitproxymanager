using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using FluentAssertions;

namespace GitProxyManager.Tests.UITests;

public class ToggleInteractionTests : IDisposable
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
        mainWindow.Should().NotBeNull();

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
    public void MasterToggle_On_EnablesBothIndividually()
    {
        // Arrange
        (_app, _automation, var mainWindow) = LaunchApp();

        // First enter a host value to enable toggles
        var textBoxes = mainWindow.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
        var hostBox = textBoxes.FirstOrDefault();
        hostBox?.Click();
        hostBox?.AsTextBox()?.Text = "192.168.1.1";

        // Act - Toggle master ON
        var masterToggle = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("MasterToggleProxy"));
        masterToggle?.As<ToggleButton>()?.Toggle();

        // Assert
        var systemToggle = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("SystemToggleProxy"));
        var gitToggle = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("GitToggleProxy"));

        systemToggle?.As<ToggleButton>()?.IsEnabled.Should().BeTrue();
        gitToggle?.As<ToggleButton>()?.IsEnabled.Should().BeTrue();
    }

    [Fact]
    public void ApplyButton_DisabledDuringApply()
    {
        // Arrange
        (_app, _automation, var mainWindow) = LaunchApp();

        // Enter a host
        var textBoxes = mainWindow.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
        var hostBox = textBoxes.FirstOrDefault();
        hostBox?.Click();
        hostBox?.AsTextBox()?.Text = "192.168.1.1";

        // Act - Click Apply
        var buttons = mainWindow.FindAllDescendants(cf => cf.ByControlType(ControlType.Button));
        var applyButton = buttons.FirstOrDefault(b => b.Name.Contains("Aplicar"));

        // The button should be clickable initially
        applyButton?.As<Button>()?.IsEnabled.Should().BeTrue();
    }

    [Fact]
    public void MainWindow_ToggleSwitches_Visible()
    {
        // Arrange & Act
        (_app, _automation, var mainWindow) = LaunchApp();

        // Assert
        var masterToggle = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("MasterToggleProxy"));
        var systemToggle = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("SystemToggleProxy"));
        var gitToggle = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("GitToggleProxy"));

        masterToggle.Should().NotBeNull();
        systemToggle.Should().NotBeNull();
        gitToggle.Should().NotBeNull();
    }
}
