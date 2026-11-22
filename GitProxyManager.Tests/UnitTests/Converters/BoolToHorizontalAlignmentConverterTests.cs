using System.Globalization;
using System.Windows;
using FluentAssertions;
using GitProxyManager;

namespace GitProxyManager.Tests.UnitTests.Converters;

public class BoolToHorizontalAlignmentConverterTests
{
    private readonly BoolToHorizontalAlignmentConverter _converter = BoolToHorizontalAlignmentConverter.Instance;

    [Fact]
    public void Convert_True_ReturnsRight()
    {
        // Act
        var result = _converter.Convert(true, typeof(HorizontalAlignment), null!, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(HorizontalAlignment.Right);
    }

    [Fact]
    public void Convert_False_ReturnsLeft()
    {
        // Act
        var result = _converter.Convert(false, typeof(HorizontalAlignment), null!, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(HorizontalAlignment.Left);
    }

    [Fact]
    public void Convert_NonBool_ReturnsLeft()
    {
        // Act
        var result = _converter.Convert("not a bool", typeof(HorizontalAlignment), null!, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(HorizontalAlignment.Left);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        // Arrange & Act & Assert
        var act = () => _converter.ConvertBack(null!, typeof(bool), null!, CultureInfo.InvariantCulture);
        act.Should().Throw<NotImplementedException>();
    }
}
