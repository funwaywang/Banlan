using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using Avalonia.Data.Converters;

namespace Banlan
{
    public class ListEmptyToVisibilityConverter : IValueConverter
    {
        public bool IsReversed { get; set; }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is IEnumerable v)
            {
                if (IsReversed)
                {
                    return !v.GetEnumerator().MoveNext();
                }
                else
                {
                    return v.GetEnumerator().MoveNext();
                }
            }
            else if (parameter is IEnumerable p)
            {
                if (IsReversed)
                {
                    return !p.GetEnumerator().MoveNext();
                }
                else
                {
                    return p.GetEnumerator().MoveNext();
                }
            }
            else
            {
                return IsReversed;
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
