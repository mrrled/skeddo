using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace newUI.Converters;

public class ObjectToBooleanConverter : IValueConverter
{
    public static ObjectToBooleanConverter Instance { get; } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}