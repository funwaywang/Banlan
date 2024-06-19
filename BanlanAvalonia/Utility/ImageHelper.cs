using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Banlan
{
    public static class ImageHelper
    {
        public static Task<Bitmap> LoadBitmapImageAsync(FileStream stream)
        {
            return Task.Factory.StartNew(() => LoadBitmapImage(stream));
        }

        public static Bitmap LoadBitmapImage(Stream stream)
        {
            var bitmap = new Bitmap(stream);
            //bitmap.BeginInit();
            //bitmap.StreamSource = stream;
            //bitmap.CacheOption = BitmapCacheOption.OnLoad;
            //bitmap.EndInit();
            //bitmap.Freeze();
            return bitmap;
        }

        public static Bitmap CreateBitmap(int width, int height, int dpiX, int dpiY, PixelFormat pixelFormat, byte[] data)
        { 
            // Standard may need to change on some devices 
            Vector dpi = new Vector(dpiX, dpiY);

            var bitmap = new WriteableBitmap(
                new PixelSize(width, height),
                dpi,
                pixelFormat,
                AlphaFormat.Premul);

            using (var frameBuffer = bitmap.Lock())
            {
                Marshal.Copy(data, 0, frameBuffer.Address, data.Length);
            }

            return bitmap;
        }

        //public static Bitmap BitmapSourceToImage(BitmapSource bitmapSource)
        //{
        //    var encoder = new BmpBitmapEncoder();

        //    using (var memoryStream = new MemoryStream())
        //    {
        //        encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
        //        encoder.Save(memoryStream);

        //        memoryStream.Position = 0;

        //        var bitmap = new BitmapImage();
        //        bitmap.BeginInit();
        //        bitmap.StreamSource = memoryStream;
        //        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        //        bitmap.EndInit();
        //        bitmap.Freeze();

        //        return bitmap;
        //    }
        //}
    }
}
