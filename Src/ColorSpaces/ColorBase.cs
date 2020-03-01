using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Banlan
{
    public abstract class ColorBase : IComparable
    {
        public string Name { get; set; }

        public byte R { get; private set; }

        public byte G { get; private set; }

        public byte B { get; private set; }

        protected void SetRgb(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public Color ToDrawingColor()
        {
            return Color.FromArgb(R, G, B);
        }

        public System.Windows.Media.Color ToMediaColor()
        {
            return System.Windows.Media.Color.FromRgb(R, G, B);
        }

        public override string ToString()
        {
            return ColorHelper.ToHexColor(R, G, B);
        }

        public int CompareTo(object obj)
        {
            if (obj is ColorBase c)
            {
                return (R << 16 | G << 8 | B).CompareTo(c.R << 16 | c.G << 8 | c.B);
            }
            else
            {
                throw new InvalidCastException();
            }
        }
    }
}
