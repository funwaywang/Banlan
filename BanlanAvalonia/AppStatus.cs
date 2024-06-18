using Avalonia;
using Avalonia.Media;
using Banlan.SwatchFiles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;

namespace Banlan
{
    public class AppStatus : AvaloniaObject
    {
        private const int MaxHistorySize = 50;
        private bool setTextBySelf = false;
        private static readonly string historyFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(Banlan), "History.bls");
        public static readonly AppStatus Default = new AppStatus();
        public static readonly DirectProperty<AppStatus, Color?> SelectedColorProperty = AvaloniaProperty.RegisterDirect<AppStatus, Color?>(nameof(SelectedColor), o => o.SelectedColor);
        public static readonly DirectProperty<AppStatus, string?> SelectedColorTextProperty = AvaloniaProperty.RegisterDirect<AppStatus, string?>(nameof(SelectedColorText), o => o.SelectedColorText, (o, t) => o.SelectedColorText = t, defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);
        public static readonly DirectProperty<AppStatus, IColorTextFormatter?> SelectedFormatterProperty = AvaloniaProperty.RegisterDirect<AppStatus, IColorTextFormatter?>(nameof(SelectedFormatter), o => o.SelectedFormatter);

        public ObservableCollection<IColorTextFormatter> ColorTextFormatters { get; private set; } = new ObservableCollection<IColorTextFormatter>
        {
            new HexColorTextFormater(),
            new Hex2ColorTextFormater(),
            new RgbColorTextFormater(),
            new CssRgbColorTextFormater(),
        };

        public Color? SelectedColor
        {
            get => GetValue(SelectedColorProperty);
            private set => SetValue(SelectedColorProperty, value);
        }

        public string? SelectedColorText
        {
            get => GetValue(SelectedColorTextProperty);
            set => SetValue(SelectedColorTextProperty, value);
        }

        public IColorTextFormatter? SelectedFormatter
        {
            get => GetValue(SelectedFormatterProperty);
            set => SetValue(SelectedFormatterProperty, value);
        }

        public Swatch History { get; private set; } = new Swatch { FileName = historyFileName };

        public void Startup()
        {
            SelectedFormatter = ColorTextFormatters.FirstOrDefault(f => string.Equals(f.Name, Settings.Default["ColorTextFormat"], StringComparison.OrdinalIgnoreCase)) ?? ColorTextFormatters.First();

            var history = SwatchFileManage.Load(historyFileName);
            if (history != null)
            {
                History = history;
            }
        }

        public void Exit()
        {
            SwatchFileManage.Save(History, historyFileName);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
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

            if (appendToHistory)
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
