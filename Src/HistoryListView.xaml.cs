using Banlan.SwatchFiles;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    public partial class HistoryListView : DocumentView
    {
        private SaveFileDialog? saveFileDialog;
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(ColorViewModel), typeof(HistoryListView));

        public HistoryListView()
        {
            InitializeComponent();

            Title = "History";
            Lang.SetId(this, Title);
            CanClose = true;
            CanSave = true;
            History = new SwatchViewModel(AppStatus.Default.History);
        }

        public SwatchViewModel History { get; private set; }

        public ColorViewModel? SelectedColor
        {
            get => (ColorViewModel?)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        private void Properties_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void SortBy_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void SelectColor_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectedColor = e.Parameter as ColorViewModel;
        }

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is ColorViewModel cvm)
            {
                History.TryRemove(cvm);
            }
        }

        private void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter != null;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == SelectedColorProperty)
            {
                if (e.NewValue is ColorViewModel cvm)
                {
                    AppStatus.Default.SelectColor(Color.FromRgb(cvm.Color.R, cvm.Color.G, cvm.Color.B), false);
                }
            }
        }

        public override Task<bool> SaveAsync()
        {
            return SaveAsAsync();
        }

        public override async Task<bool> SaveAsAsync()
        {
            var swatch = History.Swatch;
            if (swatch == null)
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

                    if (fileType == null)
                    {
                        fileType = BanlanFile.Default;
                    }

                    var filename = saveFileDialog.FileName;
                    if (!SwatchFileManage.SupportToSaveAs(filename))
                    {
                        filename += fileType.Extensions.FirstOrDefault();
                    }

                    using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
                    {
                        await SwatchFileManage.SaveAsync(swatch, stream, fileType);
                    }

                    if (File.Exists(filename) && Window.GetWindow(this) is MainWindow mainWindow)
                    {
                        mainWindow.OpenFileAsync(filename);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Window.GetWindow(this), ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return false;
        }
    }
}
