using System;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;

namespace PlcMonitor.UI.ValueConverters
{
    public class ArrayJoinConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Array arr)
            {
                var values = arr.Cast<object?>();
                if (parameter is IValueConverter converter)
                {
                    values = values.Select(v => converter.Convert(v, targetType, null, culture));
                }

                return string.Join(", ", values);
            }
            else
            {
                return parameter is IValueConverter converter
                    ? converter.Convert(value, targetType, null, culture)
                    : System.Convert.ToString(value) ?? string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}