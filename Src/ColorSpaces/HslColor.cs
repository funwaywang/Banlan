using System;

namespace Banlan
{
    public class HslColor : ColorBase
    {
        public HslColor(float h, float s, float l)
        {
            H = h;
            S = s;
            L = l;
            var (r, g, b) = ColorHelper.HslToRgb(h, s, l);
            SetRgb(r, g, b);
        }

        public float H { get; private set; }

        public float S { get; private set; }

        public float L { get; private set; }
    }
}
