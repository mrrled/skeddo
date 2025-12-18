using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace newUI.Converters;

public class ColumnSpanToWidthConverter : IValueConverter
{
    private const double BaseWidth = 150;
    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int span && span > 0)
            return span * BaseWidth;
        return BaseWidth;
    }
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}