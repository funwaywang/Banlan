using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using Avalonia.Data.Converters;

namespace Banlan
{
    public class IsTypeToVisibilityConverter : IValueConverter
    {
        public bool IsReversed { get; set; }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if(value != null && parameter is string typeName)
            {
                if (IsReversed)
                {
                    return value.GetType().Name != typeName;
                }
                else
                {
                    return value.GetType().Name == typeName;
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
