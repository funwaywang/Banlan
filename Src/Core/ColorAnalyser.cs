using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Banlan.Core
{
    /// <summary>
    /// reference to https://github.com/dcollien/Dreamcoat
    /// </summary>
    class ColorAnalyser
    {
        private readonly Octree octree;
        private readonly BitmapData image;

        public ColorAnalyser(BitmapData bitmapData, Rectangle? range = null, int maxColorBits = 8)
        {
            octree = new Octree(maxColorBits);
            image = range != null ? new ClipBitmapData(bitmapData, range.Value) : bitmapData;
            Range = range;
        }

        public Rectangle? Range { get; private set; }

        public Color? BackgroundColor { get; private set; }

        public bool TryGetPixel(int x, int y, out Color? color)
        {
            return image.TryGetPixel(x, y, out color);
        }

        public Color DetectBackground()
        {
            if (BackgroundColor == null)
            {
                var top = Enumerable.Range(0, image.Width).Select(x => image.GetPixel(x, 0)).ToList();
                var bottom = Enumerable.Range(0, image.Width).Select(x => image.GetPixel(x, image.Height - 1)).ToList();
                var left = Enumerable.Range(0, image.Height).Select(y => image.GetPixel(0, y)).ToList();
                var right = Enumerable.Range(0, image.Height).Select(y => image.GetPixel(image.Width - 1, y)).ToList();

                BackgroundColor = (from c in top.Concat(bottom).Concat(left).Concat(right)
                                   group c by c into g
                                   orderby g.Count() descending
                                   select g.Key).First();
            }

            return BackgroundColor.Value;
        }

        public Color ChooseTextColor(Color backgroundColor, IEnumerable<Palette> palette)
        {
            if (palette == null)
            {
                palette = null;
            }

            (var h, var s, var l) = ColorHelper.RgbToHsl(backgroundColor.R, backgroundColor.G, backgroundColor.B);
            h += 0.5f;
            if (h > 1)
            {
                h -= 1;
            }
            s = (1 - s) * 0.25f;
            l = 1 - l;
            if (l < 0.5)
            {
                l = -l + 0.5f;
            }
            else if (l > 0.5)
            {
                l = -l + 1.5f;
            }
            else
            {
                l = 1;
            }

            if (palette != null && palette.Any())
            {
                var maxPalette = palette.OrderByDescending(p => p.Count).First();
                var modeColor = maxPalette.Mean;
                var modeCount = maxPalette.Count;
                (var mh, var ms, var ml) = ColorHelper.RgbToHsl((byte)modeColor[0], (byte)modeColor[1], (byte)modeColor[2]);
                s = Math.Min(s, ms);
                h = Lerp(0.75f, h, mh);
            }

            var color = ColorHelper.HslToRgb(h, s, l);
            return Color.FromArgb(color.r, color.g, color.b);
        }

        private float Lerp(float t, float from, float to)
        {
            return t * to + (1 - t) * from;
        }

        public Palette[] AnalyseImage(int paletteSize, Color? background = null, bool ignoreGrey = false)
        {
            if (background == null)
            {
                background = DetectBackground();
            }

            return GetClusteredPalette(paletteSize, 1, 1024, background, 32, ignoreGrey).palettes?.ToArray();
        }

        private (List<Palette> colors, int numVectors) GetPalette(int paletteSize, Color? exclude, int error = 0, bool ignoreGrey = false)
        {
            foreach (var cp in image.GetColors())
            {
                var isExcluded = exclude != null
                        && exclude.Value.R - error < cp.color.R && cp.color.R < (exclude.Value.R + error)
                        && exclude.Value.G - error < cp.color.G && cp.color.G < (exclude.Value.G + error)
                        && exclude.Value.B - error < cp.color.B && cp.color.B < (exclude.Value.B + error);
                if (!isExcluded)
                {
                    var excludeGrey = ignoreGrey
                        && Math.Abs(cp.color.R - cp.color.G) < error
                        && Math.Abs(cp.color.R - cp.color.B) < error
                        && Math.Abs(cp.color.G - cp.color.B) < error;
                    if (!excludeGrey)
                    {
                        octree.InsertVector(new byte[] { cp.color.R, cp.color.G, cp.color.B }, cp.position);
                    }
                }
            }

            var numVectors = octree.NumVectors;
            return (octree.ReduceToSize(paletteSize), numVectors);
        }

        public Point? GetPoint(int position)
        {
            return image.GetPoint(position);
        }

        private (List<Palette> palettes, int numVectors) GetThresholdedPalette(int threshold, int paletteSize, Color? exclude, int error, bool ignoreGrey)
        {
            (var palettes, var numVectors) = GetPalette(paletteSize, exclude, error, ignoreGrey);
            palettes.Sort((a, b) => b.Count.CompareTo(a.Count));
            var newPalettes = new List<Palette>();
            var sum = 0;
            foreach (var color in palettes)
            {
                newPalettes.Add(color);
                sum += color.Count;
                if (sum > (numVectors * threshold))
                {
                    break;
                }
            }

            return (newPalettes, numVectors);
        }

        private (List<Palette> palettes, int numVectors) GetFilteredPalette(double stdDeviations, int paletteSize, Color? exclude, int error, bool ignoreGrey)
        {
            (var palettes, var numVectors) = GetPalette(paletteSize, exclude, error, ignoreGrey);
            var numColors = palettes.Count;
            var freqSum = palettes.Sum(c => c.Count);
            var meanFrequency = freqSum / numColors;
            var stdDevSum = palettes.Sum(c => (c.Count - meanFrequency) * (c.Count - meanFrequency));
            var stdDevFrequency = Math.Sqrt(stdDevSum / numColors);

            var filteredColors = (from c in palettes
                                  where Math.Abs(c.Count - meanFrequency) < (stdDevFrequency * stdDeviations)
                                  select new Palette(c.Mean, c.Count, c.Position)).ToList();
            return (filteredColors, numVectors);
        }

        private (List<Palette> palettes, int numVectors) GetClusteredPalette(int numClusters, int threshold, int paletteSize, Color? exclude, int error, bool ignoreGrey)
        {
            (var palettes, var numVectors) = GetThresholdedPalette(threshold, paletteSize, exclude, error, ignoreGrey);
            if (palettes.Any())
            {
                var clusterer = new KMeans(numClusters, 3);
                clusterer.SetPoints(palettes);
                var clusters = clusterer.PerformCluster();
                var colors = (from c in clusters
                              select new Palette(c.GetMean(), c.size, c.Position)).ToList();
                return (colors, numVectors);
            }
            else
            {
                return (null, 0);
            }
        }
    }
}
