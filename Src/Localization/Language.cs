using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Banlan
{
    public class Language
    {
        public static readonly Language Default = new Language() { Id = 0, Code = "Default", Name = "Default", DisplayName = "Default" };

        public Language()
        {
        }

        public Language(CultureInfo culture)
        {
            Id = culture.LCID;
            Code = culture.Name;
            Name = culture.EnglishName;
            NativeName = culture.NativeName;
            DisplayName = culture.DisplayName;
        }

        public int Id { get; set; }

        public string? Code { get; set; }

        public string? Name { get; set; }

        public string? DisplayName { get; set; }

        public string? NativeName { get; set; }

        public bool IsRightToLeft { get; set; }

        public string? FlagName { get; set; }

        public int Revision { get; set; }

        public LanguageCompatibleItem[] CompatibleItems { get; set; } = [];

        public Dictionary<string, string> Words { get; private set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public string this[string name]
        {
            get
            {
                if (Words.ContainsKey(name))
                    return Words[name];
                else
                    return name;
            }
        }

        public virtual void Merge(Language lang)
        {
            if (lang == null)
                throw new ArgumentNullException();

            foreach (var de in lang.Words)
            {
                Words[de.Key] = de.Value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Code, Name);
        }
    }
}
