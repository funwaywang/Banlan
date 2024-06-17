using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Banlan
{
    public class RecentFile
    {
        private const int MaxRecentFiles = 12;
        public static readonly RecentFile Default = new RecentFile();
        private readonly string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(Banlan), "RecentFiles.xml");

        public ObservableCollection<SwatchFileSummary> RecentFiles { get; private set; } = new ObservableCollection<SwatchFileSummary>();

        public void Save()
        {
            var directory = Path.GetDirectoryName(fileName);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var dom = new XmlDocument();
            dom.AppendChild(dom.CreateXmlDeclaration("1.0", "utf-8", "yes"));

            var documentElement = dom.CreateElement("recent_files");
            dom.AppendChild(documentElement);

            var count = 0;
            foreach (var file in RecentFiles)
            {
                var fileElement = dom.CreateElement("file");
                if (file.Samples != null)
                {
                    fileElement.SetAttribute("samples", string.Join(",", file.Samples.Select(s => ColorHelper.ToHexColor(s))));
                }
                fileElement.SetAttribute("update_time", file.UpdateTime.ToString("yyyy/MM/dd HH:mm:ss"));
                if (file.FileName != null)
                {
                    fileElement.InnerText = file.FileName;
                }
                documentElement.AppendChild(fileElement);

                count++;
                if (count >= MaxRecentFiles)
                {
                    break;
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

                if (dom.DocumentElement?.Name == "recent_files")
                {
                    if (dom.DocumentElement.SelectNodes("file") is XmlNodeList fileNodes)
                    {
                        foreach (var node in fileNodes.OfType<XmlElement>())
                        {
                            var recentFile = new SwatchFileSummary
                            {
                                FileName = node.InnerText,
                                Samples = (from cs in node.GetAttribute("samples").Split(',')
                                           let co = ColorHelper.ParseColor(cs)
                                           where co != null
                                           select co.Value).ToArray()
                            };

                            if (DateTime.TryParse(node.GetAttribute("update_time"), out DateTime updateTime))
                            {
                                recentFile.UpdateTime = updateTime;
                            }
                            else
                            {
                                recentFile.UpdateTime = DateTime.Now;
                            }

                            if (!string.IsNullOrEmpty(recentFile.FileName))
                            {
                                RecentFiles.Add(recentFile);
                            }
                        }
                    }
                }
            }
        }

        public void AddRecentFile(SwatchFileSummary swatchFile)
        {
            if (!string.IsNullOrEmpty(swatchFile.FileName))
            {
                foreach (var old in RecentFiles.Where(rf => string.Equals(rf.FileName, swatchFile.FileName, StringComparison.OrdinalIgnoreCase)).ToList())
                {
                    RecentFiles.Remove(old);
                }

                while (RecentFiles.Count >= MaxRecentFiles)
                {
                    RecentFiles.RemoveAt(0);
                }

                RecentFiles.Add(swatchFile);
                Save();
            }
        }

        public void RemoveRecentFile(SwatchFileSummary file)
        {
            if (RecentFiles.Contains(file))
            {
                RecentFiles.Remove(file);
                Save();
            }
        }
    }
}
