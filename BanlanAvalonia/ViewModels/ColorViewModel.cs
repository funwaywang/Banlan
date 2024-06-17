using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Avalonia.Media;
using Banlan.ViewModels;
using ReactiveUI;

namespace Banlan
{
    public class ColorViewModel : ViewModelBase
    {
        private string? _name;
        private Color? _value;
        private Brush? _background;
        private Brush? _foreground;
        private string? _text;
        private ColorBase? _color;
        private bool _isSelected;

        public ColorViewModel()
        {
            PropertyChanged += ColorViewModel_PropertyChanged;
        }

        public ColorViewModel(ColorBase color) : this()
        {
            Color = color;
        }

        public ColorViewModel(System.Drawing.Color color) : this()
        {
            Color = new RgbColor(color.R, color.G, color.B);
        }

        public ColorBase? Color
        {
            get => _color;
            set => this.RaiseAndSetIfChanged(ref _color, value);
        }

        public string? Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public Color? Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        public Brush? Background
        {
            get => _background;
            set => this.RaiseAndSetIfChanged(ref _background, value);
        }

        public Brush? Foreground
        {
            get => _foreground;
            set => this.RaiseAndSetIfChanged(ref _foreground, value);
        }

        public string? Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }

        private void ColorViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Color))
            {
                if (Color is ColorBase info)
                {
                    Name = info.Name;
                    Value = Avalonia.Media.Color.FromRgb(info.R, info.G, info.B);
                    Text = ColorHelper.ToHexColor(info.R, info.G, info.B);
                    Background = Value != null ? new SolidColorBrush(Value.Value) : null;
                    Foreground = ColorHelper.GetForeground(info.R, info.G, info.B);
                }
                else
                {
                    Name = null;
                    Value = null;
                    Text = null;
                    Background = null;
                    Foreground = null;
                }
            }
        }
    }
}
