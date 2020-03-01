using System;

namespace Banlan
{
    public class CmykColor : ColorBase
    {
        public CmykColor(float c, float m, float y, float k)
        {
            C = c;
            M = m;
            Y = y;
            K = k;

            SetRgb(Convert.ToByte(255 * (1 - c) * (1 - k)),
                Convert.ToByte(255 * (1 - m) * (1 - k)),
                Convert.ToByte(255 * (1 - y) * (1 - k)));
        }

        public float C { get; private set; }

        public float M { get; private set; }

        public float Y { get; private set; }

        public float K { get; private set; }
    }
}
