using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Banlan.ViewModels;
using ReactiveUI;

namespace Banlan
{
    public class ColorPoint : ViewModelBase
    {
        private ColorBase? _color;
        private double _x;
        private double _y;

        public ColorPoint()
        {
            PropertyChanged += ColorPoint_PropertyChanged;
        }

        public ColorBase? Color
        {
            get => _color;
            set => this.RaiseAndSetIfChanged(ref _color, value);
        }

        public double X
        {
            get => _x;
            set => this.RaiseAndSetIfChanged(ref _x, value);
        }

        public double Y
        {
            get => _y;
            set => this.RaiseAndSetIfChanged(ref _y, value);
        }

        public ColorViewModel ViewModel { get; private set; } = new ColorViewModel();

        private void ColorPoint_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Color))
            {
                ViewModel.Color = _color;
            }
        }
    }
}
