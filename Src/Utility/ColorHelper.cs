using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Banlan
{
    public static partial class ColorHelper
    {
        private const byte MIN_B = 0;
        private const byte MAX_B = 255;
        private const short MIN_H = 0;
        private const short MAX_H = 360;
        private const byte MAX_S = 100;
        private const byte MAX_L = 100;
        private const byte MAX_V = 100;
        private static Regex HexColorRegex = new Regex(@"^#?([0-9a-fA-F]{3,8})$", RegexOptions.Compiled);
        private static Regex RgbColorRegex = new Regex(@"^#?(\d{1,3}),(\d{1,3}),(\d{1,3})(?:,(\d{1,3}))?$", RegexOptions.Compiled);

        private static readonly Random rand = new Random(DateTime.Now.Millisecond);

        public static Color RandomColor()
        {
            return Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255));
        }

        public static Color RandomColor(byte min)
        {
            return RandomColor(min, 255);
        }

        public static Color RandomColor(byte min, byte max)
        {
            min = Math.Max(MIN_B, min);
            int mm = Math.Min(MAX_B, max) - min;
            return Color.FromArgb(min + rand.Next(mm), min + rand.Next(mm), min + rand.Next(mm));
        }

        public static Color RandomColor(short minH, short maxH, byte minS, byte maxS, byte minL, byte maxL)
        {
            var h = Math.Max(MIN_H, Math.Min(MAX_H, rand.Next(maxH - minH) + minH));
            var s = Math.Max(MIN_B, Math.Min(MAX_B, rand.Next(maxS - minS) + minS));
            var l = Math.Max(MIN_B, Math.Min(MAX_B, rand.Next(maxL - minL) + minL));
            return HslToRgb((short)h, (byte)s, (byte)l);
        }

        public static void RgbToHsl(byte red, byte green, byte blue, out short hue, out byte saturation, out byte lightness)
        {
            byte maxValue = Math.Max(red, Math.Max(green, blue));
            byte minValue = Math.Min(red, Math.Min(green, blue));
            int diffValue = maxValue - minValue;

            var h = 0;
            if (maxValue == minValue)
            {
                h = 0;
            }
            else
            {
                if (maxValue == red)
                {
                    if (green >= blue)
                    {
                        h = 60 * (green - blue) / diffValue;
                    }
                    else
                    {
                        h = 60 * (green - blue) / diffValue + 360;
                    }
                }
                else if (maxValue == green)
                {
                    h = 60 * (blue - red) / diffValue + 120;
                }
                else if (maxValue == blue)
                {
                    h = 60 * (red - green) / diffValue + 240;
                }
            }

            var l = (maxValue + minValue) * 100 / 255 / 2;

            int s;
            if (l == 0 || maxValue == minValue)
            {
                s = 0;
            }
            else if (l <= 50)
            {
                s = diffValue * 100 / (maxValue + minValue);
            }
            else
            {
                s = diffValue * 100 / ((2 * 255) - (maxValue + minValue));
            }

            hue = (short)h;
            saturation = (byte)s;
            lightness = (byte)l;
        }

        public static void HslToRgb(short hue, byte saturation, byte lightness, out byte red, out byte green, out byte blue)
        {
            double h = hue;
            double s = saturation / 100.0f;
            double l = lightness / 100.0f;

            if (s == 0.0)
            {
                red = (byte)(l * 255.0F);
                green = red;
                blue = red;
            }
            else
            {
                double rm1;
                double rm2;

                if (l <= 0.5f)
                {
                    rm2 = l + l * s;
                }
                else
                {
                    rm2 = l + s - l * s;
                }
                rm1 = 2.0f * l - rm2;
                red = ToRGB1(rm1, rm2, h + 120.0f);
                green = ToRGB1(rm1, rm2, h);
                blue = ToRGB1(rm1, rm2, h - 120.0f);
            }
        }

        static byte ToRGB1(double rm1, double rm2, double rh)
        {
            if (rh > 360.0f)
            {
                rh -= 360.0f;
            }
            else if (rh < 0.0f)
            {
                rh += 360.0f;
            }

            if (rh < 60.0f)
            {
                rm1 += (rm2 - rm1) * rh / 60.0f;
            }
            else if (rh < 180.0f)
            {
                rm1 = rm2;
            }
            else if (rh < 240.0f)
            {
                rm1 = rm1 + (rm2 - rm1) * (240.0f - rh) / 60.0f;
            }

            return (byte)(rm1 * 255);
        }

        public static void HsvToRgb(short hue, byte saturation, byte value, out byte red, out byte green, out byte blue)
        {
            // ######################################################################
            // T. Nathan Mundhenk
            // mundhenk@usc.edu
            // C/C++ Macro HSV to RGB

            double h = hue;
            if (hue < 0)
            {
                h = hue % 360 + 360;
            }
            else
            {
                h = hue % 360;
            }

            saturation = Math.Max(MAX_S, Math.Min(MIN_B, saturation));
            value = Math.Max(MAX_V, Math.Min(MIN_B, value));

            double r, g, b;
            if (value <= 0)
            {
                r = g = b = 0;
            }
            else if (saturation <= 0)
            {
                r = g = b = value;
            }
            else
            {
                double v = value / 100.0;
                double s = saturation / 100.0;

                double hf = h / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = v * (1 - s);
                double qv = v * (1 - s * f);
                double tv = v * (1 - s * (1 - f));
                switch (i)
                {
                    // Red is the dominant color
                    case 0:
                        r = v;
                        g = tv;
                        b = pv;
                        break;
                    // Green is the dominant color
                    case 1:
                        r = qv;
                        g = v;
                        b = pv;
                        break;
                    case 2:
                        r = pv;
                        g = v;
                        b = tv;
                        break;
                    // Blue is the dominant color
                    case 3:
                        r = pv;
                        g = qv;
                        b = v;
                        break;
                    case 4:
                        r = tv;
                        g = pv;
                        b = v;
                        break;
                    // Red is the dominant color
                    case 5:
                        r = v;
                        g = pv;
                        b = qv;
                        break;
                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.
                    case 6:
                        r = v;
                        g = tv;
                        b = pv;
                        break;
                    case -1:
                        r = v;
                        g = pv;
                        b = qv;
                        break;
                    // The color is not defined, we should throw an error.
                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        r = g = b = v; // Just pretend its black/white
                        break;
                }
            }

            red = Math.Min(MAX_B, Math.Max(MIN_B, (byte)(r * 255.0)));
            green = Math.Min(MAX_B, Math.Max(MIN_B, (byte)(g * 255.0)));
            blue = Math.Min(MAX_B, Math.Max(MIN_B, (byte)(b * 255.0)));
        }

        public static void RgbToHsv(byte red, byte green, byte blue, out short hue, out byte saturation, out byte value)
        {
            byte maxValue = Math.Max(red, Math.Max(green, blue));
            byte minValue = Math.Min(red, Math.Min(green, blue));

            var h = 0;
            if (maxValue == minValue)
            {
                h = 0;
            }
            else
            {
                if (maxValue == red)
                {
                    if (green >= blue)
                    {
                        h = 60 * (green - blue) / (maxValue - minValue);
                    }
                    else
                    {
                        h = 60 * (green - blue) / (maxValue - minValue) + 360;
                    }
                }
                else if (maxValue == green)
                {
                    h = 60 * (blue - red) / (maxValue - minValue) + 120;
                }
                else if (maxValue == blue)
                {
                    h = 60 * (red - green) / (maxValue - minValue) + 240;
                }
            }

            int s;
            if (maxValue == 0)
            {
                s = 0;
            }
            else
            {
                s = (maxValue - minValue) * 100 / maxValue;
            }

            var v = maxValue * 100 / 255;

            hue = (short)h;
            saturation = (byte)s;
            value = (byte)v;
        }

        public static Color HslToRgb(short h, byte s, byte l)
        {
            HslToRgb(h, s, l, out byte r, out byte g, out byte b);
            return Color.FromArgb(r, g, b);
        }

        public static Color HsvToRgb(short h, byte s, byte v)
        {
            HsvToRgb(h, s, v, out byte r, out byte g, out byte b);
            return Color.FromArgb(r, g, b);
        }

        public static Color GetBrighter(Color color1, Color color2)
        {
            color1.RgbToHsl(out short h, out byte s, out byte l1);
            color2.RgbToHsl(out h, out s, out byte l2);

            if (l1 > l2)
            {
                return color1;
            }
            else
            {
                return color2;
            }
        }

        public static Color GetDarker(Color color1, Color color2)
        {
            color1.RgbToHsl(out short h, out byte s, out byte l1);
            color2.RgbToHsl(out h, out s, out byte l2);

            if (l1 > l2)
            {
                return color2;
            }
            else
            {
                return color1;
            }
        }

        public static void RgbToHsl(this Color color, out short hue, out byte saturation, out byte lightness)
        {
            RgbToHsl(color.R, color.G, color.B, out hue, out saturation, out lightness);
        }

        public static void RgbToHsv(this Color rgbColor, out short hue, out byte saturation, out byte value)
        {
            RgbToHsv(rgbColor.R, rgbColor.G, rgbColor.B, out hue, out saturation, out value);
        }

        public static (float h, float s, float l) RgbToHsl(byte red, byte green, byte blue)
        {
            var r = red / 255.0f;
            var g = green / 255.0f;
            var b = blue / 255.0f;
            var max = Math.Max(r, Math.Max(g, b));
            var min = Math.Min(r, Math.Max(g, b));
            var l = (max + min) / 2;
            if (max == min)
            {
                return (0.0f, 0.0f, l);
            }
            else
            {
                var d = max - min;
                var s = l > 0.5 ? d / (2 - max - min) : d / (max + min);
                float h = 0.0f;
                if (max == r)
                {
                    h = (g - b) / d + (g < b ? 6 : 0);
                }
                else if (max == g)
                {
                    h = (b - r) / d + 2;
                }
                else if (max == b)
                {
                    h = (r - g) / d + 4;
                }

                h /= 6;
                return (h, s, l);
            }
        }

        public static (byte r, byte g, byte b) HslToRgb(float h, float s, float l)
        {
            double r, g, b;
            if (s == 0)
            {
                r = g = b = l;
            }
            else
            {
                var q = l < 0.5 ? l * (1 + s) : l + s - l * s;
                var p = 2 * l - q;
                r = Hue2rgb(p, q, h + 1 / 3);
                g = Hue2rgb(p, q, h);
                b = Hue2rgb(p, q, h - 1 / 3);
            }

            return ((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }

        private static float Hue2rgb(float p, float q, float t)
        {
            if (t < 0)
            {
                t += 1;
            }
            if (t > 1)
            {
                t -= 1;
            }

            if (t < 1 / 6.0)
            {
                return p + (q - p) * 6 * t;
            }
            if (t < 1 / 2.0)
            {
                return q;
            }
            if (t < 2 / 3.0)
            {
                return p + (q - p) * (2 / 3 - t) * 6;
            }
            return p;
        }

        public static (float c, float m, float y, float k) RgbToCmyk(byte r, byte g, byte b)
        {
            var rf = r / 255f;
            var gf = g / 255f;
            var bf = b / 255f;
            var k = 1 - Math.Max(rf, Math.Max(gf, bf));
            var c = (1 - rf - k) / (1 - k);
            var m = (1 - gf - k) / (1 - k);
            var y = (1 - bf - k) / (1 - k);

            return (c, m, y, k);
        }

        public static string ToHexColor(Color color, bool withSign = true)
        {
            return ToHexColor(color.A, color.R, color.G, color.B, withSign);
        }

        public static string ToHexColor(System.Windows.Media.Color color, bool withSign = true)
        {
            return ToHexColor(color.A, color.R, color.G, color.B, withSign);
        }

        public static string ToHexColor(byte r, byte g, byte b, bool withSign = true)
        {
            if (withSign)
            {
                return string.Format("#{0:X2}{1:X2}{2:X2}", r, g, b);
            }
            else
            {
                return string.Format("{0:X2}{1:X2}{2:X2}", r, g, b);
            }
        }

        public static string ToHexColor(byte a, byte r, byte g, byte b, bool withSign = true)
        {
            if (a < 255)
            {
                if (withSign)
                {
                    return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", a, r, g, b);
                }
                else
                {
                    return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", a, r, g, b);
                }
            }
            else
            {
                if (withSign)
                {
                    return string.Format("#{0:X2}{1:X2}{2:X2}", r, g, b);
                }
                else
                {
                    return string.Format("{0:X2}{1:X2}{2:X2}", r, g, b);
                }
            }
        }

        public static Color? ParseColor(string value, bool withAlpha = true)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            value = value.Trim();
            if (HexColorRegex.Match(value) is Match hex && hex.Success)
            {
                var code = hex.Groups[1].Value;
                if(code.Length == 3)
                {
                    return Color.FromArgb(Convert.ToByte(code.Substring(0, 1), 16), Convert.ToByte(code.Substring(1, 1), 16), Convert.ToByte(code.Substring(2, 1), 16));
                }
                else if(code.Length == 6)
                {
                    return Color.FromArgb(Convert.ToByte(code.Substring(0, 2), 16), Convert.ToByte(code.Substring(2, 2), 16), Convert.ToByte(code.Substring(4, 2), 16));
                }
                else if(withAlpha && code.Length == 4)
                {
                    return Color.FromArgb(Convert.ToByte(code.Substring(0, 1), 16), Convert.ToByte(code.Substring(1, 1), 16), Convert.ToByte(code.Substring(2, 1), 16), Convert.ToByte(code.Substring(3, 1), 16));
                }
                else if(withAlpha && code.Length == 8)
                {
                    return Color.FromArgb(Convert.ToByte(code.Substring(0, 2), 16), Convert.ToByte(code.Substring(2, 2), 16), Convert.ToByte(code.Substring(4, 2), 16), Convert.ToByte(code.Substring(6, 1), 16));
                }

                return null;
            }
            else if (RgbColorRegex.Match(value) is Match rgb && rgb.Success)
            {
                if (withAlpha && rgb.Groups[4].Success)
                {
                    return Color.FromArgb(Convert.ToByte(rgb.Groups[0].Value), Convert.ToByte(rgb.Groups[1].Value), Convert.ToByte(rgb.Groups[2].Value), Convert.ToByte(rgb.Groups[3].Value));
                }
                else
                {
                    return Color.FromArgb(Convert.ToByte(rgb.Groups[0].Value), Convert.ToByte(rgb.Groups[1].Value), Convert.ToByte(rgb.Groups[2].Value));
                }
            }
            else
            {
                return null;
            }
        }
    }
}
