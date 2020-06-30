using Banlan.Core;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Banlan
{
    public partial class ExtractImageColorsView : DocumentView
    {
        private ColorAnalyser analyser;
        private readonly BitmapData ImageData;
        private const string ImagesFilter = "All Images (*.jpg;*.png;*.gif;*.bmp;*.tiff)|*.jpg;*.jpeg;*.jfif;*.png;*.gif;*.bmp;*.tiff;|All Files (*.*)|*.*";
        public static DependencyProperty ColorsNumberProperty = DependencyProperty.Register(nameof(ColorsNumber), typeof(int), typeof(ExtractImageColorsView), new PropertyMetadata(5));
        public static DependencyProperty IsAnalysingProperty = DependencyProperty.Register(nameof(IsAnalysing), typeof(bool), typeof(ExtractImageColorsView), new PropertyMetadata(false));
        public static DependencyProperty PreviewImageProperty = DependencyProperty.Register(nameof(PreviewImage), typeof(BitmapImage), typeof(ExtractImageColorsView));
        public static readonly DependencyProperty SelectedPointProperty = DependencyProperty.Register(nameof(SelectedPoint), typeof(ColorPoint), typeof(ExtractImageColorsView),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty SelectedRangeProperty = DependencyProperty.Register(nameof(SelectedRange), typeof(Rect?), typeof(ExtractImageColorsView));
        public static readonly DependencyProperty ColorTextFormatterProperty = DependencyProperty.Register(nameof(ColorTextFormatter), typeof(IColorTextFormatter), typeof(ExtractImageColorsView));
        public static readonly DependencyProperty BackColorProperty = DependencyProperty.Register(nameof(BackColor), typeof(ColorViewModel), typeof(ExtractImageColorsView));
        public static readonly DependencyProperty ForeColorProperty = DependencyProperty.Register(nameof(ForeColor), typeof(ColorViewModel), typeof(ExtractImageColorsView));

        public ExtractImageColorsView()
        {
            InitializeComponent();

            Loaded += ExtractImageColorsView_Loaded;
            SetBinding(TitleProperty, new Binding(nameof(FileName)) { Source = this, Converter = new PathToNameConverter(), Mode = BindingMode.OneWay });
            SetBinding(ColorTextFormatterProperty, new Binding(nameof(AppStatus.SelectedFormatter)) { Source = AppStatus.Default, Mode = BindingMode.OneWay });
        }

        public ExtractImageColorsView(BitmapData imageData, BitmapImage previewImage, string filename = null)
            : this()
        {
            ImageData = imageData;
            PreviewImage = previewImage;
            FileName = filename;
        }

        public bool IsAnalysing
        {
            get => (bool)GetValue(IsAnalysingProperty);
            set => SetValue(IsAnalysingProperty, value);
        }

        public int ColorsNumber
        {
            get => (int)GetValue(ColorsNumberProperty);
            set => SetValue(ColorsNumberProperty, value);
        }

        public BitmapImage PreviewImage
        {
            get => (BitmapImage)GetValue(PreviewImageProperty);
            private set => SetValue(PreviewImageProperty, value);
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

        public IColorTextFormatter ColorTextFormatter
        {
            get => (IColorTextFormatter)GetValue(ColorTextFormatterProperty);
            set => SetValue(ColorTextFormatterProperty, value);
        }

        public ColorViewModel BackColor
        {
            get => (ColorViewModel)GetValue(BackColorProperty);
            set => SetValue(BackColorProperty, value);
        }

        public ColorViewModel ForeColor
        {
            get => (ColorViewModel)GetValue(ForeColorProperty);
            set => SetValue(ForeColorProperty, value);
        }

        public ObservableCollection<ColorPoint> ColorPoints { get; private set; } = new ObservableCollection<ColorPoint>();

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == ColorTextFormatterProperty)
            {
                RefreshColorTexts();
            }
            else if (e.Property == SelectedPointProperty)
            {
                if (e.NewValue is ColorPoint colorPoint && colorPoint.Color != null)
                {
                    AppStatus.Default.SelectColor(colorPoint.Color);
                }
            }
        }

        private void RefreshColorTexts()
        {
            if (ColorTextFormatter is IColorTextFormatter formatter)
            {
                foreach (var point in ColorPoints)
                {
                    point.ViewModel.Text = formatter.Format(point.Color);
                }
            }
        }

        private void ExtractImageColorsView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsAnalysing && analyser == null)
            {
                AnalyseImage();
            }
        }

        public async void AnalyseImage()
        {
            if (IsAnalysing || ImageData == null)
            {
                return;
            }

            try
            {
                System.Drawing.Rectangle? range = null;
                if (SelectedRange is Rect rect)
                {
                    range = new System.Drawing.Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
                }

                IsAnalysing = true;
                await AnalyseImageAsync(ColorsNumber, range);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Window.GetWindow(this), ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsAnalysing = false;
            }
        }

        private Task AnalyseImageAsync(int colorsNumber, System.Drawing.Rectangle? range)
        {
            return Task.Factory.StartNew(() => AnalyseImage(colorsNumber, range));
        }

        private void AnalyseImage(int colorsNumber, System.Drawing.Rectangle? range)
        {
            if (ImageData == null)
            {
                return;
            }

            if (analyser == null || analyser.Range != range)
            {
                analyser = new ColorAnalyser(ImageData, range);
            }

            var colors = Math.Max(1, Math.Min(100, colorsNumber));
            var palettes = analyser.AnalyseImage(colors);
            var backColor = analyser.BackgroundColor;
            var foreColor = backColor.HasValue ? analyser.ChooseTextColor(backColor.Value, palettes) : (System.Drawing.Color?)null;
            this.RunInUIThread(() =>
            {
                SetResult(palettes, backColor, foreColor);
            });
        }

        private void SetResult(Palette[] palettes, System.Drawing.Color? backColor, System.Drawing.Color? foreColor)
        {
            ColorPoints.Clear();
            BackColor = backColor.HasValue ? new ColorViewModel(backColor.Value) : null;
            ForeColor = foreColor.HasValue ? new ColorViewModel(foreColor.Value) : null;
            var formatter = ColorTextFormatter;
            if (palettes != null)
            {
                var baseX = 0.0;
                var baseY = 0.0;
                if (SelectedRange is Rect rect)
                {
                    baseX = rect.X;
                    baseY = rect.Y;
                }

                foreach (var pt in palettes)
                {
                    var point = analyser.GetPoint(pt.Position);
                    if (point != null)
                    {
                        var colorPoint = new ColorPoint
                        {
                            Color = new RgbColor((byte)pt.Mean[0], (byte)pt.Mean[1], (byte)pt.Mean[2]),
                            X = baseX + point.Value.X,
                            Y = baseY + point.Value.Y,
                        };
                        if (formatter != null)
                        {
                            colorPoint.ViewModel.Text = formatter.Format(colorPoint.Color);
                        }
                        ColorPoints.Add(colorPoint);
                    }
                }
            }
        }

        public static ExtractImageColorsView FromClipboard()
        {
            if (Clipboard.ContainsImage())
            {
                var bitmapSource = Clipboard.GetImage();

                // image data
                var stride = bitmapSource.PixelWidth * ((bitmapSource.Format.BitsPerPixel + 7) / 8);
                var data = new byte[stride * bitmapSource.PixelHeight];
                bitmapSource.CopyPixels(data, stride, 0);
                var imageData = new BitmapData(bitmapSource.PixelWidth, bitmapSource.PixelHeight, data);
                var previewImage = ImageHelper.BitmapSourceToImage(bitmapSource);

                return new ExtractImageColorsView(imageData, previewImage);
            }

            return null;
        }

        public static Task<ExtractImageColorsView> OpenImageFileAsync(Window owner)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = ImagesFilter,
                Title = "Open a Picture to extract colors"
            };

            if (openFileDialog.ShowDialog(owner) != true)
            {
                return Task.FromResult<ExtractImageColorsView>(null);
            }

            return FromImageFileAsync(openFileDialog.FileName);
        }

        public static async Task<ExtractImageColorsView> FromImageFileAsync(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new Exception("Invalid File Name.");
            }
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException();
            }

            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                var imageData = await BitmapData.LoadStreamAsync(stream);
                stream.Position = 0;
                var previewImage = await ImageHelper.LoadBitmapImageAsync(stream);

                return new ExtractImageColorsView(imageData, previewImage, filename);
            }
        }

        private void Reanalyse_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AnalyseImage();
        }

        private void OK_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow mainWindow && ColorPoints.Any())
            {
                var swatch = new Swatch
                {
                    FileName = System.IO.Path.GetFileNameWithoutExtension(FileName)
                };

                foreach (var colorPoint in ColorPoints)
                {
                    swatch.Colors.Add(colorPoint.Color);
                }

                mainWindow.AddDocumentView(new SwatchView
                {
                    Swatch = new SwatchViewModel(swatch)
                });
                mainWindow.CloseDocumentView(this);
            }
        }

        private void ImagePickBox_PointMoved(object sender, RoutedEventArgs e)
        {
            if (analyser != null && e.OriginalSource is ColorPoint colorPoint)
            {
                var x = (int)colorPoint.X;
                var y = (int)colorPoint.Y;

                if (SelectedRange is Rect rect)
                {
                    x -= (int)rect.X;
                    y -= (int)rect.Y;
                }

                if (analyser.TryGetPixel(x, y, out System.Drawing.Color? color) && color != null)
                {
                    colorPoint.Color = new RgbColor(color.Value.R, color.Value.G, color.Value.B);
                    if (ColorTextFormatter is IColorTextFormatter formatter)
                    {
                        colorPoint.ViewModel.Text = formatter.Format(colorPoint.Color);
                    }
                }
            }
        }
    }
}
