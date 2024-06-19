using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Banlan
{
    public class ScreenColorPicker : Control
    {
        private BitmapData? screenShot;
        private readonly Popup previewBox;
        private readonly PreviewView previewView;
        public static readonly RoutedEvent ColorPickedEvent = EventManager.RegisterRoutedEvent(nameof(ColorPicked), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ScreenColorPicker));
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(Color?), typeof(ScreenColorPicker));
        public static readonly DependencyProperty IsPickingProperty = DependencyProperty.Register(nameof(IsPicking), typeof(bool), typeof(ScreenColorPicker));

        static ScreenColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScreenColorPicker), new FrameworkPropertyMetadata(typeof(ScreenColorPicker)));
        }

        public ScreenColorPicker()
        {
            previewView = new PreviewView(this);
            previewBox = new Popup
            {
                Placement = PlacementMode.RelativePoint,
                HorizontalOffset = 16,
                VerticalOffset = 16,
                Focusable = false,
                StaysOpen = false,
                Child = previewView
            };

            Loaded += ScreenColorPicker_Loaded;
        }

        public event RoutedEventHandler ColorPicked
        {
            add => AddHandler(ColorPickedEvent, value);
            remove => RemoveHandler(ColorPickedEvent, value);
        }

        public Color? Color
        {
            get => (Color?)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public bool IsPicking
        {
            get => (bool)GetValue(IsPickingProperty);
            set => SetValue(IsPickingProperty, value);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.ChangedButton == MouseButton.Left)
            {
                Color = null;
                IsPicking = true;
                MakeScreenShot();
                CaptureMouse();
            }
            previewBox.IsOpen = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (IsPicking && e.LeftButton == MouseButtonState.Pressed)
            {
                var point = PointToScreen(e.GetPosition(this));
                Cursor = Cursors.Cross;
                Color = PickColorAt((int)point.X, (int)point.Y);
                previewView.Position = point;
                previewBox.PlacementRectangle = new Rect(point.X, point.Y, 0, 0);
                previewBox.IsOpen = true;
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            ReleaseMouseCapture();
            if (IsPicking)
            {
                IsPicking = false;
                if (Color != null)
                {
                    RaiseEvent(new RoutedEventArgs(ColorPickedEvent, Color));
                }
            }
            previewBox.IsOpen = false;
            Cursor = Cursors.Arrow;
            screenShot = null;
        }

        private void ScreenColorPicker_Loaded(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is Window window)
            {
                window.PreviewKeyDown += Window_PreviewKeyDown;
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (IsPicking && (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl) && Color != null)
            {
                AppStatus.Default.SelectColor(Color.Value);
            };
        }

        private Color? PickColorAt(int x, int y)
        {
            if (screenShot != null && screenShot.TryGetPixel(x, y, out Color? color))
            {
                return color;
            }
            else
            {
                return null;
            }
        }

        private void MakeScreenShot()
        {
            screenShot = null;

            var screen = (Window.GetTopLevel(this) as Window)?.Screens.Primary;
            if (screen != null)
            {
                var width = (int)screen.Bounds.Width;
                var height = (int)screen.Bounds.Height;
                if (width > 0 && height > 0)
                {
                    try
                    {
                        using (var bitmap = new System.Drawing.Bitmap(width, height))
                        {
                            using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
                            {
                                graphics.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(width, height), System.Drawing.CopyPixelOperation.SourceCopy);
                                screenShot = new BitmapData(bitmap);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        private class PreviewView : Control
        {
            private const int zoom = 6;
            private const int sampleSize = 9; // pixels in source image
            private readonly ScreenColorPicker screenColorPicker;
            private Point _Position;
            private IImage? SampleImage;

            public PreviewView(ScreenColorPicker screenColorPicker)
            {
                this.screenColorPicker = screenColorPicker;
            }

            public Point Position
            {
                get => _Position;
                set
                {
                    if (_Position != value)
                    {
                        _Position = value;
                        SampleImage = null;
                        InvalidateVisual();
                    }
                }
            }

            private void BuildSampleImage()
            {
                SampleImage = null;

                if (screenColorPicker?.screenShot is BitmapData bitmapData)
                {
                    var sourceWidth = Math.Min(bitmapData.Width, sampleSize) / 2 * 2 + 1;
                    var sourceHeight = Math.Min(bitmapData.Height, sampleSize) / 2 * 2 + 1;
                    if (sourceWidth > 0 && sourceHeight > 0)
                    {
                        var sourceRect = new System.Drawing.Rectangle(
                            Math.Min((int)(Position.X - sourceWidth / 2), bitmapData.Width - sourceWidth),
                            Math.Min((int)(Position.Y - sourceHeight / 2), bitmapData.Height - sourceHeight),
                            sourceWidth,
                            sourceHeight);

                        var data = bitmapData.GetClip(sourceRect);
                        if (data != null)
                        {
                            data = BitmapData.SimpleZoomIn(data, sourceWidth, sourceHeight, zoom);
                            var imageWidth = sourceWidth * zoom;
                            var imageHeight = sourceHeight * zoom;
                            var stride = BitmapData.CalculateStride(imageWidth, bitmapData.PixelBytes);

                            SampleImage = ImageHelper.CreateBitmap(imageWidth, imageHeight, 96, 96, PixelFormats.Bgra8888, data);
                            InvalidateMeasure();
                        }
                    }
                }
            }

            protected override Size MeasureCore(Size availableSize)
            {
                var width = (SampleImage?.Size.Width ?? sampleSize * zoom) + 2;
                var height = (SampleImage?.Size.Height ?? sampleSize * zoom) + 2;
                return new Size(Math.Min(width, availableSize.Width), Math.Min(height, availableSize.Height));
            }

            public override void Render(DrawingContext drawingContext)
            {
                base.Render(drawingContext);

                if (SampleImage == null)
                {
                    BuildSampleImage();
                }

                var rect = new Rect(0, 0, DesiredSize.Width, DesiredSize.Height);
                if (SampleImage != null)
                {
                    drawingContext.DrawImage(SampleImage, new Rect(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2));
                }
                else
                {
                    drawingContext.DrawRectangle(Brushes.White, null, rect);
                }

                if (zoom > 0)
                {
                    var full = new RectangleGeometry(rect);
                    var vBar = new RectangleGeometry(new Rect(rect.X + (rect.Width - zoom) / 2, rect.Y, zoom, rect.Height));
                    var hBar = new RectangleGeometry(new Rect(rect.X, rect.Y + (rect.Height - zoom) / 2, rect.Width, zoom));
                    var clipGeometry = new CombinedGeometry(GeometryCombineMode.Xor, vBar, hBar);
                    drawingContext.DrawGeometry(new SolidColorBrush(Avalonia.Media.Color.FromArgb(0x80, 0x00, 0x00, 0x00)), null, clipGeometry);
                }

                // draw border
                drawingContext.DrawRectangle(null, new Pen(Brushes.Black, 1), rect);
            }
        }
    }
}
