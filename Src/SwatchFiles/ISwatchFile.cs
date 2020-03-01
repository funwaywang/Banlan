using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Banlan.SwatchFiles
{
    public interface ISwatchFile
    {
        string Name { get; }

        string Description { get; }

        string[] Extensions { get; }

        bool SupportLoad { get; }

        bool SupportSave { get; }

        Swatch Load(Stream stream);

        void Save(Swatch swatch, Stream stream);

        bool CanLoad(string filename);
    }
}
