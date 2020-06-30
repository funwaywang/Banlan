using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Banlan.SwatchFiles
{
    class BanlanFile : ISwatchFile
    {
        public static readonly BanlanFile Default = new BanlanFile();

        public string Name => "BANLAN";

        public string Description => "Banlan Swatch file";

        public string[] Extensions => new string[] { ".bls" };

        public bool SupportLoad => true;

        public bool SupportSave => true;

        public bool CanLoad(string filename)
        {
            return true;
        }

        public void Load(string filename, Swatch swatch)
        {
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                var dom = new XmlDocument();
                dom.Load(stream);
                Load(dom, swatch);
            }
        }

        public Swatch Load(Stream stream)
        {
            var dom = new XmlDocument();
            dom.Load(stream);

            var swatch = new Swatch
            {
                Name = dom.DocumentElement.GetAttribute("name")
            };

            Load(dom, swatch);

            return swatch;
        }

        private void Load(XmlDocument dom, Swatch swatch)
        {
            if (dom.DocumentElement?.Name != "banlan_swatch")
            {
                throw new Exception("Invalid File Format.");
            }

            ReadColors(swatch.Colors, dom.DocumentElement.SelectSingleNode("colors") as XmlElement);

            if (dom.DocumentElement.SelectSingleNode("categories") is XmlElement categoriesNode)
            {
                foreach (var categoryNode in categoriesNode.SelectNodes("category").OfType<XmlElement>())
                {
                    var category = new Category
                    {
                        Name = categoryNode.GetAttribute("name"),
                        IsOpen = StringHelper.GetBool(categoryNode.GetAttribute("isOpen"), true)
                    };
                    ReadColors(category.Colors, categoryNode.SelectSingleNode("colors") as XmlElement);
                    swatch.Categories.Add(category);
                }
            }
        }

        public void Save(Swatch swatch, string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                Save(swatch, stream);
            }
        }

        private void ReadColors(IList<ColorBase> colors, XmlElement xmlElement)
        {
            if (colors == null || xmlElement == null)
            {
                return;
            }

            foreach (var colorElement in xmlElement.SelectNodes("color").OfType<XmlElement>())
            {
                if (ReadColor(colorElement) is ColorBase color)
                {
                    colors.Add(color);
                }
            }
        }

        private ColorBase ReadColor(XmlElement xmlElement)
        {
            var name = xmlElement.GetAttribute("name");
            var type = xmlElement.GetAttribute("type");
            switch (type.ToUpper())
            {
                case "CMYK":
                    if (float.TryParse(xmlElement.GetAttribute("C"), out float c)
                        && float.TryParse(xmlElement.GetAttribute("M"), out float m)
                        && float.TryParse(xmlElement.GetAttribute("Y"), out float y)
                        && float.TryParse(xmlElement.GetAttribute("K"), out float k))
                    {
                        return new CmykColor(c, m, y, k) { Name = name };
                    }
                    break;
                case "GRAY":
                    if (float.TryParse(xmlElement.GetAttribute("Value"), out float value))
                    {
                        return new GrayColor(value) { Name = name };
                    }
                    break;
                case "HSL":
                    if (float.TryParse(xmlElement.GetAttribute("H"), out float h)
                        && float.TryParse(xmlElement.GetAttribute("S"), out float s)
                        && float.TryParse(xmlElement.GetAttribute("L"), out float l))
                    {
                        return new HslColor(h, s, l) { Name = name };
                    }
                    break;
                case "HSV":
                    if (float.TryParse(xmlElement.GetAttribute("H"), out float h2)
                        && float.TryParse(xmlElement.GetAttribute("S"), out float s2)
                        && float.TryParse(xmlElement.GetAttribute("V"), out float v2))
                    {
                        return new HslColor(h2, s2, v2) { Name = name };
                    }
                    break;
                case "LAB":
                    if (float.TryParse(xmlElement.GetAttribute("L"), out float l3)
                        && float.TryParse(xmlElement.GetAttribute("a"), out float a3)
                        && float.TryParse(xmlElement.GetAttribute("b"), out float b3))
                    {
                        return new LabColor(l3, a3, b3) { Name = name };
                    }
                    break;
                case "RGB":
                default:
                    if (byte.TryParse(xmlElement.GetAttribute("R"), out byte r)
                        && byte.TryParse(xmlElement.GetAttribute("G"), out byte g)
                        && byte.TryParse(xmlElement.GetAttribute("B"), out byte b))
                    {
                        return new RgbColor(r, g, b) { Name = name };
                    }
                    break;
            }

            return null;
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
            dom.AppendChild(dom.CreateElement("banlan_swatch"));

            if (!string.IsNullOrEmpty(swatch.Name))
            {
                dom.DocumentElement.SetAttribute("name", swatch.Name);
            }

            if (swatch.Colors.Any())
            {
                var defaultNode = dom.CreateElement("colors");
                dom.DocumentElement.AppendChild(defaultNode);
                foreach (var c in swatch.Colors)
                {
                    WriteColorTo(c, defaultNode);
                }
            }

            if (swatch.Categories.Any())
            {
                var categoriesNode = dom.CreateElement("categories");
                dom.DocumentElement.AppendChild(categoriesNode);
                foreach (var category in swatch.Categories)
                {
                    var categoryNode = dom.CreateElement("category");
                    categoryNode.SetAttribute("name", category.Name);
                    if (!category.IsOpen)
                    {
                        categoryNode.SetAttribute("isOpen", category.IsOpen.ToString());
                    }
                    categoriesNode.AppendChild(categoryNode);
                    if (category.Colors.Any())
                    {
                        var colorsNode = dom.CreateElement("colors");
                        categoryNode.AppendChild(colorsNode);
                        foreach (var c in category.Colors)
                        {
                            WriteColorTo(c, colorsNode);
                        }
                    }
                }
            }
        }

        private void WriteColorTo(ColorBase color, XmlElement parentElement)
        {
            var element = parentElement.OwnerDocument.CreateElement("color");
            parentElement.AppendChild(element);

            if (!string.IsNullOrEmpty(color.Name))
            {
                element.SetAttribute("name", color.Name);
            }

            if (color is RgbColor rgb)
            {
                element.SetAttribute("type", "RGB");
                element.SetAttribute("R", rgb.R.ToString());
                element.SetAttribute("G", rgb.G.ToString());
                element.SetAttribute("B", rgb.B.ToString());
            }
            else if (color is CmykColor cmyk)
            {
                element.SetAttribute("type", "CMYK");
                element.SetAttribute("C", cmyk.C.ToString());
                element.SetAttribute("M", cmyk.M.ToString());
                element.SetAttribute("Y", cmyk.Y.ToString());
                element.SetAttribute("K", cmyk.K.ToString());
            }
            else if (color is GrayColor gray)
            {
                element.SetAttribute("type", "GRAY");
                element.SetAttribute("Value", gray.Value.ToString());
            }
            else if (color is HslColor hsl)
            {
                element.SetAttribute("type", "HSL");
                element.SetAttribute("H", hsl.H.ToString());
                element.SetAttribute("S", hsl.S.ToString());
                element.SetAttribute("L", hsl.L.ToString());
            }
            else if (color is HsvColor hsv)
            {
                element.SetAttribute("type", "HSV");
                element.SetAttribute("H", hsv.H.ToString());
                element.SetAttribute("S", hsv.S.ToString());
                element.SetAttribute("V", hsv.V.ToString());
            }
            else if (color is LabColor lab)
            {
                element.SetAttribute("type", "LAB");
                element.SetAttribute("L", lab.L.ToString());
                element.SetAttribute("a", lab.a.ToString());
                element.SetAttribute("b", lab.b.ToString());
            }
            else
            {
                element.SetAttribute("type", color.GetType().Name);
                element.SetAttribute("R", color.R.ToString());
                element.SetAttribute("G", color.G.ToString());
                element.SetAttribute("B", color.B.ToString());
            }
        }
    }
}
