using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Banlan
{
    public class ColorPoint : DependencyObject
    {
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(ColorBase), typeof(ColorPoint));
        public static readonly DependencyProperty XProperty = DependencyProperty.Register(nameof(X), typeof(double), typeof(ColorPoint));
        public static readonly DependencyProperty YProperty = DependencyProperty.Register(nameof(Y), typeof(double), typeof(ColorPoint));

        public ColorPoint()
        {
            BindingOperations.SetBinding(ViewModel, ColorViewModel.ColorProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath(ColorProperty.Name),
                Mode = BindingMode.OneWay,
            });
        }

        public ColorBase Color
        {
            get => (ColorBase)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public double X
        {
            get => (double)GetValue(XProperty);
            set => SetValue(XProperty, value);
        }

        public double Y
        {
            get => (double)GetValue(YProperty);
            set => SetValue(YProperty, value);
        }

        public ColorViewModel ViewModel { get; private set; } = new ColorViewModel();
    }
}
