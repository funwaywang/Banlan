using Banlan.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Banlan.SwatchFiles
{
    class AcoFile : ISwatchFile
    {
        enum ColorSpace
        {
            RGB = 0,
            HSB = 1,
            CMYK = 2,
            Pantone = 3,
            Focoltone = 4,
            Trumatch = 5,
            Toyo88 = 6,
            Lab = 7,
            Grayscale = 8,
            WideCMYK = 9,
            HKS = 10,
        };

        public static readonly AcoFile Default = new AcoFile();

        public string Name => "ACO";

        public string Description => "Adobe Color file";

        public string[] Extensions => new string[] { ".aco" };

        public bool SupportLoad => true;

        public bool SupportSave => true;

        public bool CanLoad(string filename)
        {
            return true;
        }

        public Swatch Load(Stream stream)
        {
            using (var reader = new BigEndianBinaryReader(stream))
            {
                var version = reader.ReadInt16();
                var count = reader.ReadInt16();
                var swatch = new Swatch();
                if (count * 10 + 4 < stream.Length)
                {
                    // version 2
                    stream.Seek(count * 10, SeekOrigin.Current);
                    version = reader.ReadInt16();
                    count = reader.ReadInt16();
                    if (count > 0)
                    {
                        ReadColors(reader, swatch, count, 2);
                    }
                }
                else if (count > 0) // version 1
                {
                    ReadColors(reader, swatch, count, 1);
                }

                return swatch;
            }
        }

        private void ReadColors(BinaryReader reader, Swatch swatch, int count, short version)
        {
            for (int i = 0; i < count; i++)
            {
                var space = (ColorSpace)reader.ReadInt16();
                var item1 = reader.ReadUInt16();
                var item2 = reader.ReadUInt16();
                var item3 = reader.ReadUInt16();
                var item4 = reader.ReadUInt16();

                ColorBase? color = null;
                switch (space)
                {
                    case ColorSpace.RGB:
                        color = new RgbColor(Convert.ToByte(item1 / 256), Convert.ToByte(item2 / 256), Convert.ToByte(item3 / 256));
                        break;
                    case ColorSpace.HSB:
                        color = new HsvColor(item1 / 65535f, item2 / 65535f, item3 / 65535f);
                        break;
                    case ColorSpace.Grayscale:
                        color = new GrayColor(item1 / 10000f);
                        break;
                    case ColorSpace.CMYK:
                        color = new CmykColor(1f - (item1 / 65535f), 1f - (item2 / 65535f), 1f - (item3 / 65535f), 1f - (item4 / 65535f));
                        break;
                    case ColorSpace.WideCMYK:
                        color = new CmykColor(item1 / 10000f, item2 / 10000f, item3 / 10000f, item4 / 10000f);
                        break;
                    case ColorSpace.Lab:
                        color = new LabColor((short)(item1 / 100), (short)(item2 / 100), (short)(item3 / 100));
                        break;
                    default:
                        break;
                }

                if (version == 2)
                {
                    reader.ReadBytes(2);
                    var nameLength = reader.ReadInt16() * 2;
                    if (nameLength > 0)
                    {
                        var nameBuffer = reader.ReadBytes(nameLength);
                        if (color != null)
                        {
                            color.Name = Encoding.BigEndianUnicode.GetString(nameBuffer, 0, nameLength - 2);
                        }
                    }
                }

                if (color != null)
                {
                    swatch.Colors.Add(color);
                }
            }
        }

        public void Save(Swatch swatch, Stream stream)
        {
            using (var writer = new BigEndianBinaryWriter(stream))
            {
                var colors = swatch.Colors.Union(swatch.Categories.SelectMany(c => c.Colors)).ToList();
                WriteColors(writer, colors, 1);
                WriteColors(writer, colors, 2);
            }
        }

        void WriteColors(BinaryWriter writer, List<ColorBase> colors, ushort version)
        {
            writer.Write(version);
            writer.Write((short)colors.Count);

            foreach (var c in colors)
            {
                ColorSpace space;
                ushort[] values = new ushort[4];
                if (c is RgbColor rgb)
                {
                    space = ColorSpace.RGB;
                    values[0] = (ushort)(rgb.R * 256);
                    values[1] = (ushort)(rgb.G * 256);
                    values[2] = (ushort)(rgb.B * 256);
                }
                else if (c is HsvColor hsv)
                {
                    space = ColorSpace.HSB;
                    values = new ushort[] { (ushort)(hsv.H * 65535), (ushort)(hsv.S * 65535), (ushort)(hsv.V * 65535) };
                }
                else if (c is GrayColor gray)
                {
                    space = ColorSpace.Grayscale;
                    values[0] = (ushort)(gray.Value * 10000);
                }
                else if (c is CmykColor cmyk)
                {
                    space = ColorSpace.CMYK;
                    values = new ushort[] { (ushort)((1f - cmyk.C) * 65535), (ushort)((1f - cmyk.M) * 65535), (ushort)((1f - cmyk.Y) * 65535), (ushort)((1f - cmyk.K) * 65535) };
                }
                else if (c is LabColor lab)
                {
                    space = ColorSpace.Lab;
                    values = new ushort[] { (ushort)(lab.L * 100), (ushort)(lab.a * 100), (ushort)(lab.b * 100) };
                }
                else
                {
                    space = ColorSpace.RGB;
                    values[0] = (ushort)(c.R * 256);
                    values[1] = (ushort)(c.G * 256);
                    values[2] = (ushort)(c.B * 256);
                }

                writer.Write((ushort)space);
                writer.Write(values[0]);
                writer.Write(values[1]);
                writer.Write(values[2]);
                writer.Write(values[3]);

                if (version == 2)
                {
                    writer.Write(new byte[2]); // write a null word
                    if (!string.IsNullOrEmpty(c.Name))
                    {
                        writer.Write(c.Name.Length + 1);
                        writer.Write(Encoding.BigEndianUnicode.GetBytes(c.Name));
                    }
                    writer.Write(new byte[2]); // write a null word
                }
            }
        }
    }
}
