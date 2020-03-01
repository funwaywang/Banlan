using System;

namespace Banlan
{
    public class GrayColor : ColorBase
    {
        public GrayColor(float value)
        {
            Value = value;
            var rgb = Convert.ToByte(Math.Max(0f, Math.Min(1f, value)) * 255);
            SetRgb(rgb, rgb, rgb);
        }

        public float Value { get; private set; }
    }
}
