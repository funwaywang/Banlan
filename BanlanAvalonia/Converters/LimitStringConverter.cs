using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia.Data.Converters;

namespace Banlan
{
    public class LimitStringConverter : IValueConverter
    {
        private const int DefaultMaxLength = 30;

        public LimitStringConverter()
        {
        }

        public LimitStringConverter(int maxLength)
        {
            MaxLength = maxLength;
        }

        public int MaxLength { get; set; } = DefaultMaxLength;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string pv)
            {
                return StringHelper.LimitStringLength(pv, MaxLength);
            }
            else if (parameter is string pp)
            {
                return StringHelper.LimitStringLength(pp, MaxLength);
            }
            else
            {
                return null;
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
