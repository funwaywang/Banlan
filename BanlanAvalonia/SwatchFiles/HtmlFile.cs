using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Banlan.SwatchFiles
{
    class HtmlFile : ISwatchFile
    {
        private readonly Regex RgbaRegex = new Regex(@"rgba(?: )*\((?: )*(\d+)(?: )*,(?: )*(\d+)(?: )*,(?: )*(\d+)(?: )*,(?: )*(\d+)(?: )*\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex RgbRegex = new Regex(@"rgb(?: )*\((?: )*(\d+)(?: )*,(?: )*(\d+)(?: )*,(?: )*(\d+)(?: )*\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex HexRegex = new Regex(@"#([0-9a-fA-F]{3,6})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static readonly HtmlFile Default = new HtmlFile();

        public string Name => "HTML";

        public string Description => "Html Page/URL";

        public string[] Extensions => new string[] { ".html", ".htm", ".css", ".less", ".sass" };

        public bool SupportLoad => true;

        public bool SupportSave => false;

        public bool CanLoad(string filename)
        {
            return true;
        }

        public Swatch Load(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var line = reader.ReadLine();
                var list = new HashSet<int>();
                var swatch = new Swatch();
                while (line != null)
                {
                    foreach (Match match in RgbaRegex.Matches(line))
                    {
                        var rgba = new int[]
                        {
                        int.Parse(match.Groups[1].Value),
                        int.Parse(match.Groups[2].Value),
                        int.Parse(match.Groups[3].Value),
                        int.Parse(match.Groups[4].Value)
                        };
                        if (rgba.All(b => b >= 0 && b <= 255))
                        {
                            var value = rgba[0] << 16 | rgba[1] << 8 | rgba[2];
                            if (!list.Contains(value))
                            {
                                list.Add(value);
                                swatch.Colors.Add(new RgbColor((byte)rgba[0], (byte)rgba[1], (byte)rgba[2]));
                            }
                        }
                    }

                    foreach (Match match in RgbRegex.Matches(line))
                    {
                        var rgb = new int[]
                        {
                        int.Parse(match.Groups[1].Value),
                        int.Parse(match.Groups[2].Value),
                        int.Parse(match.Groups[3].Value)
                        };
                        if (rgb.All(b => b >= 0 && b <= 255))
                        {
                            var value = rgb[0] << 16 | rgb[1] << 8 | rgb[2];
                            if (!list.Contains(value))
                            {
                                list.Add(value);
                                swatch.Colors.Add(new RgbColor((byte)rgb[0], (byte)rgb[1], (byte)rgb[2]));
                            }
                        }
                    }

                    foreach (Match match in HexRegex.Matches(line))
                    {
                        byte[]? rgb = null;
                        if (match.Groups[1].Value.Length == 3)
                        {
                            rgb = new byte[]
                            {
                                Convert.ToByte(match.Groups[1].Value.Substring(0,1), 16),
                                Convert.ToByte(match.Groups[1].Value.Substring(1,1), 16),
                                Convert.ToByte(match.Groups[1].Value.Substring(2,1), 16),
                            };
                        }
                        else if (match.Groups[1].Value.Length == 6)
                        {
                            rgb = new byte[]
                            {
                                Convert.ToByte(match.Groups[1].Value.Substring(0,2), 16),
                                Convert.ToByte(match.Groups[1].Value.Substring(2,2), 16),
                                Convert.ToByte(match.Groups[1].Value.Substring(4,2), 16),
                            };
                        }

                        if (rgb != null)
                        {
                            var value = rgb[0] << 16 | rgb[1] << 8 | rgb[2];
                            if (!list.Contains(value))
                            {
                                list.Add(value);
                                swatch.Colors.Add(new RgbColor(rgb[0], rgb[1], rgb[2]));
                            }
                        }
                    }

                    line = reader.ReadLine();
                }

                return swatch;
            }
        }

        public void Save(Swatch swatch, Stream stream)
        {
            throw new NotSupportedException();
        }
    }
}
