using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Banlan
{
    public class Settings
    {
        public static readonly Settings Default = new Settings();
        private readonly string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(Banlan), "Settings.xml");

        public Dictionary<string, string?> Values { get; private set; } = new(StringComparer.OrdinalIgnoreCase);

        public string? this[string name]
        {
            get => Values.TryGetValue(name, out string? value) ? value : null;
            set => Values[name] = value;
        }

        public void Save()
        {
            var directory = Path.GetDirectoryName(fileName);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var dom = new XmlDocument();
            dom.AppendChild(dom.CreateXmlDeclaration("1.0", "utf-8", "yes"));

            var documentElement = dom.CreateElement("settings");
            dom.AppendChild(documentElement);

            var values = dom.CreateElement("values");
            documentElement.AppendChild(values);
            foreach (var value in Values)
            {
                if (value.Value != null)
                {
                    var valueElement = dom.CreateElement("item");
                    valueElement.SetAttribute("name", value.Key);
                    valueElement.InnerText = value.Value;
                    values.AppendChild(valueElement);
                }
            }

            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                dom.Save(fs);
            }
        }

        public void Load()
        {
            if (File.Exists(fileName))
            {
                var dom = new XmlDocument();
                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    dom.Load(fs);
                }

                if (dom.DocumentElement?.Name == "settings")
                {
                    if (dom.DocumentElement.SelectNodes("values/item") is XmlNodeList itemNodes)
                    {
                        foreach (var node in itemNodes.OfType<XmlElement>())
                        {
                            var name = node.GetAttribute("name");
                            if (name != null)
                            {
                                Values[name] = node.InnerText;
                            }
                        }
                    }
                }
            }
        }

        public void Remove(string item)
        {
            if (item != null)
            {
                Values.Remove(item);
            }
        }

        public string? Get(string name, string? defaultValue = null)
        {
            return Values.TryGetValue(name, out string? value) ? value : defaultValue;
        }

        public void Set(string name, string value)
        {
            if (name != null)
            {
                Values[name] = value;
            }
        }
    }
}
