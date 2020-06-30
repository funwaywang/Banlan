using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Banlan.SwatchFiles
{
    class RiffPalFile : ISwatchFile
    {
        public static readonly RiffPalFile Default = new RiffPalFile();

        public string Name => "PAL";

        public string Description => "Microsoft RIFF Color palette file";

        public string[] Extensions => new string[] { ".pal" };

        public bool SupportLoad => true;

        public bool SupportSave => true;

        public bool CanLoad(string filename)
        {
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
            {
                return false;
            }

            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                var header = new byte[4];
                if (stream.Read(header, 0, 4) == 4
                    && Encoding.ASCII.GetString(header) == "RIFF")
                {
                    return true;
                }
            }

            return false;
        }

        public Swatch Load(Stream stream)
        {
            using (var reader = new BinaryReader(stream))
            {
                if (Encoding.ASCII.GetString(reader.ReadBytes(4)) != "RIFF"
                    || Encoding.ASCII.GetString(reader.ReadBytes(8), 4, 4) != "PAL ")
                {
                    throw new Exception("Invalid File Format.");
                }

                var swatch = new Swatch();
                var chunk = reader.ReadBytes(8);
                while (chunk != null && chunk.Length == 8)
                {
                    var chunkSize = BitConverter.ToInt32(chunk, 4);
                    if (Encoding.ASCII.GetString(chunk, 0, 4) == "data")
                    {
                        var buffer = reader.ReadBytes(chunkSize);
                        if (buffer.Length != chunkSize)
                        {
                            throw new InvalidDataException("Failed to read enough data to match chunk size.");
                        }
                        ReadColors(buffer, swatch);
                        break;
                    }
                    else
                    {
                        if (chunkSize % 2 != 0)
                        {
                            chunkSize++;
                        }
                        stream.Seek(chunkSize, SeekOrigin.Current);
                    }

                    chunk = reader.ReadBytes(8);
                }

                return swatch;
            }
        }

        private void ReadColors(byte[] buffer, Swatch swatch)
        {
            // The buffer should hold a LOGPALETTE structure containing
            // OS version (2 bytes, always 03)
            // Color count (2 bytes)
            // Color data (4 bytes * color count)
            var count = BitConverter.ToInt16(buffer, 2);
            var p = 4;
            for (int i = 0; i < count; i++)
            {
                swatch.Colors.Add(new RgbColor(buffer[p], buffer[p + 1], buffer[p + 2]));
                p += 4;
            }
        }

        public void Save(Swatch swatch, Stream stream)
        {
            var colors = swatch.Colors.Union(swatch.Categories.SelectMany(c => c.Colors)).ToList();

            // 4 bytes for RIFF
            // 4 bytes for document size
            // 4 bytes for PAL
            // 4 bytes for data
            // 4 bytes for chunk size
            // 2 bytes for the version
            // 2 bytes for the count
            // (4*n) for the colors
            var count = Math.Min(ushort.MaxValue, colors.Count);
            var chunkSize = 4 + count * 4;
            var docSize = 4 + 4 + 4 + 4 + 4 + 2 + 2 + count * 4;

            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(Encoding.ASCII.GetBytes("RIFF"));
                writer.Write(docSize);
                writer.Write(Encoding.ASCII.GetBytes("PAL "));
                writer.Write(Encoding.ASCII.GetBytes("data"));
                writer.Write(chunkSize);
                writer.Write((short)0x03);
                writer.Write((short)count);
                for (int i = 0; i < count; i++)
                {
                    var color = colors[i];
                    writer.Write(new byte[] { color.R, color.G, color.B, 0 });
                }
            }
        }
    }
}
