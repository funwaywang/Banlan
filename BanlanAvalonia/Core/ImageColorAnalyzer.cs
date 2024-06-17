using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banlan.Core
{
    //using System;
    //using System.Drawing;
    //using System.Linq;
    //using System.Net;
    //using AForge.Imaging;
    //using AForge.Imaging.Filters;
    //using AForge.Math.Geometry;
    //using Banlan.Core;

    //namespace DominantColors
    //{
    //    class Program
    //    {
    //        static void Main(string[] args)
    //        {
    //            string imagePath = "path_to_your_image.jpg"; // 图片路径
    //            int numberOfColors = 5; // 想要提取的颜色数量

    //            // 加载位图
    //            Bitmap bitmap = new Bitmap(imagePath);

    //            // 提取主要颜色
    //            Color[] dominantColors = GetDominantColors(bitmap, numberOfColors);

    //            // 输出主要颜色
    //            foreach (Color color in dominantColors)
    //            {
    //                Console.WriteLine($"Color: {color}");
    //            }
    //        }

    //        static Color[] GetDominantColors(Bitmap bitmap, int numberOfColors)
    //        {
    //            // 将图片缩小，以加速处理
    //            Bitmap smallBitmap = new ResizeBilinear(100, 100).Apply(bitmap);

    //            // 提取所有像素的颜色
    //            Color[] colors = new Color[smallBitmap.Width * smallBitmap.Height];
    //            for (int x = 0; x < smallBitmap.Width; x++)
    //            {
    //                for (int y = 0; y < smallBitmap.Height; y++)
    //                {
    //                    colors[x + y * smallBitmap.Width] = smallBitmap.GetPixel(x, y);
    //                }
    //            }

    //            // 将颜色转换为AForge.NET的IntPoint
    //            IntPoint[] points = colors.Select(c => new IntPoint(c.R, c.G, c.B)).ToArray();

    //            // K均值聚类
    //            KMeans kmeans = new KMeans(numberOfColors);
    //            KMeansClusterCollection clusters = kmeans.Learn(points);

    //            // 提取每个聚类的中心颜色
    //            Color[] resultColors = clusters.Select(cluster =>
    //            {
    //                IntPoint center = cluster.Center;
    //                return Color.FromArgb(center.X, center.Y, center.Z);
    //            }).ToArray();

    //            return resultColors;
    //        }
    //    }
    //}
}
