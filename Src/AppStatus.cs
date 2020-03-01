using Banlan.SwatchFiles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace Banlan
{
    public class AppStatus : DependencyObject
    {
        private const int MaxHistorySize = 50;
        private bool setTextBySelf = false;
        private static readonly string historyFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(Banlan), "History.bls");
        public static readonly AppStatus Default = new AppStatus();
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(System.Windows.Media.Color?), typeof(AppStatus));
        public static readonly DependencyProperty SelectedColorTextProperty = DependencyProperty.Register(nameof(SelectedColorText), typeof(string), typeof(AppStatus), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty SelectedFormatterProperty = DependencyProperty.Register(nameof(SelectedFormatter), typeof(IColorTextFormatter), typeof(AppStatus));

        public ObservableCollection<IColorTextFormatter> ColorTextFormatters { get; private set; } = new ObservableCollection<IColorTextFormatter>
        {
            new HexColorTextFormater(),
            new Hex2ColorTextFormater(),
            new RgbColorTextFormater(),
            new CssRgbColorTextFormater(),
        };

        public Color? SelectedColor
        {
            get => (Color?)GetValue(SelectedColorProperty);
            private set => SetValue(SelectedColorProperty, value);
        }

        public string SelectedColorText
        {
            get => (string)GetValue(SelectedColorTextProperty);
            set => SetValue(SelectedColorTextProperty, value);
        }

        public IColorTextFormatter SelectedFormatter
        {
            get => (IColorTextFormatter)GetValue(SelectedFormatterProperty);
            set => SetValue(SelectedFormatterProperty, value);
        }

        public Swatch History { get; private set; } = new Swatch { FileName = historyFileName };

        public void Startup()
        {
            SelectedFormatter = ColorTextFormatters.FirstOrDefault(f => string.Equals(f.Name, Settings.Default["ColorTextFormat"], StringComparison.OrdinalIgnoreCase)) ?? ColorTextFormatters.FirstOrDefault();
            History = SwatchFileManage.Load(historyFileName);
        }

        public void Exit()
        {
            SwatchFileManage.Save(History, historyFileName);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == SelectedFormatterProperty)
            {
                Settings.Default["ColorTextFormat"] = (e.NewValue as IColorTextFormatter)?.Name;
                RefreshSelectedColorText();
            }
            else if (e.Property == SelectedColorProperty)
            {
                RefreshSelectedColorText();
            }
            else if (e.Property == SelectedColorTextProperty)
            {
                if (!setTextBySelf)
                {
                    var color = ColorHelper.ParseColor(e.NewValue as string, false);
                    if (color != null)
                    {
                        SelectColor(color.Value);
                    }
                }
            }
        }

        private void RefreshSelectedColorText()
        {
            setTextBySelf = true;
            try
            {
                if (SelectedColor == null)
                {
                    SelectedColorText = null;
                }
                else if (SelectedFormatter != null)
                {
                    SelectedColorText = SelectedFormatter.Format(SelectedColor.Value.R, SelectedColor.Value.G, SelectedColor.Value.B);
                }
                else
                {
                    SelectedColorText = ColorHelper.ToHexColor(SelectedColor.Value);
                }
            }
            finally
            {
                setTextBySelf = false;
            }
        }

        public void SelectColor(System.Drawing.Color color, bool appendToHistory = true)
        {
            SelectColor(Color.FromRgb(color.R, color.G, color.B), appendToHistory);
        }

        public void SelectColor(ColorBase color, bool appendToHistory = true)
        {
            SelectedColor = color?.ToMediaColor();

            if (color != null && appendToHistory)
            {
                AppendToHistory(color.R, color.G, color.B);
            }
        }

        public void SelectColor(Color color, bool appendToHistory = true)
        {
            SelectedColor = color;

            if (color != null && appendToHistory)
            {
                AppendToHistory(color.R, color.G, color.B);
            }
        }

        private void AppendToHistory(byte r, byte g, byte b)
        {
            var old = History.Colors.FirstOrDefault(ch => ch.R == r && ch.G == g && ch.B == b);
            if (old != null)
            {
                History.Colors.Remove(old);
            }

            History.Colors.Insert(0, new RgbColor(r, g, b));
            while (History.Colors.Count > MaxHistorySize)
            {
                History.Colors.RemoveAt(MaxHistorySize);
            }
        }
    }
}
