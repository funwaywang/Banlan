using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace Banlan
{
    public static partial class ColorHelper
    {
        public static Brush GetForeground(byte r, byte g, byte b)
        {
            var (_, _, l) = RgbToHsl(r, g, b);
            if (l > 0.8)
            {
                return Brushes.DimGray;
            }
            else if (l < 0.4)
            {
                return Brushes.LightGray;
            }
            else
            {
                return Brushes.Black;
            }
        }

        public static Color GetForeColor(byte r, byte g, byte b)
        {
            var (_, _, l) = RgbToHsl(r, g, b);
            if (l > 0.8)
            {
                return Colors.DimGray;
            }
            else if (l < 0.4)
            {
                return Colors.GhostWhite;
            }
            else
            {
                return Colors.Black;
            }
        }
    }
}
