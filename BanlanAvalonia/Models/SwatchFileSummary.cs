using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Banlan
{
    public class SwatchFileSummary : INotifyPropertyChanged
    {
        private const int GeneralSamplesCount = 6;
        private string? _FileName;
        private Color[]? _Samples;
        private DateTime _UpdateTime;

        public event PropertyChangedEventHandler? PropertyChanged;

        public SwatchFileSummary()
        {
        }

        public SwatchFileSummary(Swatch swatch)
        {
            if (swatch != null)
            {
                FileName = swatch.FileName;
                Samples = swatch.Colors.Union(swatch.Categories.SelectMany(c => c.Colors)).Take(GeneralSamplesCount).Select(c => c.ToDrawingColor()).ToArray();
                UpdateTime = DateTime.Now;
            }
        }

        public string? FileName
        {
            get => _FileName;
            set
            {
                if (_FileName != value)
                {
                    _FileName = value;
                    OnPropertyChanged(nameof(FileName));
                }
            }
        }

        public Color[]? Samples
        {
            get => _Samples;
            set
            {
                if (_Samples != value)
                {
                    _Samples = value;
                    OnPropertyChanged(nameof(Samples));
                }
            }
        }

        public DateTime UpdateTime
        {
            get => _UpdateTime;
            set
            {
                if (_UpdateTime != value)
                {
                    _UpdateTime = value;
                    OnPropertyChanged(nameof(UpdateTime));
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
