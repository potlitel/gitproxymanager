using System.Globalization;
using FluentAssertions;
using GitProxyManager;

namespace GitProxyManager.Tests.UnitTests.Converters;

public class InvertBoolConverterTests
{
    private readonly InvertBoolConverter _converter = InvertBoolConverter.Instance;

    [Fact]
    public void Convert_True_ReturnsFalse()
    {
        // Act
        var result = _converter.Convert(true, typeof(bool), null!, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(false);
    }

    [Fact]
    public void Convert_False_ReturnsTrue()
    {
        // Act
        var result = _converter.Convert(false, typeof(bool), null!, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(true);
    }

    [Fact]
    public void Convert_NonBool_ReturnsSameValue()
    {
        // Arrange
        var input = "not a bool";

        // Act
        var result = _converter.Convert(input, typeof(object), null!, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(input);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        // Arrange & Act & Assert
        var act = () => _converter.ConvertBack(null!, typeof(bool), null!, CultureInfo.InvariantCulture);
        act.Should().Throw<NotImplementedException>();
    }
}
