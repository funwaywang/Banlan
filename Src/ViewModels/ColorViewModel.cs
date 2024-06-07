using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Banlan
{
    public class ColorViewModel : DependencyObject
    {
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register(nameof(Name), typeof(string), typeof(ColorViewModel));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(Color?), typeof(ColorViewModel));
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(ColorViewModel));
        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(ColorViewModel));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(ColorViewModel));
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(ColorBase), typeof(ColorViewModel));
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(ColorViewModel));

        public ColorViewModel()
        {
        }

        public ColorViewModel(ColorBase color)
        {
            Color = color;
        }

        public ColorViewModel(System.Drawing.Color color)
        {
            Color = new RgbColor(color.R, color.G, color.B);
        }

        public ColorBase Color
        {
            get => (ColorBase)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public string? Name
        {
            get => (string?)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        public Color? Value
        {
            get => (Color?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public Brush? Background
        {
            get => (Brush?)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        public Brush? Foreground
        {
            get => (Brush?)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }

        public string? Text
        {
            get => (string?)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == ColorProperty)
            {
                if (e.NewValue is ColorBase info)
                {
                    Name = info.Name;
                    Value = System.Windows.Media.Color.FromRgb(info.R, info.G, info.B);
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
