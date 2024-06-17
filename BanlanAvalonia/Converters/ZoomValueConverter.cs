using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Banlan
{
    public class ZoomValueConverter : AvaloniaObject, IValueConverter
    {
        // public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(nameof(Zoom), typeof(double), typeof(ZoomValueConverter), new PropertyMetadata(1.0));
        public static readonly DirectProperty<ZoomValueConverter, double?> ZoomProperty = AvaloniaProperty.RegisterDirect<ZoomValueConverter, double?>(
            nameof(Zoom),
            o => o.Zoom,
            (o, v) => o.Zoom = v);

        public double? Zoom
        {
            get => (double?)GetValue(ZoomProperty);
            set => SetValue(ZoomProperty, value);
        }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return System.Convert.ToDouble(value) * (Zoom ?? 1.0);
            }
            else if (parameter != null)
            {
                return System.Convert.ToDouble(parameter) * (Zoom ?? 1.0);
            }

            throw new InvalidCastException();
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var zoom = (Zoom == null || Zoom == 0.0) ? 1.0 : Zoom;
            if (value != null)
            {
                return System.Convert.ToDouble(value) / zoom;
            }
            else if (parameter != null)
            {
                return System.Convert.ToDouble(parameter) / zoom;
            }

            return new BindingNotification(new InvalidCastException("Invalid source value."), BindingErrorType.Error);
        }
    }
}
