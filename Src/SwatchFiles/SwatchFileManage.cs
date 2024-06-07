using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banlan.SwatchFiles
{
    static class SwatchFileManage
    {
        public static readonly ISwatchFile[] SupportedFiles = new ISwatchFile[]
        {
            ActFile.Default,
            AcoFile.Default,
            AseFile.Default,
            GifFile.Default,
            GplFile.Default,
            KctFile.Default,
            TextFile.Default,
            HtmlFile.Default,
            CorelDrawPalFile.Default,
            RiffPalFile.Default,
            BanlanFile.Default
        };

        public static ISwatchFile[] SupportedRead => SupportedFiles.Where(sf => sf.SupportLoad).ToArray();

        public static ISwatchFile[] SupportedWrite => SupportedFiles.Where(sf => sf.SupportSave).ToArray();

        public static string OpenFileFilter
        {
            get
            {
                var allExtes = string.Join(";", SupportedRead.SelectMany(s => s.Extensions).Distinct().Select(et => "*" + et));
                return $"All Supported files ({allExtes})|{allExtes}|"
                    + string.Join("|", from sf in SupportedRead
                                       let exts = string.Join(";", sf.Extensions.Select(et => "*" + et))
                                       select $"{sf.Description} ({exts})|{exts}");
            }
        }

        public static string SaveFileFilter
        {
            get
            {
                return string.Join("|", from sf in SupportedWrite
                                        let exts = string.Join(";", sf.Extensions.Select(et => "*" + et))
                                        select $"{sf.Description} ({exts})|{exts}");
            }
        }

        public static Task<Swatch?> LoadAsync(string filename, ISwatchFile? fileType = null)
        {
            return Task.Run(() => Load(filename, fileType));
        }

        public static Swatch? Load(string filename, ISwatchFile? fileType = null)
        {
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
            {
                throw new FileNotFoundException();
            }

            var extension = Path.GetExtension(filename);
            if (fileType == null)
            {
                var fileTypes = SupportedRead.Where(f => f.Extensions.Contains(extension, StringComparer.OrdinalIgnoreCase)).ToList();
                if (fileTypes.Count > 1)
                {
                    fileType = fileTypes.FirstOrDefault(ft => ft.CanLoad(filename));
                    if (fileType == null)
                    {
                        throw new NotSupportedException($"Not supported file type [{extension}].");
                    }
                }
                else if (fileTypes.Count == 1)
                {
                    fileType = fileTypes[0];
                }
            }

            if (fileType != null)
            {
                using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    var swatch = fileType.Load(stream);
                    if (swatch != null)
                    {
                        swatch.FileName = filename;
                    }
                    return swatch;
                }
            }
            else
            {
                throw new NotSupportedException($"Not supported file type [{extension}].");
            }
        }

        public static Task SaveAsync(Swatch swatch, string filename, ISwatchFile? fileType = null)
        {
            return Task.Run(() => Save(swatch, filename, fileType));
        }

        public static void Save(Swatch swatch, string filename, ISwatchFile? fileType = null)
        {
            if (swatch == null || filename == null)
            {
                throw new ArgumentNullException();
            }

            if (fileType == null)
            {
                var extension = Path.GetExtension(filename);
                fileType = SupportedWrite.FirstOrDefault(f => f.Extensions.Contains(extension, StringComparer.OrdinalIgnoreCase));
                if (fileType == null)
                {
                    throw new NotSupportedException($"Not supported file type [{extension}].");
                }
            }

            if (!fileType.SupportSave)
            {
                throw new NotSupportedException($"Not supported to save file as type [{fileType.Name}].");
            }

            var fileInfo = new FileInfo(filename);
            if (fileInfo.Exists && fileInfo.IsReadOnly)
            {
                throw new Exception($"The file is read only.");
            }

            using (var stream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fileType.Save(swatch, stream);
                swatch.FileName = filename;
            }
        }

        public static Task SaveAsync(Swatch swatch, Stream stream, ISwatchFile? fileType = null)
        {
            return Task.Run(() => Save(swatch, stream, fileType));
        }

        public static void Save(Swatch swatch, Stream stream, ISwatchFile? fileType)
        {
            if (swatch == null || fileType == null)
            {
                throw new ArgumentNullException();
            }

            if (!fileType.SupportSave)
            {
                throw new NotSupportedException($"Not supported to save file as type [{fileType.Name}].");
            }

            fileType.Save(swatch, stream);
        }

        public static bool SupportToSaveAs(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return false;
            }

            var extension = Path.GetExtension(filename);
            return SupportedWrite.Any(sw => sw.Extensions.Any(ex => ex.Equals(extension, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
