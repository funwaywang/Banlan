using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using Avalonia.Threading;
using ReactiveUI;

namespace Banlan
{
    public class SwatchViewModel : CategoryViewModel
    {
        private string? _fileName;
        private bool _hasCategories;

        public SwatchViewModel()
        {
        }

        public SwatchViewModel(Swatch swatch)
            : base(swatch)
        {
            if (swatch != null)
            {
                FileName = swatch.FileName;
                Categories.Add(new CategoryViewModel(swatch));
                foreach (var c in swatch.Categories)
                {
                    Categories.Add(new CategoryViewModel(c)
                    {
                        Level = 1
                    });
                }
                HasCategories = swatch.Categories.Any();
                swatch.Categories.CollectionChanged += Categories_CollectionChanged;
                swatch.PropertyChanged += Swatch_PropertyChanged;
            }
        }

        public bool HasCategories
        {
            get => _hasCategories;
            set => this.RaiseAndSetIfChanged(ref _hasCategories, value);
        }

        public string? FileName
        {
            get => _fileName;
            set => this.RaiseAndSetIfChanged(ref _fileName, value);
        }

        public Swatch? Swatch => Info as Swatch;

        public ObservableCollection<CategoryViewModel> Categories { get; private set; } = new ObservableCollection<CategoryViewModel>();

        private void Swatch_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Swatch.FileName))
            {
                Dispatcher.UIThread.Post(() => FileName = Swatch?.FileName);
            }
        }

        private void Categories_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                Categories.Clear();
            }
            else
            {
                if (e.OldItems != null)
                {
                    foreach (var it in e.OldItems.OfType<Category>())
                    {
                        var vm = Categories.FirstOrDefault(c => c.Info == it);
                        if (vm != null)
                        {
                            Categories.Remove(vm);
                        }
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (var it in e.NewItems.OfType<Category>())
                    {
                        Categories.Add(new CategoryViewModel(it)
                        {
                            Level = 1
                        });
                    }
                }
            }

            HasCategories = Swatch?.Categories.Any() == true;
        }

        public override bool TryRemove(ColorViewModel model)
        {
            if (base.TryRemove(model))
            {
                return true;
            }

            foreach (var category in Categories)
            {
                if (category.TryRemove(model))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
