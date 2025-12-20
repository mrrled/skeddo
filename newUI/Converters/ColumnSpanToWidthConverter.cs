using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace newUI.Converters;

public class ColumnSpanToWidthConverter : IValueConverter
{
    private const double BaseWidth = 150;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int span = 0;
        if (value is int s) span = s;
    
        // Если подгрупп 0, мы всё равно должны занять 1 колонку (150px)
        if (span <= 0) span = 1; 

        return span * BaseWidth;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}