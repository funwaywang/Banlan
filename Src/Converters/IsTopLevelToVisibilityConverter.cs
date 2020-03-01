using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Banlan
{
    public class IsTopLevelToVisibilityConverter : IValueConverter
    {
        public bool IsReversed { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is int vl)
            {
                if (IsReversed)
                {
                    return vl == 0 ? Visibility.Collapsed : Visibility.Visible;
                }
                else
                {
                    return vl == 0 ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            else if(parameter is int pl)
            {
                if (IsReversed)
                {
                    return pl == 0 ? Visibility.Collapsed : Visibility.Visible;
                }
                else
                {
                    return pl == 0 ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
