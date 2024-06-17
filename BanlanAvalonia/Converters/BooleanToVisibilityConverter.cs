using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Banlan
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public bool IsReversed { get; set; }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool vb)
            {
                if (IsReversed)
                {
                    return !vb;
                }
                else
                {
                    return vb;
                }
            }
            else if (parameter is bool pb)
            {
                if (IsReversed)
                {
                    return !pb;
                }
                else
                {
                    return pb;
                }
            }
            else
            {
                throw new InvalidCastException();
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
