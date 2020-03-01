using Colourful;
using Colourful.Conversion;
using System;
using System.Collections.Generic;
using System.Text;

namespace Banlan
{
    public class LabColor : ColorBase
    {
        public LabColor(float l, float a, float b)
        {
            L = l;
            this.a = a;
            this.b = b;

            var converter = new ColourfulConverter { TargetLabWhitePoint = Illuminants.D65 };
            var rgb = converter.ToRGB(new Colourful.LabColor(l, a / 128 * 100, b / 128 * 100));
            SetRgb(Convert.ToByte(rgb.R * 255), Convert.ToByte(rgb.G * 255), Convert.ToByte(rgb.B * 255));
        }

        public float L { get; set; }

        public float a { get; set; }

        public float b { get; set; }
    }
}
