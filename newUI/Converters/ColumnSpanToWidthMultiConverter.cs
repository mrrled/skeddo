using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;

namespace newUI.Converters;

public class ColumnSpanToWidthMultiConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count > 0 && values[0] is int count)
        {
            // Если подгрупп 0, кнопка занимает 1 колонку (150px)
            // Если подгрупп 2, кнопка занимает 2 колонки (300px)
            int span = count > 0 ? count : 1;
            return (double)(span * 150);
        }
        return 150.0;
    }
}