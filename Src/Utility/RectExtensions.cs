using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Banlan
{
    public static class RectExtensions
    {
        public static Rect Zoom(this Rect rect, double zoom)
        {
            return new Rect(rect.X * zoom, rect.Y * zoom, rect.Width * zoom, rect.Height * zoom);
        }

        public static Rect Unzoom(this Rect rect, double zoom)
        {
            if (zoom > 0)
                return new Rect(rect.X / zoom, rect.Y / zoom, rect.Width / zoom, rect.Height / zoom);
            else
                throw new ArgumentOutOfRangeException(nameof(zoom));
        }
    }
}
