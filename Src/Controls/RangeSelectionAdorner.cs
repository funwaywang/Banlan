using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Banlan
{
    public class RangeSelectionAdorner : Adorner
    {
        private readonly Pen PenBlack = new Pen(new SolidColorBrush(Color.FromArgb(0x80, 0x00, 0x00, 0x00)), 1);
        private readonly Brush BrushMask = new SolidColorBrush(Color.FromArgb(0x80, 0xff, 0xff, 0xff));
        private const double HandlerSize = 8;
        private readonly DragHandler[] DragHandlers;
        private bool isSelectionMode;
        private Point? ImageBoxMouseDownPoint;

        public static readonly RoutedEvent RangeChangedEvent = EventManager.RegisterRoutedEvent(nameof(RangeChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RangeSelectionAdorner));
        public static readonly RoutedEvent InvalidClickEvent = EventManager.RegisterRoutedEvent(nameof(InvalidClick), RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(RangeSelectionAdorner));
        public static readonly DependencyProperty RangeProperty = DependencyProperty.Register(nameof(Range), typeof(Rect?), typeof(RangeSelectionAdorner), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty HandlerStyleProperty = DependencyProperty.Register(nameof(HandlerStyle), typeof(Style), typeof(RangeSelectionAdorner), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public RangeSelectionAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            SnapsToDevicePixels = true;

            DragHandlers = new DragHandler[]
            {
                new DragHandler { Cursor = Cursors.SizeNWSE },
                new DragHandler { Cursor = Cursors.SizeNS },
                new DragHandler { Cursor = Cursors.SizeNESW },
                new DragHandler { Cursor = Cursors.SizeWE },
                new DragHandler { Cursor = Cursors.SizeWE },
                new DragHandler { Cursor = Cursors.SizeNESW },
                new DragHandler { Cursor = Cursors.SizeNS },
                new DragHandler { Cursor = Cursors.SizeNWSE },
            };

            foreach (var handler in DragHandlers)
            {
                handler.SetBinding(StyleProperty, new Binding { Source = this, Path = new PropertyPath(nameof(HandlerStyle)), Mode = BindingMode.OneWay });
                handler.BorderPen = PenBlack;
                handler.Background = Brushes.White;
                handler.Width = HandlerSize;
                handler.Height = HandlerSize;
                handler.DragDelta += Handler_DragDelta;
                AddVisualChild(handler);
            }
        }

        public event RoutedEventHandler RangeChanged
        {
            add => AddHandler(RangeChangedEvent, value);
            remove => RemoveHandler(RangeChangedEvent, value);
        }

        public event RoutedEventHandler InvalidClick
        {
            add => AddHandler(InvalidClickEvent, value);
            remove => RemoveHandler(InvalidClickEvent, value);
        }

        public Rect? Range
        {
            get => (Rect?)GetValue(RangeProperty);
            set => SetValue(RangeProperty, value);
        }

        protected override int VisualChildrenCount => DragHandlers?.Length ?? 0;

        public Style HandlerStyle
        {
            get => (Style)GetValue(HandlerStyleProperty);
            set => SetValue(HandlerStyleProperty, value);
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index > -1 && index < DragHandlers.Length)
            {
                return DragHandlers[index];
            }

            return base.GetVisualChild(index);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            foreach (var handler in DragHandlers)
            {
                handler.Measure(new Size(HandlerSize, HandlerSize));
            }

            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Range is Rect rect)
            {
                var xl = rect.Left;
                var xr = rect.Right;
                var xm = xl + (xr - xl) / 2;
                var yt = rect.Top;
                var yb = rect.Bottom;
                var ym = yt + (yb - yt) / 2;
                var hh = HandlerSize / 2;
                DragHandlers[0].ArrangeBounds(new Rect(xl - hh, yt - hh, HandlerSize, HandlerSize));
                DragHandlers[1].ArrangeBounds(new Rect(xm - hh, yt - hh, HandlerSize, HandlerSize));
                DragHandlers[2].ArrangeBounds(new Rect(xr - hh, yt - hh, HandlerSize, HandlerSize));
                DragHandlers[3].ArrangeBounds(new Rect(xl - hh, ym - hh, HandlerSize, HandlerSize));
                DragHandlers[4].ArrangeBounds(new Rect(xr - hh, ym - hh, HandlerSize, HandlerSize));
                DragHandlers[5].ArrangeBounds(new Rect(xl - hh, yb - hh, HandlerSize, HandlerSize));
                DragHandlers[6].ArrangeBounds(new Rect(xm - hh, yb - hh, HandlerSize, HandlerSize));
                DragHandlers[7].ArrangeBounds(new Rect(xr - hh, yb - hh, HandlerSize, HandlerSize));
            }
            else
            {
                foreach (var handler in DragHandlers)
                {
                    handler.ArrangeBounds(new Rect());
                }
            }

            return base.ArrangeOverride(finalSize);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (Range is Rect rect)
            {
                var adornedSize = AdornedElement.DesiredSize;
                // draw mask
                if (rect.Width > 0 && rect.Height > 0)
                {
                    var full = new RectangleGeometry(new Rect(0, 0, adornedSize.Width, adornedSize.Height));
                    var exclude = new RectangleGeometry(rect);
                    var clipGeometry = new CombinedGeometry(GeometryCombineMode.Exclude, full, exclude);
                    drawingContext.DrawGeometry(BrushMask, null, clipGeometry);
                }
                else
                {
                    drawingContext.DrawRectangle(BrushMask, null, new Rect(0, 0, adornedSize.Width, adornedSize.Height));
                }

                // draw bounds
                if (rect.Width > 2 && rect.Height > 2)
                {
                    double halfPenWidth = PenBlack.Thickness / 2;
                    var guidelines = new GuidelineSet(new double[] { rect.Left + halfPenWidth, rect.Right + halfPenWidth }, new double[] { rect.Top + halfPenWidth, rect.Bottom + halfPenWidth });
                    drawingContext.PushGuidelineSet(guidelines);
                    drawingContext.DrawRectangle(null, PenBlack, rect);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton == MouseButtonState.Pressed && ImageBoxMouseDownPoint is Point downPoint)
            {
                var point = e.GetPosition(this);
                if (Math.Abs(downPoint.X - point.X) > 2 || Math.Abs(downPoint.Y - point.Y) > 2)
                {
                    var adornedSize = AdornedElement.DesiredSize;
                    var rect = new Rect(Math.Max(0, Math.Min(point.X, downPoint.X)),
                        Math.Max(0, Math.Min(point.Y, downPoint.Y)),
                        Math.Abs(point.X - downPoint.X),
                        Math.Abs(point.Y - downPoint.Y));
                    rect.Width = Math.Min(rect.Width, adornedSize.Width - rect.Left);
                    rect.Height = Math.Min(rect.Height, adornedSize.Height - rect.Top);
                    if (rect.Width > 10 || rect.Height > 10)
                    {
                        isSelectionMode = true;
                        SetRange(rect);
                    }
                }
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.ChangedButton == MouseButton.Left)
            {
                isSelectionMode = false;
                ImageBoxMouseDownPoint = e.GetPosition(this);
                CaptureMouse();
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (!isSelectionMode && Range != null)
            {
                SetRange(null);
            }

            isSelectionMode = false;
            ImageBoxMouseDownPoint = null;
            ReleaseMouseCapture();
        }

        private void Handler_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (e.Source is DragHandler handler && Range is Rect rect)
            {
                var size = AdornedElement.DesiredSize;
                var left = rect.Left;
                var top = rect.Top;
                var right = rect.Right;
                var bottom = rect.Bottom;
                switch (Array.IndexOf(DragHandlers, handler))
                {
                    case 0:
                        left += e.HorizontalChange;
                        top += e.VerticalChange;
                        break;
                    case 1:
                        top += e.VerticalChange;
                        break;
                    case 2:
                        right += e.HorizontalChange;
                        top += e.VerticalChange;
                        break;
                    case 3:
                        left += e.HorizontalChange;
                        break;
                    case 4:
                        right += e.HorizontalChange;
                        break;
                    case 5:
                        left += e.HorizontalChange;
                        bottom += e.VerticalChange;
                        break;
                    case 6:
                        bottom += e.VerticalChange;
                        break;
                    case 7:
                        right += e.HorizontalChange;
                        bottom += e.VerticalChange;
                        break;
                }

                if (left != rect.Left) left = Math.Max(0, Math.Min(left, right - 1));
                if (top != rect.Top) top = Math.Max(0, Math.Min(top, bottom - 1));
                if (right != rect.Right) right = Math.Min(Math.Max(left + 1, right), size.Width);
                if (bottom != rect.Bottom) bottom = Math.Min(Math.Max(top + 1, bottom), size.Height);

                var rect2 = new Rect(left, top, Math.Max(0, right - left), Math.Max(0, bottom - top));
                if (Range == null || Range.Value != rect2)
                {
                    SetRange(rect2);
                }
            }
        }

        private void SetRange(Rect? rect)
        {
            Range = rect;
            RaiseEvent(new RoutedEventArgs(RangeChangedEvent));
        }

        private class DragHandler : Thumb
        {
            public DragHandler()
            {
                Focusable = false;
                Template = null;
            }

            public Pen BorderPen { get; set; }

            public Rect Bounds { get; private set; }

            protected override void OnRender(DrawingContext drawingContext)
            {
                var rect = new Rect(0, 0, ActualWidth, ActualHeight);
                if (rect.Width > 0 && rect.Height > 0)
                {
                    if (BorderPen != null && BorderPen.Thickness > 0)
                    {
                        double halfPenWidth = BorderPen.Thickness / 2;
                        var guidelines = new GuidelineSet(new double[] { rect.X + halfPenWidth, rect.Right + halfPenWidth },
                            new double[] { rect.Top + halfPenWidth, rect.Bottom + halfPenWidth });
                        drawingContext.PushGuidelineSet(guidelines);
                    }

                    drawingContext.DrawRectangle(Brushes.White, BorderPen, rect);
                }
            }

            public void ArrangeBounds(Rect rect)
            {
                Bounds = rect;
                Arrange(rect);
                InvalidateVisual();
            }
        }
    }
}
