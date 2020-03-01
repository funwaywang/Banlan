using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Banlan
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public bool IsReversed { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool vb)
            {
                if (IsReversed)
                {
                    return vb ? Visibility.Collapsed : Visibility.Visible;
                }
                else
                {
                    return vb ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            else if (parameter is bool pb)
            {
                if (IsReversed)
                {
                    return pb ? Visibility.Collapsed : Visibility.Visible;
                }
                else
                {
                    return pb ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            else
            {
                throw new InvalidCastException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
