using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace GitProxyManager;

public class BoolToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var isChecked = value is bool b && b;
        return new SolidColorBrush(isChecked ? Color.FromRgb(0x89, 0xB4, 0xFA) : Color.FromRgb(0x58, 0x5B, 0x7A));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class BoolToHorizontalAlignmentConverter : IValueConverter
{
    public static readonly BoolToHorizontalAlignmentConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var isChecked = value is bool b && b;
        return isChecked ? HorizontalAlignment.Right : HorizontalAlignment.Left;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str)
            return string.IsNullOrWhiteSpace(str) ? Visibility.Collapsed : Visibility.Visible;
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}