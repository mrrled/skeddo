using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace newUI.Converters;

public class GroupWideBackgroundConverter : IValueConverter
{
    Color color = Color.FromArgb(21, Colors.Blue.R, Colors.Blue.G, Colors.Blue.B);

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isGroupWide && isGroupWide)
            return new SolidColorBrush(color);
        return new SolidColorBrush(Colors.Transparent);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}