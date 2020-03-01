using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Banlan
{
    public class Swatch : Category, INotifyPropertyChanged
    {
        private string _FileName;

        public Swatch()
        {
        }

        public Swatch(string name, IEnumerable<ColorBase> colors = null)
            : base(name, colors)
        {
        }

        public string FileName
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

        public ObservableCollection<Category> Categories { get; private set; } = new ObservableCollection<Category>();

        public void Merge(Swatch another)
        {
            if (another == null || another == this)
            {
                return;
            }

            if (another.Colors.Any())
            {
                string name = another.Name;
                if (string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(another.FileName))
                {
                    name = Path.GetFileName(another.FileName);
                }

                var category = new Category { Name = name };
                foreach (var c in another.Colors)
                {
                    category.Colors.Add(c);
                }
                Categories.Add(category);
            }

            foreach (var ac in another.Categories)
            {
                var category = new Category { Name = ac.Name };
                foreach (var c in ac.Colors)
                {
                    category.Colors.Add(c);
                }
                Categories.Add(category);
            }
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(FileName) && !string.IsNullOrEmpty(FileName))
            {
                Name = Path.GetFileNameWithoutExtension(FileName);
            }
        }
    }
}
