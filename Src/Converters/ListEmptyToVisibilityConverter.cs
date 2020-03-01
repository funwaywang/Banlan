using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Banlan
{
    public class ListEmptyToVisibilityConverter : IValueConverter
    {
        public bool IsReversed { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable v)
            {
                if (IsReversed)
                {
                    return v.GetEnumerator().MoveNext() ? Visibility.Collapsed : Visibility.Visible;
                }
                else
                {
                    return v.GetEnumerator().MoveNext() ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            else if (parameter is IEnumerable p)
            {
                if (IsReversed)
                {
                    return p.GetEnumerator().MoveNext() ? Visibility.Collapsed : Visibility.Visible;
                }
                else
                {
                    return p.GetEnumerator().MoveNext() ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            else
            {
                return IsReversed ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
