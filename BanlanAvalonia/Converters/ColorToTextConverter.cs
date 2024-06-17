using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Banlan
{
    public class ColorToTextConverter : AvaloniaObject, IValueConverter
    {
        // public static readonly DependencyProperty FormatterProperty = DependencyProperty.Register(nameof(Formatter), typeof(IColorTextFormatter), typeof(ColorToTextConverter));

        public static readonly DirectProperty<ColorToTextConverter, IColorTextFormatter?> FormatterProperty = AvaloniaProperty.RegisterDirect<ColorToTextConverter, IColorTextFormatter?>(
            nameof(Formatter),
            o => o.Formatter,
            (o, v) => o.Formatter = v);

        public IColorTextFormatter? Formatter
        {
            get => GetValue(FormatterProperty);
            set => SetValue(FormatterProperty, value);
        }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is ColorBase vi)
            {
                return Formatter?.Format(vi) ?? vi.ToString();
            }
            else if (parameter is ColorBase pi)
            {
                return Formatter?.Format(pi) ?? pi.ToString();
            }
            else if (value is Avalonia.Media.Color mc)
            {
                return Formatter?.Format(mc.R, mc.G, mc.B) ?? ColorHelper.ToHexColor(mc);
            }
            else if (parameter is Avalonia.Media.Color mp)
            {
                return Formatter?.Format(mp.R, mp.G, mp.B) ?? ColorHelper.ToHexColor(mp);
            }
            else if (value is System.Drawing.Color vc)
            {
                return Formatter?.Format(vc.R, vc.G, vc.B) ?? ColorHelper.ToHexColor(vc);
            }
            else if (parameter is System.Drawing.Color pc)
            {
                return Formatter?.Format(pc.R, pc.G, pc.B) ?? ColorHelper.ToHexColor(pc);
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
