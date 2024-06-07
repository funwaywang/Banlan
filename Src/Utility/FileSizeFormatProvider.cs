using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banlan
{
    public class FileSizeFormatProvider : IFormatProvider, ICustomFormatter
    {
        private const string FileSizeFormat = "fs";
        private const long OneKiloByte = 1024;
        private const long OneMegaByte = OneKiloByte * 1024;
        private const long OneGigaByte = OneMegaByte * 1024;

        public object? GetFormat(Type? formatType)
        {
            if (formatType == typeof(ICustomFormatter) || formatType == typeof(NumberFormatInfo))
            {
                return this;
            }

            return null;
        }

        public string Format(string? format, object? arg, IFormatProvider? formatProvider)
        {
            if (format == null || !format.StartsWith(FileSizeFormat))
            {
                return DefaultFormat(format, arg, formatProvider) ?? string.Empty;
            }

            if (arg is string)
            {
                return DefaultFormat(format, arg, formatProvider) ?? string.Empty;
            }

            double size;

            try
            {
                size = Convert.ToInt64(arg);
            }
            catch (InvalidCastException)
            {
                return DefaultFormat(format, arg, formatProvider) ?? string.Empty;
            }

            string suffix;
            if (size > OneGigaByte)
            {
                size /= OneGigaByte;
                suffix = "GB";
            }
            else if (size > OneMegaByte)
            {
                size /= OneMegaByte;
                suffix = "MB";
            }
            else if (size > OneKiloByte)
            {
                size /= OneKiloByte;
                suffix = "KB";
            }
            else
            {
                suffix = "B";
            }

            string precision = format.Substring(2);
            if (string.IsNullOrEmpty(precision))
            {
                precision = "2";
            }

            return string.Format("{0:N" + precision + "} {1}", size, suffix);
        }

        public static string Format(long length)
        {
            string suffix;
            double size = length;
            if (size > OneGigaByte)
            {
                size /= OneGigaByte;
                suffix = "GB";
            }
            else if (size > OneMegaByte)
            {
                size /= OneMegaByte;
                suffix = "MB";
            }
            else if (size > OneKiloByte)
            {
                size /= OneKiloByte;
                suffix = "KB";
            }
            else
            {
                suffix = "B";
            }

            string precision = "2";
            return string.Format("{0:N" + precision + "} {1}", size, suffix);
        }

        private static string? DefaultFormat(string? format, object? arg, IFormatProvider? formatProvider)
        {
            if (arg is IFormattable formattableArg)
            {
                return formattableArg.ToString(format, formatProvider);
            }

            return arg?.ToString();
        }
    }
}
