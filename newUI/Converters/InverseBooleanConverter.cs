using System;
using Avalonia.Data.Converters;
using System.Globalization;

namespace newUI.Converters
{
    public class InverseBooleanConverter : IValueConverter
    {
        public static InverseBooleanConverter Instance { get; } = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool b ? !b : value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool b ? !b : value;
        }
    }
}