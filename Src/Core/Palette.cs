using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Banlan.Core
{
    public struct Palette
    {
        public Palette(float[] mean, int count, int position)
        {
            Mean = mean;
            Count = count;
            Position = position;
        }

        public float[] Mean { get; set; }

        public int Count { get; set; }

        public int Position { get; set; }

        public override string ToString()
        {
            return $"{string.Join(',', Mean?.Select(c => c.ToString()))}${Count}";
        }

        public Color GetColor()
        {
            return Color.FromArgb((byte)Mean[0], (byte)Mean[1], (byte)Mean[2]);
        }
    }
}
