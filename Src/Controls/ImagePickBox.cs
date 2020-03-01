using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Banlan
{
    public class ImagePickBox : Control
    {
        private Image PreviewImageBox;
        private RangeSelectionAdorner rangeSelectionAdorner;
        private PickHandlersAdorner pickHandlersAdorner;
        private Point? ImageBoxMouseDownPoint = null;
        private bool isSelectionMode = false;
        public static readonly RoutedEvent PointMovedEvent = EventManager.RegisterRoutedEvent(nameof(PointMoved), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ImagePickBox));
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(nameof(Image), typeof(BitmapImage), typeof(ImagePickBox));
        public static readonly DependencyProperty ColorPointsProperty = DependencyProperty.Register(nameof(ColorPoints), typeof(IEnumerable<ColorPoint>), typeof(ImagePickBox));
        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(nameof(Zoom), typeof(double), typeof(ImagePickBox), new PropertyMetadata(1.0), new ValidateValueCallback(v => v is double zoom && zoom > 0.0));
        public static readonly DependencyProperty SelectedPointProperty = DependencyProperty.Register(nameof(SelectedPoint), typeof(ColorPoint), typeof(ImagePickBox),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty SelectedRangeProperty = DependencyProperty.Register(nameof(SelectedRange), typeof(Rect?), typeof(ImagePickBox));
        public static readonly DependencyProperty PickHandlerStyleProperty = DependencyProperty.Register(nameof(PickHandlerStyle), typeof(Style), typeof(ImagePickBox));

        static ImagePickBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImagePickBox), new FrameworkPropertyMetadata(typeof(ImagePickBox)));
        }

        public ImagePickBox()
        {
            AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(Thumb_DragDelta));
            Loaded += ImagePickBox_Loaded;
        }

        private void ImagePickBox_Loaded(object sender, RoutedEventArgs e)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(PreviewImageBox);
            if (adornerLayer != null)
            {
                if (rangeSelectionAdorner != null && rangeSelectionAdorner.Parent == null)
                {
                    adornerLayer.Add(rangeSelectionAdorner);
                }
                if (pickHandlersAdorner != null && pickHandlersAdorner.Parent == null)
                {
                    adornerLayer.Add(pickHandlersAdorner);
                }
            }
        }

        public event RoutedEventHandler PointMoved
        {
            add => AddHandler(PointMovedEvent, value);
            remove => RemoveHandler(PointMovedEvent, value);
        }

        public BitmapImage Image
        {
            get => (BitmapImage)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        public IEnumerable<ColorPoint> ColorPoints
        {
            get => (IEnumerable<ColorPoint>)GetValue(ColorPointsProperty);
            set => SetValue(ColorPointsProperty, value);
        }

        public double Zoom
        {
            get => (double)GetValue(ZoomProperty);
            set => SetValue(ZoomProperty, value);
        }

        public Style PickHandlerStyle
        {
            get => (Style)GetValue(PickHandlerStyleProperty);
            set => SetValue(PickHandlerStyleProperty, value);
        }

        public ColorPoint SelectedPoint
        {
            get => (ColorPoint)GetValue(SelectedPointProperty);
            set => SetValue(SelectedPointProperty, value);
        }

        public Rect? SelectedRange
        {
            get => (Rect?)GetValue(SelectedRangeProperty);
            set => SetValue(SelectedRangeProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PreviewImageBox = GetTemplateChild("PART_PreviewBox") as Image;
            if (PreviewImageBox != null)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(PreviewImageBox);
                if (adornerLayer != null)
                {
                    rangeSelectionAdorner = new RangeSelectionAdorner(PreviewImageBox)
                    {
                        HandlerStyle = TryFindResource("SizingHandlerStyle") as Style
                    };
                    rangeSelectionAdorner.SetBinding(VisibilityProperty, new Binding
                    {
                        Source = this,
                        Path = new PropertyPath(nameof(SelectedRange)),
                        Converter = new NullToVisibilityConverter()
                    });
                    rangeSelectionAdorner.RangeChanged += RangeSelectionAdorner_RangeChanged;
                    adornerLayer.Add(rangeSelectionAdorner);

                    pickHandlersAdorner = new PickHandlersAdorner(PreviewImageBox, this);
                    pickHandlersAdorner.SetBinding(PickHandlersAdorner.ColorPointsProperty, new Binding
                    {
                        Source = this,
                        Path = new PropertyPath(nameof(ColorPoints)),
                        Mode = BindingMode.OneWay
                    });
                    adornerLayer.Add(pickHandlersAdorner);
                }

                PreviewImageBox.MouseDown += PreviewImageBox_MouseDown;
                PreviewImageBox.MouseMove += PreviewImageBox_MouseMove;
                PreviewImageBox.MouseUp += PreviewImageBox_MouseUp;
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == ZoomProperty)
            {
                if (rangeSelectionAdorner != null
                    && e.NewValue is double zoom
                    && SelectedRange is Rect rect)
                {
                    rangeSelectionAdorner.Range = rect.Zoom(Zoom);
                }
            }
        }

        private void PreviewImageBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && ImageBoxMouseDownPoint is Point downPoint)
            {
                var point = e.GetPosition(PreviewImageBox);
                if (!isSelectionMode)
                {
                    SelectedRange = null;
                }

                if (Math.Abs(downPoint.X - point.X) > 2 || Math.Abs(downPoint.Y - point.Y) > 2)
                {
                    var adornedSize = PreviewImageBox.DesiredSize;
                    var rect = new Rect(Math.Max(0, Math.Min(point.X, downPoint.X)),
                        Math.Max(0, Math.Min(point.Y, downPoint.Y)),
                        Math.Abs(point.X - downPoint.X),
                        Math.Abs(point.Y - downPoint.Y));
                    rect.Width = Math.Min(rect.Width, adornedSize.Width - rect.Left);
                    rect.Height = Math.Min(rect.Height, adornedSize.Height - rect.Top);
                    if (rect.Width > 10 || rect.Height > 10)
                    {
                        isSelectionMode = true;
                        SelectedRange = rect.Unzoom(Zoom);
                        if (rangeSelectionAdorner != null)
                        {
                            rangeSelectionAdorner.Range = rect;
                        }
                    }
                }
            }
        }

        private void PreviewImageBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                isSelectionMode = false;
                ImageBoxMouseDownPoint = e.GetPosition(PreviewImageBox);
                PreviewImageBox.CaptureMouse();
            }
        }

        private void PreviewImageBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!isSelectionMode && SelectedRange != null)
            {
                SelectedRange = null;
            }

            isSelectionMode = false;
            ImageBoxMouseDownPoint = null;
            PreviewImageBox.ReleaseMouseCapture();
        }

        private void RangeSelectionAdorner_RangeChanged(object sender, RoutedEventArgs e)
        {
            if (rangeSelectionAdorner.Range is Rect rect)
            {
                SelectedRange = rect.Unzoom(Zoom);
            }
            else
            {
                SelectedRange = null;
            }
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Image != null && (e.OriginalSource as Thumb)?.DataContext is ColorPoint colorPoint)
            {
                var x = Math.Max(0, Math.Min(Image.PixelWidth, colorPoint.X + e.HorizontalChange / Zoom));
                var y = Math.Max(0, Math.Min(Image.PixelHeight, colorPoint.Y + e.VerticalChange / Zoom));

                if (SelectedRange is Rect rect)
                {
                    x = Math.Max(rect.X, Math.Min(rect.Right, x));
                    y = Math.Max(rect.Y, Math.Min(rect.Bottom, y));
                }

                if (x != colorPoint.X || y != colorPoint.Y)
                {
                    colorPoint.X = x;
                    colorPoint.Y = y;

                    var args = new RoutedEventArgs(PointMovedEvent, colorPoint);
                    RaiseEvent(args);

                    pickHandlersAdorner?.InvalidateArrange();
                }
            }
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            if (PreviewImageBox != null && Image != null)
            {
                var zoom = Math.Min(PreviewImageBox.DesiredSize.Width / Image.PixelWidth, PreviewImageBox.DesiredSize.Height / Image.PixelHeight);
                Zoom = zoom > 0 ? zoom : 1.0;
            }
            else
            {
                Zoom = 1.0;
            }

            return base.ArrangeOverride(arrangeBounds);
        }

        private class PickHandlersAdorner : Adorner
        {
            private readonly ImagePickBox parent;
            private readonly List<Thumb> thumbs = new List<Thumb>();
            public static readonly DependencyProperty ColorPointsProperty = ImagePickBox.ColorPointsProperty.AddOwner(typeof(PickHandlersAdorner));

            public PickHandlersAdorner(UIElement adornedElement, ImagePickBox parent)
                : base(adornedElement)
            {
                this.parent = parent;
            }

            public IEnumerable<ColorPoint> ColorPoints
            {
                get => (IEnumerable<ColorPoint>)GetValue(ColorPointsProperty);
                set => SetValue(ColorPointsProperty, value);
            }

            protected override int VisualChildrenCount => thumbs.Count;

            protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
            {
                base.OnPropertyChanged(e);

                if (e.Property == ColorPointsProperty)
                {
                    RebuildHandlers();

                    if (e.OldValue is ObservableCollection<ColorPoint> oldValue)
                    {
                        oldValue.CollectionChanged -= ColorPoints_CollectionChanged;
                    }

                    if (e.NewValue is ObservableCollection<ColorPoint> newValue)
                    {
                        newValue.CollectionChanged += ColorPoints_CollectionChanged;
                    }
                }
            }

            private void RebuildHandlers()
            {
                ClearThumbs();
                if (ColorPoints != null)
                {
                    AddThumbs(ColorPoints);
                }
                InvalidateVisual();
            }

            private void ClearThumbs()
            {
                thumbs.ForEach(t => RemoveVisualChild(t));
                thumbs.Clear();
            }

            private void RemoveThumbs(IEnumerable<ColorPoint> colorPoints)
            {
                foreach (var colorPoint in colorPoints)
                {
                    var thumb = thumbs.FirstOrDefault(t => t.DataContext == colorPoint);
                    if (thumb != null)
                    {
                        thumbs.Remove(thumb);
                        RemoveVisualChild(thumb);
                    }
                }
            }

            private void AddThumbs(IEnumerable<ColorPoint> colorPoints)
            {
                foreach (var colorPoint in colorPoints)
                {
                    var thumb = new Thumb
                    {
                        DataContext = colorPoint,
                        Style = parent.PickHandlerStyle
                    };
                    thumb.DragDelta += Thumb_DragDelta;
                    thumbs.Add(thumb);
                    AddVisualChild(thumb);
                }
            }

            private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
            {
                parent.Thumb_DragDelta(sender, e);
            }

            private void ColorPoints_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    ClearThumbs();
                }
                else
                {
                    if (e.OldItems != null)
                    {
                        RemoveThumbs(e.OldItems.OfType<ColorPoint>());
                    }

                    if (e.NewItems != null)
                    {
                        AddThumbs(e.NewItems.OfType<ColorPoint>());
                    }
                }
                InvalidateVisual();
            }

            protected override Visual GetVisualChild(int index)
            {
                if (index > -1 && index < thumbs.Count)
                {
                    return thumbs[index];
                }

                return base.GetVisualChild(index);
            }

            protected override Size MeasureOverride(Size constraint)
            {
                foreach (var thumb in thumbs)
                {
                    thumb.Measure(constraint);
                }
                return base.MeasureOverride(constraint);
            }

            protected override Size ArrangeOverride(Size finalSize)
            {
                foreach (var thumb in thumbs)
                {
                    if (thumb.DataContext is ColorPoint colorPoint)
                    {
                        var size = thumb.DesiredSize;
                        var rect = new Rect(colorPoint.X * parent.Zoom - size.Width / 2, colorPoint.Y * parent.Zoom - size.Height / 2, size.Width, size.Height);
                        thumb.Arrange(rect);
                    }
                }

                return base.ArrangeOverride(finalSize);
            }
        }
    }
}
