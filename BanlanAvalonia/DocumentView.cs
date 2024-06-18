using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Banlan
{
    public class DocumentView : UserControl, ILocalizable
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(object), typeof(DocumentView));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(DocumentView));
        public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(nameof(FileName), typeof(string), typeof(DocumentView));
        public static readonly DependencyProperty CanSaveProperty = DependencyProperty.Register(nameof(CanSave), typeof(bool), typeof(DocumentView), new PropertyMetadata(false));
        public static readonly DependencyProperty CanCloseProperty = DependencyProperty.Register(nameof(CanClose), typeof(bool), typeof(DocumentView), new PropertyMetadata(true));
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(DocumentView), new PropertyMetadata(false));
        public static readonly DependencyProperty IsModifiedProperty = DependencyProperty.Register(nameof(IsModified), typeof(bool), typeof(DocumentView), new PropertyMetadata(false));
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(DocumentView), new PropertyMetadata(false));

        public object Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string? FileName
        {
            get => (string?)GetValue(FileNameProperty);
            set => SetValue(FileNameProperty, value);
        }

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public bool CanSave
        {
            get => (bool)GetValue(CanSaveProperty);
            set => SetValue(CanSaveProperty, value);
        }

        public bool CanClose
        {
            get => (bool)GetValue(CanCloseProperty);
            set => SetValue(CanCloseProperty, value);
        }

        public bool IsModified
        {
            get => (bool)GetValue(IsModifiedProperty);
            set => SetValue(IsModifiedProperty, value);
        }

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public virtual Task<bool> SaveAsync()
        {
            return Task.FromResult(false);
        }

        public virtual Task<bool> SaveAsAsync()
        {
            return Task.FromResult(false);
        }

        public virtual Task<bool> ConfirmToCloseAsync()
        {
            return Task.FromResult(true);
        }

        public void SetLocalizedText(string text)
        {
            Title = text;
        }
    }
}
