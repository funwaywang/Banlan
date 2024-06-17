using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia.Data.Converters;

namespace Banlan
{
    public class ColorForegroundConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is ColorBase vi)
            {
                return ColorHelper.GetForeColor(vi.R, vi.G, vi.B);
            }
            else if (parameter is ColorBase pi)
            {
                return ColorHelper.GetForeColor(pi.R, pi.G, pi.B);
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
