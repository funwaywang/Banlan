using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Data.Core;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using Banlan.Controls;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

        public static readonly RoutedEvent RangeChangedEvent = RoutedEvent.Register<RangeSelectionAdorner, RoutedEventArgs>(nameof(RangeChanged), RoutingStrategies.Bubble);
        public static readonly RoutedEvent InvalidClickEvent = RoutedEvent.Register<RangeSelectionAdorner, RoutedEventArgs>(nameof(InvalidClick), RoutingStrategies.Direct);
        public static readonly DirectProperty<RangeSelectionAdorner, Rect?> RangeProperty = AvaloniaProperty.RegisterDirect<RangeSelectionAdorner, Rect?>(nameof(Range), o => o.Range);
        public static readonly StyledProperty<Style?> HandlerStyleProperty = AvaloniaProperty.Register<RangeSelectionAdorner, Style?>(nameof(HandlerStyle));

        static RangeSelectionAdorner()
        {
            AffectsRender<RangeSelectionAdorner>(HandlerStyleProperty, RangeProperty);
        }

        public RangeSelectionAdorner(Control adornedElement)
            : base(adornedElement)
        {
            // SnapsToDevicePixels = true;

            DragHandlers = new DragHandler[]
            {
                new DragHandler { Cursor = new Cursor( StandardCursorType.TopLeftCorner) },
                new DragHandler { Cursor = new Cursor( StandardCursorType.TopSide) },
                new DragHandler { Cursor = new Cursor( StandardCursorType.TopRightCorner )},
                new DragHandler { Cursor = new Cursor( StandardCursorType.RightSide) },
                new DragHandler { Cursor = new Cursor( StandardCursorType.BottomRightCorner) },
                new DragHandler { Cursor = new Cursor( StandardCursorType.BottomSide) },
                new DragHandler { Cursor = new Cursor( StandardCursorType.BottomLeftCorner) },
                new DragHandler { Cursor = new Cursor( StandardCursorType.LeftSide) },
            };

            foreach (var handler in DragHandlers)
            {
                // handler.Bind(DragHandler.StyleProperty, new Binding { Source = this, Path = nameof(HandlerStyle), Mode = BindingMode.OneWay });
                handler.BorderPen = PenBlack;
                handler.Background = Brushes.White;
                handler.Width = HandlerSize;
                handler.Height = HandlerSize;
                handler.DragDelta += Handler_DragDelta;

                VisualChildren.Add(handler);
            }
        }

        public event EventHandler<RoutedEventArgs> RangeChanged
        {
            add => AddHandler(RangeChangedEvent, value);
            remove => RemoveHandler(RangeChangedEvent, value);
        }

        public event EventHandler<RoutedEventArgs> InvalidClick
        {
            add => AddHandler(InvalidClickEvent, value);
            remove => RemoveHandler(InvalidClickEvent, value);
        }

        public Rect? Range
        {
            get => (Rect?)GetValue(RangeProperty);
            set => SetValue(RangeProperty, value);
        }

        // protected override int VisualChildrenCount => DragHandlers?.Length ?? 0;

        public Style? HandlerStyle
        {
            get => (Style?)GetValue(HandlerStyleProperty);
            set => SetValue(HandlerStyleProperty, value);
        }

        //protected override Visual GetVisualChild(int index)
        //{
        //    if (index > -1 && index < DragHandlers.Length)
        //    {
        //        return DragHandlers[index];
        //    }

        //    return base.GetVisualChild(index);
        //}

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

        public override void Render(DrawingContext drawingContext)
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
                    //double halfPenWidth = PenBlack.Thickness / 2;
                    //var guidelines = new GuidelineSet(new double[] { rect.Left + halfPenWidth, rect.Right + halfPenWidth }, new double[] { rect.Top + halfPenWidth, rect.Bottom + halfPenWidth });
                    //drawingContext.PushGuidelineSet(guidelines);
                    drawingContext.DrawRectangle(null, PenBlack, rect);
                }
            }
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);

            var point = e.GetCurrentPoint(this);
            if (point.Properties.IsLeftButtonPressed && ImageBoxMouseDownPoint is Point downPoint)
            {
                if (Math.Abs(downPoint.X - point.Position.X) > 2 || Math.Abs(downPoint.Y - point.Position.Y) > 2)
                {
                    var adornedSize = AdornedElement.DesiredSize;

                    var left = Math.Max(0, Math.Min(point.Position.X, downPoint.X));
                    var top = Math.Max(0, Math.Min(point.Position.Y, downPoint.Y));
                    var width = Math.Abs(point.Position.X - downPoint.X);
                    var height = Math.Abs(point.Position.Y - downPoint.Y);
                    width = Math.Min(width, adornedSize.Width - left);
                    height = Math.Min(height, adornedSize.Height - top);

                    if (width > 10 || height > 10)
                    {
                        isSelectionMode = true;
                        SetRange(new Rect(left, top, width, height));
                    }
                }
            }
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            var point = e.GetCurrentPoint(this);
            if (point.Properties.IsLeftButtonPressed)
            {
                isSelectionMode = false;
                ImageBoxMouseDownPoint = e.GetPosition(this);
                // this.CaptureMouse();
            }
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);

            if (!isSelectionMode && Range != null)
            {
                SetRange(null);
            }

            isSelectionMode = false;
            ImageBoxMouseDownPoint = null;
            // ReleaseMouseCapture();
        }

        private void Handler_DragDelta(object? sender, VectorEventArgs e)
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
                        left += e.Vector.X;
                        top += e.Vector.Y;
                        break;
                    case 1:
                        top += e.Vector.Y;
                        break;
                    case 2:
                        right += e.Vector.X;
                        top += e.Vector.Y;
                        break;
                    case 3:
                        left += e.Vector.X;
                        break;
                    case 4:
                        right += e.Vector.X;
                        break;
                    case 5:
                        left += e.Vector.X;
                        bottom += e.Vector.Y;
                        break;
                    case 6:
                        bottom += e.Vector.Y;
                        break;
                    case 7:
                        right += e.Vector.X;
                        bottom += e.Vector.Y;
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

            public Pen? BorderPen { get; set; }

            //public Rect? Bounds { get; private set; }

            public override void Render(DrawingContext drawingContext)
            {
                var rect = new Rect(0, 0, Bounds.Width, Bounds.Height);
                if (rect.Width > 0 && rect.Height > 0)
                {
                    //if (BorderPen != null && BorderPen.Thickness > 0)
                    //{
                    //    double halfPenWidth = BorderPen.Thickness / 2;
                    //    var guidelines = new GuidelineSet(new double[] { rect.X + halfPenWidth, rect.Right + halfPenWidth },
                    //        new double[] { rect.Top + halfPenWidth, rect.Bottom + halfPenWidth });
                    //    drawingContext.PushGuidelineSet(guidelines);
                    //}

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
