using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Avalonia.Threading;
using Banlan.ViewModels;
using ReactiveUI;

namespace Banlan
{
    public class CategoryViewModel : ViewModelBase
    {
        private int _level;
        private string? _name;
        private bool _isOpen;
        private bool _isEmpty;
        private bool _isModified;

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
                category.Colors.CollectionChanged += Category_Colors_CollectionChanged;
                category.PropertyChanged += Category_PropertyChanged;
            }

            Colors.CollectionChanged += Colors_CollectionChanged;
            IsEmpty = !Colors.Any();
        }

        public Category? Info { get; private set; }

        public int Level
        {
            get => _level;
            set => this.RaiseAndSetIfChanged(ref _level, value);
        }

        public string? Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public bool IsOpen
        {
            get => _isOpen;
            set => this.RaiseAndSetIfChanged(ref _isOpen, value);
        }

        public bool IsEmpty
        {
            get => _isEmpty;
            set => this.RaiseAndSetIfChanged(ref _isEmpty, value);
        }

        public bool IsModified
        {
            get => _isModified;
            set => this.RaiseAndSetIfChanged(ref _isModified, value);
        }

        public ObservableCollection<ColorViewModel> Colors { get; private set; } = new ObservableCollection<ColorViewModel>();

        private void Category_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Category.Name))
            {
                Dispatcher.UIThread.Post(() => Name = Info?.Name);
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

        private void Colors_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            IsEmpty = !Colors.Any();
            IsModified = true;
        }

        private void Category_Colors_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
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
            if (model.Color != null 
                && Info?.Colors?.Contains(model.Color) == true)
            {
                Info.Colors.Remove(model.Color);
                return true;
            }

            return false;
        }
    }
}
