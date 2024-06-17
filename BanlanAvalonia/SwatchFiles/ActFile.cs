using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Banlan.SwatchFiles
{
    class ActFile : ISwatchFile
    {
        public static readonly ActFile Default = new ActFile();

        public string Name => "ACT";

        public string Description => "Adobe Color Table file";

        public string[] Extensions => new string[] { ".act" };

        public bool SupportLoad => true;

        public bool SupportSave => true;

        public bool CanLoad(string filename)
        {
            return true;
        }

        public Swatch Load(Stream stream)
        {
            var swatch = new Swatch();
            if (stream.Length == 768)
            {
                var buffer = new byte[768];
                stream.Read(buffer, 0, buffer.Length);

                byte r = 0, g = 0, b = 0;
                for (int i = 0; i < buffer.Length; i += 3)
                {
                    if (i > 0 && buffer[i + 0] == r && buffer[i + 1] == g && buffer[i + 2] == b)
                    {
                        break;
                    }

                    r = buffer[i + 0];
                    g = buffer[i + 1];
                    b = buffer[i + 2];

                    swatch.Colors.Add(new RgbColor(buffer[i + 0], buffer[i + 1], buffer[i + 2]));
                }
            }
            else if (stream.Length == 772)
            {
                var buffer = new byte[772];
                stream.Read(buffer, 0, buffer.Length);

                var size = Math.Min(256, buffer[768] << 8 | buffer[769]);
                var transparency = buffer[770] << 8 | buffer[771];
                var p = 0;
                for (int i = 0; i < size; i++)
                {
                    swatch.Colors.Add(new RgbColor(buffer[p + 0], buffer[p + 1], buffer[p + 2]));
                    p += 3;
                }

                if (transparency > -1 && transparency < swatch.Colors.Count)
                {
                    swatch.Colors[transparency] = new RgbColor(0, 0, 0);
                }
            }
            else
            {
                throw new Exception("Invalid File Format.");
            }

            return swatch;
        }

        public void Save(Swatch swatch, Stream stream)
        {
            if (swatch == null || stream == null)
            {
                throw new ArgumentNullException();
            }

            var colors = swatch.Colors.Union(swatch.Categories.SelectMany(c => c.Colors)).Take(256);
            var count = 0;
            var transparency = -1;
            foreach (var c in colors)
            {
                var buffer = new byte[] { c.R, c.G, c.B };
                stream.Write(buffer, 0, buffer.Length);
                //if(c.A == 0)
                //{
                //    transparency = count;
                //}
                count++;
            }

            if (count < 256)
            {
                var blank = new byte[(256 - count) * 3];
                for (int i = 0; i < blank.Length; i++)
                {
                    blank[i] = 0xff;
                }
                stream.Write(blank, 0, blank.Length);
            }

            stream.Write(new byte[] { (byte)((count & 0xFF00) >> 8), (byte)(count & 0xFF) }, 0, 2);

            if (transparency > -1 && transparency < count)
            {
                stream.Write(new byte[] { (byte)((transparency & 0xFF00) >> 8), (byte)(transparency & 0xFF) }, 0, 2);
            }
            else
            {
                stream.Write(new byte[] { 0xff, 0xff }, 0, 2);
            }
        }
    }
}
