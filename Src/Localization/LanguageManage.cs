using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Xml;

namespace Banlan
{
    public class LanguageManage : IEnumerable<Language>, INotifyPropertyChanged
    {
        private Language? _CurrentLanguage;
        public static readonly LanguageManage Default = new LanguageManage { Language.Default };

        public event PropertyChangedEventHandler? PropertyChanged;

        public Language? CurrentLanguage
        {
            get => _CurrentLanguage;
            private set
            {
                if (_CurrentLanguage != value)
                {
                    _CurrentLanguage = value;
                    OnPropertyChanged(nameof(CurrentLanguage));
                }
            }
        }

        public List<Language> Languages { get; private set; } = new List<Language>();

        public bool Contains(string name)
        {
            if (CurrentLanguage != null)
            {
                return CurrentLanguage.Words.ContainsKey(name);
            }
            else
            {
                return false;
            }
        }

        public Language? LoadXmlString(string xml)
        {
            var dom = new XmlDocument();
            dom.LoadXml(xml);
            return LoadXml(dom);
        }

        public Language? LoadXml(string filename)
        {
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
            {
                return null;
            }

            var dom = new XmlDocument();
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                dom.Load(stream);
                return LoadXml(dom);
            }
        }

        public Language? LoadXml(Stream stream)
        {
            var dom = new XmlDocument();
            try
            {
                dom.Load(stream);
            }
            catch
            {
                return null;
            }

            return LoadXml(dom);
        }

        public Language? LoadXml(XmlDocument dom)
        {
            if (dom.DocumentElement == null)
            {
                return null;
            }

            var infoElement = dom.DocumentElement.SelectSingleNode("information") as XmlElement;
            if (infoElement == null)
            {
                return null;
            }

            if (!int.TryParse(infoElement.GetAttribute("id"), out int id))
            {
                StringHelper.TryParseHex(infoElement.GetAttribute("id"), out id);
            }

            var language = new Language
            {
                Id = id,
                Code = infoElement.GetAttribute("code"),
                Name = infoElement.GetAttribute("name"),
                IsRightToLeft = StringHelper.GetBoolDefault(infoElement.GetAttribute("is_right_to_left")),
                Revision = StringHelper.GetIntDefault(infoElement.GetAttribute("revision"))
            };

            var compatibility = dom.DocumentElement.SelectSingleNode("compatibility");
            if (compatibility != null)
            {
                var comps = new List<LanguageCompatibleItem>();
                foreach (XmlElement ce in compatibility)
                {
                    var value = ce.GetAttribute("value");
                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }

                    if (int.TryParse(value, out int lcid) || StringHelper.TryParseHex(value, out lcid))
                    {
                        if (lcid != language.Id)
                        {
                            comps.Add(new LanguageCompatibleItem() { Lcid = lcid, Code = ce.GetAttribute("code") });
                        }
                    }
                }
                language.CompatibleItems = comps.Distinct().ToArray();
            }

            var wordsNode = dom.DocumentElement.SelectSingleNode("words");
            if (wordsNode != null)
            {
                LoadXmlNodes(language.Words, wordsNode);
            }

            return language;
        }

        private void LoadXmlNodes(Dictionary<string, string> words, XmlNode group)
        {
            XmlNodeList? list = group.SelectNodes("item");
            if (list != null)
            {
                foreach (XmlElement node in list)
                {
                    string name = node.GetAttribute("name");
                    if (!words.ContainsKey(name))
                    {
                        words.Add(name, node.InnerText);
                    }
                }
            }
        }

        public void SaveXmlFile(Language language, string filename)
        {
            if (language == null || filename == null)
            {
                throw new ArgumentNullException();
            }
            if (filename == string.Empty)
            {
                throw new ArgumentException("filename");
            }

            var dom = SaveToXml(language);
            if (dom != null)
            {
                var dir = Path.GetDirectoryName(filename);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
                {
                    dom.Save(stream);
                }
            }
        }

        public XmlDocument SaveToXml(Language language)
        {
            if (language == null)
            {
                throw new ArgumentNullException();
            }

            var dom = new XmlDocument();
            dom.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?>");

            var documentElement = dom.CreateElement("dictionary");
            dom.AppendChild(documentElement);

            var infoElement = dom.CreateElement("information");
            infoElement.SetAttribute("id", language.Id.ToString());
            infoElement.SetAttribute("code", language.Code);
            infoElement.SetAttribute("name", language.Name);
            infoElement.SetAttribute("revision", language.Revision.ToString());
            infoElement.SetAttribute("is_right_to_left", language.IsRightToLeft.ToString());
            documentElement.AppendChild(infoElement);

            if (language.CompatibleItems.Any())
            {
                var compatibilityElement = dom.CreateElement("compatibility");
                foreach (var ci in language.CompatibleItems)
                {
                    var ciElement = dom.CreateElement("lcid");
                    ciElement.SetAttribute("value", ci.Lcid.ToString());
                    ciElement.SetAttribute("code", ci.Code);
                    compatibilityElement.AppendChild(ciElement);
                }
                documentElement.AppendChild(compatibilityElement);
            }

            var wordsElement = dom.CreateElement("words");
            foreach (var word in language.Words)
            {
                var itemElement = dom.CreateElement("item");
                itemElement.SetAttribute("name", word.Key);
                itemElement.InnerText = word.Value;
                wordsElement.AppendChild(itemElement);
            }
            documentElement.AppendChild(wordsElement);

            return dom;
        }

        public void Add(Language language)
        {
            var exists = Languages.FirstOrDefault(l => l.Id == language.Id);
            if (exists != null)
            {
                exists.Merge(language);
            }
            else
            {
                Languages.Add(language);
            }
        }

        public void AddXmlString(string xml)
        {
            var language = LoadXmlString(xml);
            if (language != null)
            {
                Languages.Add(language);
            }
        }

        public void LoadFolder(string path)
        {
            try
            {
                if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                {
                    foreach (var filename in Directory.GetFiles(path, "*.xml"))
                    {
                        if (LoadXml(filename) is Language language)
                        {
                            Add(language);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public void SelectOrDefault(string? code = null)
        {
            if (!string.IsNullOrEmpty(code) && Languages.FirstOrDefault(lg => string.Equals(lg.Code, code, StringComparison.OrdinalIgnoreCase)) is Language lang)
            {
                CurrentLanguage = lang;
            }
            else
            {
                CurrentLanguage = Language.Default;
            }
        }

        public bool SwitchLanguage(string code)
        {
            if (!string.IsNullOrEmpty(code)
                && Languages.FirstOrDefault(lg => string.Equals(lg.Code, code, StringComparison.OrdinalIgnoreCase)) is Language lang)
            {
                CurrentLanguage = lang;
                return true;
            }

            return false;
        }

        public void SwitchLanguage(Language language)
        {
            CurrentLanguage = language;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public IEnumerator<Language> GetEnumerator()
        {
            return Languages.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Languages.GetEnumerator();
        }
    }
}
