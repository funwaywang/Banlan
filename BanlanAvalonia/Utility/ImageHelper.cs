using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;

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
