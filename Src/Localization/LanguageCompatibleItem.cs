using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banlan
{
    public class LanguageCompatibleItem
    {
        public LanguageCompatibleItem()
        {
        }

        public LanguageCompatibleItem(CultureInfo culture)
        {
            Lcid = culture.LCID;
            Code = culture.Name;
            Name = culture.EnglishName;
            NativeName = culture.NativeName;
            DisplayName = culture.DisplayName;
        }

        public int OwnerId { get; set; }
        public int Lcid { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? DisplayName { get; set; }
        public string? NativeName { get; set; }
        public string? LcidHex
        {
            get => Lcid.ToString("X4");
        }
    }
}
