using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Banlan
{
    [ValueConversion(typeof(string), typeof(string))]
    public class TextLocalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
