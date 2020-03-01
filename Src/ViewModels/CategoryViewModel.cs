using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Banlan
{
    public class CategoryViewModel : DependencyObject
    {
        public static readonly DependencyProperty LevelProperty = DependencyProperty.Register(nameof(Level), typeof(int), typeof(CategoryViewModel), new PropertyMetadata(0));
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register(nameof(Name), typeof(string), typeof(CategoryViewModel));
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(CategoryViewModel));
        public static readonly DependencyProperty IsEmptyProperty = DependencyProperty.Register(nameof(IsEmpty), typeof(bool), typeof(CategoryViewModel));
        public static readonly DependencyProperty IsModifiedProperty = DependencyProperty.Register(nameof(IsModified), typeof(bool), typeof(CategoryViewModel));

        public CategoryViewModel()
        {
        }

        public CategoryViewModel(Category category)
        {
            Info = category;
            if (category != null)
            {
                Name = category.Name;
                IsOpen = category.IsOpen;
                RefreshColors(category);
                if (category != null)
                {
                    category.Colors.CollectionChanged += Category_Colors_CollectionChanged;
                }
                category.PropertyChanged += Category_PropertyChanged;
            }

            Colors.CollectionChanged += Colors_CollectionChanged;
            IsEmpty = !Colors.Any();
        }

        public Category Info { get; private set; }

        public int Level
        {
            get => (int)GetValue(LevelProperty);
            set => SetValue(LevelProperty, value);
        }

        public string Name
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        public bool IsEmpty
        {
            get => (bool)GetValue(IsEmptyProperty);
            set => SetValue(IsEmptyProperty, value);
        }

        public bool IsModified
        {
            get => (bool)GetValue(IsModifiedProperty);
            set => SetValue(IsModifiedProperty, value);
        }

        public ObservableCollection<ColorViewModel> Colors { get; private set; } = new ObservableCollection<ColorViewModel>();

        private void Category_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Category.Name))
            {
                this.RunInUIThread(() => Name = Info?.Name);
            }
        }

        private void RefreshColors(Category category)
        {
            Colors.Clear();
            if (category != null)
            {
                foreach (var c in category.Colors)
                {
                    Colors.Add(new ColorViewModel(c));
                }
            }
        }

        private void Colors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IsEmpty = !Colors.Any();
            IsModified = true;
        }

        private void Category_Colors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                Colors.Clear();
            }
            else
            {
                if (e.OldItems != null)
                {
                    foreach (var it in e.OldItems.OfType<ColorBase>())
                    {
                        var vm = Colors.FirstOrDefault(c => c.Color == it);
                        if (vm != null)
                        {
                            Colors.Remove(vm);
                        }
                    }
                }

                if (e.NewItems != null)
                {
                    var index = e.NewStartingIndex;
                    foreach (var it in e.NewItems.OfType<ColorBase>())
                    {
                        Colors.Insert(index++, new ColorViewModel(it));
                    }
                }
            }
        }

        public virtual bool TryRemove(ColorViewModel model)
        {
            if (Info.Colors.Contains(model.Color))
            {
                Info.Colors.Remove(model.Color);
                return true;
            }

            return false;
        }
    }
}
