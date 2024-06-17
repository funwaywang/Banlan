using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia.Data.Converters;

namespace Banlan
{
    public class StringIsEmptyConverter : IValueConverter
    {
        public bool IsReversed { get; set; }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var flag = (value is string vs && string.IsNullOrEmpty(vs))
                || (parameter is string ps && string.IsNullOrEmpty(ps))
                || (value == null && parameter == null);

            if (IsReversed)
            {
                return !flag;
            }
            else
            {
                return flag;
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
