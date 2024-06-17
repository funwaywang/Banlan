using System;

namespace Banlan
{
    public class HsvColor : ColorBase
    {
        public HsvColor(float h, float s, float v)
        {
            H = h;
            S = s;
            V = v;
            var (r, g, b) = ColorHelper.HslToRgb(h, s, v);
            SetRgb(r, g, b);
        }

        public float H { get; private set; }

        public float S { get; private set; }

        public float V { get; private set; }
    }
}
