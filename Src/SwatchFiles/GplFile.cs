using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Banlan.SwatchFiles
{
    class GplFile : ISwatchFile
    {
        private readonly Regex ColorLineRegex = new Regex(@"^(?: *)(\d+)(?: +)(\d+)(?: +)(\d+)(?:\s*)(.*)?$", RegexOptions.Compiled | RegexOptions.Singleline);
        public static readonly GplFile Default = new GplFile();

        public string Name => "GPL";

        public string Description => "Gimp Palette file";

        public string[] Extensions => new string[] { ".gpl" };

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
                if (reader.ReadLine() != "GIMP Palette")
                {
                    throw new Exception("Invalid File Format.");
                }

                var swatch = new Swatch();
                var name = reader.ReadLine();
                if (name.StartsWith("Name:"))
                {
                    swatch.Name = name.Substring(5, name.Length - 5).Trim();
                }

                while (reader.ReadLine() != "#")
                {
                }

                var line = reader.ReadLine();
                while (line != null)
                {
                    if (ColorLineRegex.Match(line) is Match match && match.Success)
                    {
                        swatch.Colors.Add(new RgbColor(Convert.ToByte(match.Groups[1].Value), Convert.ToByte(match.Groups[2].Value), Convert.ToByte(match.Groups[3].Value))
                        {
                            Name = match.Groups[4].Value
                        });
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
                writer.WriteLine("GIMP Palette");
                writer.WriteLine($"Name: {swatch.Name}");
                writer.WriteLine("#");

                var colors = swatch.Colors.Union(swatch.Categories.SelectMany(c => c.Colors));
                foreach (var c in colors)
                {
                    writer.WriteLine("{0} {1} {2}\t{3}", c.R.ToString().PadLeft(3, ' '), c.G.ToString().PadLeft(3, ' '), c.B.ToString().PadLeft(3, ' '), c.Name);
                }
            }
        }
    }
}
