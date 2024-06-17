using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Banlan.SwatchFiles
{
    class KctFile : ISwatchFile
    {
        public static readonly KctFile Default = new KctFile();

        public string Name => "KCT";

        public string Description => "Kong Color Table file";

        public string[] Extensions => new string[] { ".kct" };

        public bool SupportLoad => true;

        public bool SupportSave => true;

        public bool CanLoad(string filename)
        {
            return true;
        }

        public Swatch Load(Stream stream)
        {
            var dom = new XmlDocument();
            dom.Load(stream);
            if (dom.DocumentElement?.Name != "root")
            {
                throw new Exception("Invalid File Format.");
            }

            var swatch = new Swatch
            {
                Name = (dom.DocumentElement.SelectSingleNode("info/title") as XmlElement)?.InnerText
            };

            if (dom.DocumentElement.SelectNodes("colors/item") is XmlNodeList itemNodes)
            {
                foreach (XmlElement node in itemNodes)
                {
                    var color = ColorHelper.ParseColor(node.GetAttribute("value"));
                    if (color != null)
                    {
                        swatch.Colors.Add(new RgbColor(color.Value.R, color.Value.G, color.Value.B)
                        {
                            Name = node.GetAttribute("name")
                        });
                    }
                }
            }

            return swatch;
        }

        public void Save(Swatch swatch, Stream stream)
        {
            var dom = new XmlDocument();
            SaveToXml(swatch, dom);
            dom.Save(stream);
        }

        private void SaveToXml(Swatch swatch, XmlDocument dom)
        {
            dom.AppendChild(dom.CreateXmlDeclaration("1.0", "utf-8", "yes"));
            var documentElement = dom.CreateElement("root");
            dom.AppendChild(documentElement);

            if (!string.IsNullOrWhiteSpace(swatch.Name))
            {
                var title = dom.CreateElement("title");
                title.InnerText = swatch.Name;
                var info = dom.CreateElement("info");
                info.AppendChild(title);
                documentElement.AppendChild(info);
            }

            var colors = dom.CreateElement("colors");
            documentElement.AppendChild(colors);
            foreach (var c in swatch.Colors.Union(swatch.Categories.SelectMany(c => c.Colors)))
            {
                var item = dom.CreateElement("item");
                item.SetAttribute("value", ColorHelper.ToHexColor(c.R, c.G, c.B, false));
                item.SetAttribute("name", c.Name);
                colors.AppendChild(item);
            }
        }
    }
}
