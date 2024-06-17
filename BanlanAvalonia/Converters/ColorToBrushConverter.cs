using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Banlan
{
    public class ColorToBrushConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is ColorBase vi)
            {
                return new SolidColorBrush(Color.FromRgb(vi.R, vi.G, vi.B));
            }
            else if (parameter is ColorBase pi)
            {
                return new SolidColorBrush(Color.FromRgb(pi.R, pi.G, pi.B));
            }
            else if (value is Color mc)
            {
                return new SolidColorBrush(mc);
            }
            else if (parameter is Color mp)
            {
                return new SolidColorBrush(mp);
            }
            else if (value is System.Drawing.Color vc)
            {
                return new SolidColorBrush(Color.FromArgb(vc.A, vc.R, vc.G, vc.B));
            }
            else if (parameter is System.Drawing.Color pc)
            {
                return new SolidColorBrush(Color.FromArgb(pc.A, pc.R, pc.G, pc.B));
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
