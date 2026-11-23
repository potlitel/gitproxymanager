using System.Globalization;
using FluentAssertions;
using GitProxyManager;

namespace GitProxyManager.Tests.UnitTests.Converters;

public class StringToVisibilityConverterTests
{
    private readonly StringToVisibilityConverter _converter = new();

    [Fact]
    public void Convert_Null_ReturnsCollapsed()
    {
        // Act
        var result = _converter.Convert(null!, typeof(bool), null!, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(false);
    }

    [Fact]
    public void Convert_EmptyString_ReturnsCollapsed()
    {
        // Act
        var result = _converter.Convert(string.Empty, typeof(bool), null!, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(false);
    }

    [Fact]
    public void Convert_WhiteSpace_ReturnsCollapsed()
    {
        // Act
        var result = _converter.Convert("   ", typeof(bool), null!, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(false);
    }

    [Fact]
    public void Convert_HasContent_ReturnsVisible()
    {
        // Act
        var result = _converter.Convert("some text", typeof(bool), null!, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(true);
    }

    [Fact]
    public void Convert_NonString_ReturnsCollapsed()
    {
        // Act
        var result = _converter.Convert(123, typeof(bool), null!, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(false);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        // Arrange & Act & Assert
        var act = () => _converter.ConvertBack(null!, typeof(string), null!, CultureInfo.InvariantCulture);
        act.Should().Throw<NotImplementedException>();
    }
}
