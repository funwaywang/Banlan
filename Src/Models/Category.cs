using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows;

namespace Banlan
{
    public class Category : IEnumerable<ColorBase>, INotifyPropertyChanged
    {
        private string _Name;
        private bool _IsOpen = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public Category()
        {
        }

        public Category(string name, IEnumerable<ColorBase> colors = null)
        {
            Name = name;
            if (colors != null)
            {
                foreach (var c in colors)
                {
                    Colors.Add(c);
                }
            }
        }

        public bool IsOpen
        {
            get => _IsOpen;
            set
            {
                if (_IsOpen != value)
                {
                    _IsOpen = value;
                    OnPropertyChanged(nameof(IsOpen));
                }
            }
        }

        public string Name
        {
            get => _Name;
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public ObservableCollection<ColorBase> Colors { get; private set; } = new ObservableCollection<ColorBase>();

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public IEnumerator<ColorBase> GetEnumerator()
        {
            return Colors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Colors.GetEnumerator();
        }

        public void Add(ColorBase color)
        {
            Colors.Add(color);
        }
    }
}
