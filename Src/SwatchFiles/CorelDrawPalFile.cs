using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Banlan.SwatchFiles
{
    class CorelDrawPalFile : ISwatchFile
    {
        private readonly Regex LineRegex = new Regex("\"(.*)\"(?:\\s+)(\\d+)(?:\\s+)(\\d+)(?:\\s+)(\\d+)(?:\\s+)(\\d)(?:\\s*)$", RegexOptions.Compiled);
        public static readonly CorelDrawPalFile Default = new CorelDrawPalFile();

        public string Name => "PAL";

        public string Description => "CorelDRAW Color palette file";

        public string[] Extensions => new string[] { ".pal" };

        public bool SupportLoad => true;

        public bool SupportSave => true;

        public bool CanLoad(string filename)
        {
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
            {
                return false;
            }

            using var stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            var header = new byte[1];
            if (stream.Read(header, 0, 1) == 1 && header[0] == '"')
            {
                return true;
            }

            return false;
        }

        public Swatch Load(Stream stream)
        {
            using var reader = new StreamReader(stream);
            var swatch = new Swatch();
            var line = reader.ReadLine();
            while (line != null)
            {
                if (LineRegex.Match(line) is Match match && match.Success)
                {
                    var cmyk = new int[]
                    {
                        int.Parse(match.Groups[2].Value),
                        int.Parse(match.Groups[3].Value),
                        int.Parse(match.Groups[4].Value),
                        int.Parse(match.Groups[5].Value),
                    };

                    if (cmyk.All(v => v >= 0 && v <= 100))
                    {
                        swatch.Colors.Add(new CmykColor(cmyk[0] / 100f, cmyk[1] / 100f, cmyk[2] / 100f, cmyk[3] / 100f)
                        {
                            Name = match.Groups[1].Value
                        });
                    }
                }
                line = reader.ReadLine();
            }

            return swatch;
        }

        public void Save(Swatch swatch, Stream stream)
        {
            using var writer = new StreamWriter(stream);
            var colors = swatch.Colors.Union(swatch.Categories.SelectMany(c => c.Colors));
            foreach (var color in colors)
            {
                int c, m, y, k;
                if (color is CmykColor cmyk)
                {
                    c = (int)(cmyk.C * 100);
                    m = (int)(cmyk.M * 100);
                    y = (int)(cmyk.Y * 100);
                    k = (int)(cmyk.K * 100);
                }
                else
                {
                    var value = ColorHelper.RgbToCmyk(color.R, color.G, color.B);
                    c = (int)(value.c * 100);
                    m = (int)(value.m * 100);
                    y = (int)(value.y * 100);
                    k = (int)(value.k * 100);
                }

                writer.WriteLine("\"{0}\" {1} {2} {3} {4}", color.Name, c, m, y, k);
            }

            writer.Write(new byte[] { 0x1A }); // write EOF mark
        }
    }
}
