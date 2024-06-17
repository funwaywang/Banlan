using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Banlan
{
    public interface IColorTextFormatter
    {
        string Name { get; }

        string Format(ColorBase color);

        string Format(byte r, byte g, byte b);
    }

    internal class RgbColorTextFormater : IColorTextFormatter
    {
        public string Name => "RGB";

        public string Format(ColorBase color)
        {
            return $"{color.R}, {color.G}, {color.B}";
        }

        public string Format(byte r, byte g, byte b)
        {
            return $"{r}, {g}, {b}";
        }
    }

    internal class CssRgbColorTextFormater : IColorTextFormatter
    {
        public string Name => "RGB(Css)";

        public string Format(ColorBase color)
        {
            return $"rgb({color.R},{color.G},{color.B})";
        }

        public string Format(byte r, byte g, byte b)
        {
            return $"rgb({r},{g},{b})";
        }
    }

    internal class HexColorTextFormater : IColorTextFormatter
    {
        public string Name => "#Hex";

        public string Format(ColorBase color)
        {
            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        public string Format(byte r, byte g, byte b)
        {
            return "#" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
        }
    }

    internal class Hex2ColorTextFormater : IColorTextFormatter
    {
        public string Name => "Hex";

        public string Format(ColorBase color)
        {
            return color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        public string Format(byte r, byte g, byte b)
        {
            return r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
        }
    }
}
