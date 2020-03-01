using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Banlan.SwatchFiles
{
    class GifFile : ISwatchFile
    {
        public static readonly GifFile Default = new GifFile();

        public string Name => "GIF";

        public string Description => "Graphics Interchange Format";

        public string[] Extensions => new string[] { ".gif" };

        public bool SupportLoad => true;

        public bool SupportSave => false;

        public bool CanLoad(string filename)
        {
            return true;
        }

        public Swatch Load(Stream stream)
        {
            using var reader = new BinaryReader(stream);
            if (Encoding.ASCII.GetString(reader.ReadBytes(3)) != "GIF")
            {
                throw new Exception("Invalid File Format.");
            }

            var version = Encoding.ASCII.GetString(reader.ReadBytes(3)); // "87a" or "89a"
            stream.Seek(4, SeekOrigin.Current);
            var flags = reader.ReadByte();
            var globalTableSize = 1 << ((flags & 0b00000111) + 1);
            var colotTableSorted = (flags & 0b00001000) > 0;
            var hasGlobalTable = (flags & 0b10000000) > 0;
            stream.Seek(2, SeekOrigin.Current);

            var swatch = new Swatch();
            var colorSet = new HashSet<int>();
            if (hasGlobalTable)
            {
                for (int i = 0; i < globalTableSize; i++)
                {
                    var rgb = reader.ReadBytes(3);
                    var value = rgb[0] << 16 | rgb[1] << 8 | rgb[2];
                    if (!colorSet.Contains(value))
                    {
                        colorSet.Add(value);
                        swatch.Colors.Add(new RgbColor(rgb[0], rgb[1], rgb[2]));
                    }
                }
            }

            var label = reader.ReadBytes(1);
            while (label.Length == 1 && label[0] != 0x3b)
            {
                if (label[0] == 0x21)
                {
                    // this is a extension block
                    reader.ReadByte();
                    var size = reader.ReadByte();
                    stream.Seek(size, SeekOrigin.Current);
                    var subBlockSize = reader.ReadByte();
                    while (subBlockSize > 0)
                    {
                        stream.Seek(subBlockSize, SeekOrigin.Current);
                        subBlockSize = reader.ReadByte();
                    }
                }
                else if (label[0] == 0x2c)
                {
                    // this is a image data block
                    var frame = reader.ReadBytes(9);
                    if (frame.Length == 9)
                    {
                        var flag = frame[8];
                        var hasLocalTable = (flag & 0b10000000) > 1;
                        if (hasLocalTable)
                        {
                            var localTableSize = 1 << ((flag & 0b00000111) + 1);
                            for (int i = 0; i < localTableSize; i++)
                            {
                                var rgb = reader.ReadBytes(3);
                                var value = rgb[0] << 16 | rgb[1] << 8 | rgb[2];
                                if (!colorSet.Contains(value))
                                {
                                    colorSet.Add(value);
                                    swatch.Colors.Add(new RgbColor(rgb[0], rgb[1], rgb[2]));
                                    if(swatch.Colors.Count >= 256)
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        if (swatch.Colors.Count >= 256)
                        {
                            break;
                        }

                        // LZW Minimum Code Size
                        reader.ReadByte();

                        // Data Block
                        var blockSize = reader.ReadByte();
                        while (blockSize > 0)
                        {
                            stream.Seek(blockSize, SeekOrigin.Current);
                            blockSize = reader.ReadByte();
                        }
                    }
                }

                label = reader.ReadBytes(1);
            }

            return swatch;
        }

        public void Save(Swatch swatch, Stream stream)
        {
            throw new NotSupportedException();
        }
    }
}
