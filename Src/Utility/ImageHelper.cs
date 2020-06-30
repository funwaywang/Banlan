using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Banlan
{
    public static class ImageHelper
    {
        public static Task<BitmapImage> LoadBitmapImageAsync(FileStream stream)
        {
            return Task.Factory.StartNew(() => LoadBitmapImage(stream));
        }

        public static BitmapImage LoadBitmapImage(Stream stream)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = stream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }

        public static BitmapImage BitmapSourceToImage(BitmapSource bitmapSource)
        {
            var encoder = new BmpBitmapEncoder();

            using (var memoryStream = new MemoryStream())
            {
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(memoryStream);

                memoryStream.Position = 0;

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = memoryStream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();

                return bitmap;
            }
        }
    }
}
