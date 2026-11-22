using System.Globalization;
using System.Windows;
using System.Windows.Media;
using FluentAssertions;
using GitProxyManager;

namespace GitProxyManager.Tests.UnitTests.Converters;

public class BoolToBrushConverterTests
{
    private readonly BoolToBrushConverter _converter = new();

    [Fact]
    public void Convert_True_ReturnsBlueBrush()
    {
        // Act
        var result = _converter.Convert(true, typeof(Brush), null!, CultureInfo.InvariantCulture);

        // Assert
        result.Should().BeOfType<SolidColorBrush>();
        var brush = (SolidColorBrush)result;
        brush.Color.Should().Be(Color.FromRgb(0x89, 0xB4, 0xFA));
    }

    [Fact]
    public void Convert_False_ReturnsGrayBrush()
    {
        // Act
        var result = _converter.Convert(false, typeof(Brush), null!, CultureInfo.InvariantCulture);

        // Assert
        result.Should().BeOfType<SolidColorBrush>();
        var brush = (SolidColorBrush)result;
        brush.Color.Should().Be(Color.FromRgb(0x58, 0x5B, 0x7A));
    }

    [Fact]
    public void Convert_NonBool_ReturnsGrayBrush()
    {
        // Act
        var result = _converter.Convert("not a bool", typeof(Brush), null!, CultureInfo.InvariantCulture);

        // Assert
        result.Should().BeOfType<SolidColorBrush>();
        var brush = (SolidColorBrush)result;
        brush.Color.Should().Be(Color.FromRgb(0x58, 0x5B, 0x7A));
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        // Arrange & Act & Assert
        var act = () => _converter.ConvertBack(null!, typeof(bool), null!, CultureInfo.InvariantCulture);
        act.Should().Throw<NotImplementedException>();
    }
}
