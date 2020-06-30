using Banlan.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Banlan.SwatchFiles
{
    class AseFile : ISwatchFile
    {
        enum BlockType
        {
            Color = 0x0001,
            GroupStart = 0xC001,
            GroupEnd = 0xC002
        }

        enum ColorType
        {
            Global,
            Spot,
            Normal
        }

        public static readonly AseFile Default = new AseFile();

        public string Name => "ASE";

        public string Description => "Adobe Swatch Exchange file";

        public string[] Extensions => new string[] { ".ase" };

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
                var signature = Encoding.ASCII.GetString(reader.ReadBytes(4));
                if (signature != "ASEF")
                {
                    throw new Exception("Invalid File Format.");
                }

                var majorVersion = reader.ReadInt16();
                var minorVersion = reader.ReadInt16();
                var blocks = reader.ReadUInt32();

                var swatch = new Swatch();
                Category category = swatch;
                var categoryStack = new Stack<Category>();

                for (int i = 0; i < blocks; i++)
                {
                    var blockType = (BlockType)reader.ReadUInt16();
                    var blockLength = reader.ReadUInt32();
                    var position = stream.Position;
                    string name = null;

                    switch (blockType)
                    {
                        case BlockType.Color:
                            name = ReadName(reader);
                            var color = ReadColorBlock(reader);
                            if (color != null)
                            {
                                color.Name = name;
                                category.Colors.Add(color);
                            }
                            break;
                        case BlockType.GroupStart:
                            name = ReadName(reader);
                            categoryStack.Push(category);
                            category = new Category
                            {
                                Name = name
                            };
                            swatch.Categories.Add(category);
                            break;
                        case BlockType.GroupEnd:
                            category = categoryStack.Pop() ?? swatch;
                            break;
                    }

                    // move to next block
                    var nextPosition = position + blockLength;
                    if (stream.Position < nextPosition && i < blocks - 1)
                    {
                        stream.Seek(nextPosition - stream.Position, SeekOrigin.Current);
                    }
                }

                return swatch;
            }
        }

        private string ReadName(BigEndianBinaryReader reader)
        {
            var length = reader.ReadInt16();
            if (length > 0)
            {
                var buffer = reader.ReadBytes(length * 2);
                return Encoding.BigEndianUnicode.GetString(buffer, 0, buffer.Length - 2);
            }

            return null;
        }

        private ColorBase ReadColorBlock(BinaryReader reader)
        {
            var value1 = 0f;
            var value2 = 0f;
            var value3 = 0f;
            var value4 = 0f;

            var colorMode = Encoding.ASCII.GetString(reader.ReadBytes(4));
            ColorBase color;
            switch (colorMode)
            {
                case "RGB ":
                    value1 = reader.ReadSingle();
                    value2 = reader.ReadSingle();
                    value3 = reader.ReadSingle();
                    color = new RgbColor(Convert.ToByte(255 * value1), Convert.ToByte(255 * value2), Convert.ToByte(255 * value3));
                    break;
                case "CMYK":
                    value1 = reader.ReadSingle();
                    value2 = reader.ReadSingle();
                    value3 = reader.ReadSingle();
                    value4 = reader.ReadSingle();
                    color = new CmykColor(value1, value2, value3, value4);
                    break;
                case "Gray":
                    value1 = reader.ReadSingle();
                    color = new GrayColor(value1);
                    break;
                case "LAB ":
                    value1 = reader.ReadSingle();
                    value2 = reader.ReadSingle();
                    value3 = reader.ReadSingle();
                    color = new LabColor(value1 * 100, value2, value3);
                    break;
                default:
                    throw new InvalidDataException($"Unsupported color mode '{colorMode}'.");
            }

            var colorSpace = (ColorType)reader.ReadInt16();
            return color;
        }

        public void Save(Swatch swatch, Stream stream)
        {
            using (var writer = new BigEndianBinaryWriter(stream))
            {
                writer.Write(Encoding.ASCII.GetBytes("ASEF")); // signature
                writer.Write((ushort)1); // major Version
                writer.Write((ushort)0); // minor Version

                var blockNumber = swatch.Colors.Count + swatch.Categories.Count * 2 + swatch.Categories.SelectMany(c => c.Colors).Count();
                writer.Write(blockNumber);

                WriteColors(writer, swatch.Colors);
                foreach (var category in swatch.Categories)
                {
                    writer.Write((ushort)BlockType.GroupStart);

                    var blockLength = 0;
                    byte[] nameBytes = null;
                    var name = category.Name ?? "";
                    if (name != null)
                    {
                        nameBytes = Encoding.BigEndianUnicode.GetBytes(name);
                        blockLength += 2 + nameBytes.Length + 2;
                    }
                    else
                    {
                        blockLength += 2;
                    }

                    writer.Write(blockLength);
                    if (nameBytes != null)
                    {
                        writer.Write((short)(name.Length + 1));
                        writer.Write(nameBytes);
                        writer.Write(new byte[] { 0, 0 });
                    }
                    else
                    {
                        writer.Write((short)0);
                    }

                    WriteColors(writer, category.Colors);

                    writer.Write((ushort)BlockType.GroupEnd);
                    writer.Write(0);
                }
            }
        }

        private void WriteColors(BigEndianBinaryWriter writer, IEnumerable<ColorBase> colors)
        {
            foreach (var color in colors)
            {
                writer.Write((ushort)BlockType.Color);

                string colorMode;
                float[] data = null;
                if (color is CmykColor cmyk)
                {
                    colorMode = "CMYK";
                    data = new float[] { cmyk.C, cmyk.M, cmyk.Y, cmyk.K };
                }
                else if (color is GrayColor gray)
                {
                    colorMode = "Gray";
                    data = new float[] { gray.Value };
                }
                else if (color is LabColor lab)
                {
                    colorMode = "LAB ";
                    data = new float[] { lab.L / 100f, lab.a / 100f, lab.b / 100f };
                }
                else
                {
                    colorMode = "RGB ";
                    data = new float[] { color.R / 255f, color.G / 255f, color.B / 255f };
                }

                var blockLength = 4 + data.Length * 4 + 2;

                byte[] nameBytes = null;
                var name = color.Name ?? "";
                if (name != null)
                {
                    nameBytes = Encoding.BigEndianUnicode.GetBytes(name);
                    blockLength += 2 + nameBytes.Length + 2;
                }
                else
                {
                    blockLength += 2;
                }

                writer.Write(blockLength);
                if (nameBytes != null)
                {
                    writer.Write((short)(name.Length + 1));
                    writer.Write(nameBytes);
                    writer.Write(new byte[] { 0, 0 });
                }
                else
                {
                    writer.Write((short)0);
                }

                writer.Write(Encoding.ASCII.GetBytes(colorMode));
                foreach (var d in data)
                {
                    writer.Write(d);
                }
                writer.Write((ushort)ColorType.Global);
            }
        }
    }
}
