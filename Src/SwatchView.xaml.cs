using Banlan.SwatchFiles;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Banlan
{
    public partial class SwatchView : DocumentView
    {
        private SaveFileDialog? saveFileDialog;
        public static readonly DependencyProperty SwatchProperty = DependencyProperty.Register(nameof(Swatch), typeof(SwatchViewModel), typeof(SwatchView));
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(ColorViewModel), typeof(SwatchView));
        public static readonly DependencyProperty ColorTextFormatterProperty = DependencyProperty.Register(nameof(ColorTextFormatter), typeof(IColorTextFormatter), typeof(SwatchView));

        public SwatchView()
        {
            InitializeComponent();

            SetBinding(ColorTextFormatterProperty, new Binding(AppStatus.SelectedFormatterProperty.Name) { Source = AppStatus.Default, Mode = BindingMode.OneWay });
        }

        public SwatchViewModel Swatch
        {
            get => (SwatchViewModel)GetValue(SwatchProperty);
            set => SetValue(SwatchProperty, value);
        }

        public ColorViewModel? SelectedColor
        {
            get => (ColorViewModel?)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        public IColorTextFormatter ColorTextFormatter
        {
            get => (IColorTextFormatter)GetValue(ColorTextFormatterProperty);
            set => SetValue(ColorTextFormatterProperty, value);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == SwatchProperty)
            {
                if (e.NewValue is SwatchViewModel swatch)
                {
                    SetBinding(TitleProperty, new Binding(CategoryViewModel.NameProperty.Name) { Source = swatch, Converter = new LimitStringConverter() });
                    SetBinding(FileNameProperty, new Binding(SwatchViewModel.FileNameProperty.Name) { Source = swatch });
                    SetBinding(IsModifiedProperty, new Binding(CategoryViewModel.IsModifiedProperty.Name) { Source = Swatch, Mode = BindingMode.TwoWay });
                    CanSave = true;
                    RefreshColorTexts();
                }
            }
            else if (e.Property == SelectedColorProperty)
            {
                if (e.OldValue is ColorViewModel oldValue)
                {
                    oldValue.IsSelected = false;
                }

                if (e.NewValue is ColorViewModel newValue)
                {
                    newValue.IsSelected = true;
                    if (newValue.Color != null)
                    {
                        AppStatus.Default.SelectColor(newValue.Color);
                    }
                }
            }
            else if (e.Property == ColorTextFormatterProperty)
            {
                RefreshColorTexts();
            }
        }

        public override async Task<bool> SaveAsync()
        {
            if (Swatch == null || Swatch.Swatch == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(Swatch.FileName)
                && SwatchFileManage.SupportToSaveAs(Swatch.FileName)
                && File.Exists(Swatch.FileName))
            {
                try
                {
                    await SwatchFileManage.SaveAsync(Swatch.Swatch, Swatch.FileName);
                    IsModified = false;
                    Settings.Default.AddRecentFile(new SwatchFileSummary(Swatch.Swatch));
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Window.GetWindow(this), ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            else
            {
                return await SaveAsAsync();
            }
        }

        public override async Task<bool> SaveAsAsync()
        {
            if (Swatch == null || Swatch.Swatch == null)
            {
                return false;
            }

            if (saveFileDialog == null)
            {
                saveFileDialog = new SaveFileDialog
                {
                    Filter = SwatchFileManage.SaveFileFilter,
                    FilterIndex = Array.IndexOf(SwatchFileManage.SupportedWrite, BanlanFile.Default) + 1
                };
            }

            if (!string.IsNullOrEmpty(Swatch.FileName))
            {
                saveFileDialog.FileName = System.IO.Path.GetFileName(Swatch.FileName);
            }
            else if (!string.IsNullOrEmpty(Swatch.Name))
            {
                saveFileDialog.FileName = Swatch.Name;
            }

            if (saveFileDialog.ShowDialog(Window.GetWindow(this)) == true)
            {
                try
                {
                    ISwatchFile? fileType = null;
                    var filterIndex = saveFileDialog.FilterIndex - 1;
                    if (filterIndex > -1 && filterIndex < SwatchFileManage.SupportedWrite.Length)
                    {
                        fileType = SwatchFileManage.SupportedWrite[filterIndex];
                    }

                    var filename = saveFileDialog.FileName;
                    if (!SwatchFileManage.SupportToSaveAs(filename))
                    {
                        filename += fileType?.Extensions?.FirstOrDefault() ?? BanlanFile.Default.Extensions.First();
                    }

                    await SwatchFileManage.SaveAsync(Swatch.Swatch, filename, fileType);
                    IsModified = false;
                    Settings.Default.AddRecentFile(new SwatchFileSummary(Swatch.Swatch));
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Window.GetWindow(this), ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return false;
        }

        private void SortBy_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void ExpandAll_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Swatch != null)
            {
                Swatch.IsOpen = true;

                foreach (var category in Swatch.Categories)
                {
                    category.IsOpen = true;
                }
            }
        }

        private void CollapseAll_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Swatch != null)
            {
                Swatch.IsOpen = false;

                foreach (var category in Swatch.Categories)
                {
                    category.IsOpen = false;
                }
            }
        }

        private void Properties_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void SelectColor_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectedColor = e.Parameter as ColorViewModel;
        }

        private void RefreshColorTexts()
        {
            if (Swatch != null && ColorTextFormatter is IColorTextFormatter formatter)
            {
                foreach (var color in Swatch.Colors.Union(Swatch.Categories.SelectMany(c => c.Colors)))
                {
                    color.Text = formatter.Format(color.Color.R, color.Color.G, color.Color.B);
                }
            }
        }

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!IsReadOnly)
            {
                if (e.Parameter is ColorViewModel cv)
                {
                    Swatch.TryRemove(cv);
                }
            }
        }

        private void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsReadOnly && e.Parameter != null;
        }

        public override async Task<bool> ConfirmToCloseAsync()
        {
            if (!IsReadOnly && IsModified)
            {
                var result = MessageBox.Show(Window.GetWindow(this), "Do you want to save this swatch?", nameof(Banlan), MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    return await SaveAsync();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return false;
                }
            }

            return await base.ConfirmToCloseAsync();
        }
    }
}
