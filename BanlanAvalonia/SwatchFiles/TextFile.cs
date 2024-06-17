using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Banlan.SwatchFiles
{
    class TextFile : ISwatchFile
    {
        public static readonly TextFile Default = new TextFile();

        public string Name => "TEXT";

        public string Description => "Text file";

        public string[] Extensions => new string[] { ".txt" };

        public bool SupportLoad => true;

        public bool SupportSave => true;

        public bool CanLoad(string filename)
        {
            return true;
        }

        public Swatch Load(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var line = reader.ReadLine();
                var swatch = new Swatch();
                while (line != null)
                {
                    var color = ColorHelper.ParseColor(line);
                    if (color != null)
                    {
                        swatch.Colors.Add(new RgbColor(color.Value.R, color.Value.G, color.Value.B));
                    }

                    line = reader.ReadLine();
                }

                return swatch;
            }
        }

        public void Save(Swatch swatch, Stream stream)
        {
            using (var writer = new StreamWriter(stream))
            {
                var colors = swatch.Colors.Union(swatch.Categories.SelectMany(c => c.Colors));
                foreach (var c in colors)
                {
                    writer.WriteLine(ColorHelper.ToHexColor(c.R, c.G, c.B));
                }
            }
        }
    }
}
