using System;
using System.Collections.Generic;
using System.Text;

namespace Banlan
{
    public struct LangId
    {
        public LangId(string value)
        {
            LanguageId = value;
            WithColon = false;
            WithEllipsis = false;
            Accelerator = '\0';

            if (value != null)
            {
                if (value.EndsWith(":"))
                {
                    LanguageId = value.Substring(0, value.Length - 1);
                    WithColon = true;
                }
                else if (value.EndsWith("…"))
                {
                    LanguageId = value.Substring(0, value.Length - 1);
                    WithEllipsis = true;
                }
                else if (value.EndsWith("..."))
                {
                    LanguageId = value.Substring(0, value.Length - 3);
                    WithEllipsis = true;
                }
                else
                {
                    LanguageId = value;
                }
            }
        }

        public string LanguageId { get; set; }

        public bool WithEllipsis { get; set; }

        public bool WithColon { get; set; }

        public char Accelerator { get; set; }

        public static implicit operator LangId(string langId)
        {
            if (langId == null)
            {
                return null;
            }

            return new LangId(langId);
        }
    }
}
