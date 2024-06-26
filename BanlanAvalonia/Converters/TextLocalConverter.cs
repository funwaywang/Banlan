﻿using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Banlan
{
    public class TextLocalConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (parameter is string p)
            {
                return Lang.GetText(p);
            }
            else if (value is string v)
            {
                return Lang.GetText(v);
            }
            else if (parameter != null)
            {
                return Lang.GetText(parameter.ToString());
            }
            else if (value != null)
            {
                return Lang.GetText(value.ToString());
            }

            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
