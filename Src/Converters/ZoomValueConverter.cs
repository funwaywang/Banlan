using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Banlan
{
    public class ZoomValueConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(nameof(Zoom), typeof(double), typeof(ZoomValueConverter), new PropertyMetadata(1.0));

        public double Zoom
        {
            get => (double)GetValue(ZoomProperty);
            set => SetValue(ZoomProperty, value);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if(e.Property == ZoomProperty)
            {
                Console.WriteLine(e.NewValue);
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return System.Convert.ToDouble(value) * Zoom;
            }
            else if (parameter != null)
            {
                return System.Convert.ToDouble(parameter) * Zoom;
            }

            throw new InvalidCastException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var zoom = Zoom == 0.0 ? 1.0 : Zoom;
            if (value != null)
            {
                return System.Convert.ToDouble(value) / zoom;
            }
            else if (parameter != null)
            {
                return System.Convert.ToDouble(parameter) / zoom;
            }

            throw new InvalidCastException();
        }
    }
}
