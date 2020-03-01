using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Data;

namespace Banlan
{
    public class PathToNameConverter : IValueConverter
    {
        private const int DefaultMaxLength = 30;

        public int MaxLength { get; set; } = DefaultMaxLength;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string pv)
            {
                return StringHelper.LimitStringLength(Path.GetFileName(pv), MaxLength);
            }
            else if (parameter is string pp)
            {
                return StringHelper.LimitStringLength(Path.GetFileName(pp), MaxLength);
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static string PathToName(string path, int maxLength = DefaultMaxLength)
        {
            return StringHelper.LimitStringLength(Path.GetFileName(path), maxLength);
        }
    }
}
