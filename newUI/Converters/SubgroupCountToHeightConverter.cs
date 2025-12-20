using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace newUI.Converters;

public class SubgroupCountToHeightConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Если количество подгрупп 0 или null — возвращаем высоту 60 (на две строки)
        // Если есть подгруппы — возвращаем 30 (только верхняя строка)
        if (value is int count && count > 0)
            return 30.0;

        return 60.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}