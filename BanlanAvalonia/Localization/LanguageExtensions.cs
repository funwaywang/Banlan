using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

namespace Banlan
{
    public static class LanguageExtensions
    {
        private static readonly Regex WordWithMarkRegex = new Regex(@"(\w.*\w)([\s\W]+)", RegexOptions.Compiled);

        [return: NotNullIfNotNull(nameof(name))]
        public static string? GetText(this Language current, string? name)
        {
            if (string.IsNullOrEmpty(name) || current == null)
            {
                return name;
            }

            if (current.Words.ContainsKey(name))
            {
                return current.Words[name];
            }
            else if (name.EndsWith("..."))
            {
                var n2 = name.Substring(0, name.Length - 3).Trim();
                if (current.Words.ContainsKey(n2))
                {
                    return current.Words[n2] + "…";
                }
            }
            else if (WordWithMarkRegex.Match(name) is Match match && match.Success
                && current.Words.ContainsKey(match.Groups[1].Value))
            {
                var n2 = match.Groups[1].Value.Trim();
                if (current.Words.ContainsKey(n2))
                {
                    return current.Words[n2] + match.Groups[2].Value;
                }
            }

            return name;
        }

        public static string GetText(this Language current, string name, string defaultValue)
        {
            if (name == null)
            {
                return defaultValue;
            }

            if (current != null && current.Words.ContainsKey(name))
            {
                return current.Words[name];
            }
            else
            {
                return defaultValue;
            }
        }

        public static string? GetTextAny(this Language current, string name, params string[] names)
        {
            if (names == null || names.Length == 0)
            {
                return GetText(current, name);
            }

            if (current == null)
            {
                if (string.IsNullOrEmpty(name))
                {
                    return names.FirstOrDefault(n => !string.IsNullOrEmpty(n));
                }
                else
                {
                    return name;
                }
            }
            else
            {
                if (current.Words.ContainsKey(name))
                {
                    return current.Words[name];
                }
                else
                {
                    var key = names.FirstOrDefault(n => current.Words.ContainsKey(n));
                    if (key == null)
                    {
                        if (string.IsNullOrEmpty(name))
                        {
                            return names.FirstOrDefault(n => !string.IsNullOrEmpty(n));
                        }
                        else
                        {
                            return name;
                        }
                    }
                    else
                    {
                        return current.Words[key];
                    }
                }
            }
        }

        public static string? GetTextWithEllipsis(this Language current, string name)
        {
            if (current != null && current.Words.ContainsKey(name))
            {
                return current.Words[name] + "…";
            }
            else
            {
                return name + "…";
            }
        }

        public static string? GetTextWithColon(this Language current, string name)
        {
            return GetText(current, name, false, true, '\0');
        }

        public static string? GetTextWithAccelerator(this Language current, string name, bool withEllipsis, char accelerator)
        {
            return GetText(current, name, true, false, accelerator);
        }

        public static string? GetTextWithAccelerator(this Language current, string name, char accelerator)
        {
            return GetText(current, name, false, false, accelerator);
        }

        public static string? GetText(this Language current, LangId langId)
        {
            return GetText(current, langId.LanguageId, langId.WithEllipsis, langId.WithColon, langId.Accelerator);
        }

        public static string? GetText(this Language current, string name, bool withEllipsis, bool withColon, char accelerator)
        {
            var str = (current != null && current.Words.ContainsKey(name)) ? current.Words[name] : name;
            if (str == null)
            {
                return str;
            }

            if (accelerator > 0)
            {
                str = StringHelper.AddAccelerator(str, accelerator);
            }

            if (withEllipsis)
            {
                str += "…";
            }

            if (withColon)
            {
                str += ":";
            }

            return str;
        }

        public static string Format(this Language current, string name, params object[] args)
        {
            return Format(current, name, true, args);
        }

        public static string Format(this Language current, string name, bool withArgs, params object?[] args)
        {
            if (withArgs && args != null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    object? arg = args[i];
                    if (arg is string)
                    {
                        args[i] = GetText(current, (string)arg);
                    }
                }

                return string.Format(GetText(current, name), args);
            }
            else
            {
                return GetText(current, name);
            }
        }
    }
}
