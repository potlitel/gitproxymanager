using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Media;

namespace GitProxyManager;

public class BoolToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var isChecked = value is bool b && b;
        return new SolidColorBrush(isChecked
            ? Color.FromRgb(0x89, 0xB4, 0xFA)
            : Color.FromRgb(0x58, 0x5B, 0x7A));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class BoolToHorizontalAlignmentConverter : IValueConverter
{
    public static readonly BoolToHorizontalAlignmentConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && b ? HorizontalAlignment.Right : HorizontalAlignment.Left;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class StringToVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is string str && !string.IsNullOrWhiteSpace(str);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class BoolToVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && b;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class InvertBoolConverter : IValueConverter
{
    public static readonly InvertBoolConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b ? !b : value;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class StringToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var color = value is string hex && Color.TryParse(hex, out var parsed)
            ? parsed
            : Color.FromRgb(0x88, 0x88, 0x88);

        return new SolidColorBrush(color);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class StringToColorConverter : IValueConverter
{
    public static readonly StringToColorConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is string hex && Color.TryParse(hex, out var parsed)
            ? parsed
            : Color.FromRgb(0x88, 0x88, 0x88);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
