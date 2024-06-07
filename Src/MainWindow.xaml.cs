using Banlan.Core;
using Banlan.SwatchFiles;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
    public partial class MainWindow : Window
    {
        private readonly StartPage startPage;
        private HistoryListView? historyListView;
        private SettingsPage? settingsPage;
        private OpenFileDialog? openFileDialog;
        private int FromClipboardIndex = 1;
        private int NewSwatchIndex = 1;
        public static readonly DependencyProperty CurrentDocumentViewProperty = DependencyProperty.Register(nameof(CurrentDocumentView), typeof(DocumentView), typeof(MainWindow));

        public MainWindow()
        {
            Current = this;
            InitializeComponent();

            Lang.SetIds(this, Title);
            startPage = new StartPage();
            AddDocumentView(startPage);
            AddHandler(ScreenColorPicker.ColorPickedEvent, new RoutedEventHandler(ScreenColorPicker_ColorPicked));
        }

        private void SwatchDocuments_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = e.Item is SwatchView;
        }

        public static MainWindow? Current { get; private set; }

        public ObservableCollection<DocumentView> Documents { get; private set; } = new ObservableCollection<DocumentView>();

        public DocumentView? CurrentDocumentView
        {
            get => (DocumentView?)GetValue(CurrentDocumentViewProperty);
            set => SetValue(CurrentDocumentViewProperty, value);
        }

        public ObservableCollection<SwatchView> SwatchDocuments { get; private set; } = new ObservableCollection<SwatchView>();

        public void AddDocumentView(DocumentView view)
        {
            Documents.Add(view);
            CurrentDocumentView = view;

            if (view is SwatchView sv && !SwatchDocuments.Contains(sv))
            {
                SwatchDocuments.Add(sv);
            }
        }

        public void CloseDocumentView(DocumentView view)
        {
            if (CurrentDocumentView == view)
            {
                CurrentDocumentView = Documents.FirstOrDefault();
            }
            Documents.Remove(view);

            if (view is SwatchView sv && SwatchDocuments.Contains(sv))
            {
                SwatchDocuments.Remove(sv);
            }
        }

        private async void ExtractColorsFromImage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (await ExtractImageColorsView.OpenImageFileAsync(this) is DocumentView view)
            {
                AddDocumentView(view);
            }
        }

        private void ScreenColorPicker_ColorPicked(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Color color)
            {
                AppStatus.Default.SelectColor(color);
            }
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is string filename)
            {
                OpenFileAsync(filename);
            }
            else
            {
                ShowOpenDialog(null);
            }
        }

        public async void OpenFileAsync(string filename, ISwatchFile? fileType = null)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                return;
            }

            try
            {

                if (!File.Exists(filename))
                {
                    throw new Exception("The specified file does not exist." + "\n" + filename);
                }

                var view = Documents.FirstOrDefault(s => string.Equals(s.FileName, filename, StringComparison.OrdinalIgnoreCase));
                if (view != null)
                {
                    CurrentDocumentView = view;
                }
                else
                {
                    var swatch = await SwatchFileManage.LoadAsync(filename, fileType);
                    if (swatch != null)
                    {
                        AddDocumentView(new SwatchView
                        {
                            Swatch = new SwatchViewModel(swatch)
                        });

                        Settings.Default.AddRecentFile(new SwatchFileSummary(swatch));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CurrentDocumentView != null && CurrentDocumentView.CanSave)
            {
                CurrentDocumentView.SaveAsync();
            }
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CurrentDocumentView != null && CurrentDocumentView.CanSave;
        }

        private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CurrentDocumentView != null && CurrentDocumentView.CanSave)
            {
                CurrentDocumentView.SaveAsAsync();
            }
        }

        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void OpenAsOne_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (openFileDialog == null)
            {
                openFileDialog = new OpenFileDialog
                {
                    Filter = SwatchFileManage.OpenFileFilter,
                    Multiselect = true
                };
            }

            openFileDialog.Title = "Open Multi-Files as One…";
            if (openFileDialog.ShowDialog(this) == true && openFileDialog.FileNames != null)
            {
                ISwatchFile? fileType = null;
                var filterIndex = openFileDialog.FilterIndex - 2;
                if (filterIndex > -1 && filterIndex < SwatchFileManage.SupportedRead.Length)
                {
                    fileType = SwatchFileManage.SupportedRead[filterIndex];
                }

                try
                {
                    if (openFileDialog.FileNames.Length == 1)
                    {
                        OpenFileAsync(openFileDialog.FileNames[0], fileType);
                    }
                    else
                    {
                        var name = string.Format("{0}_{1}files", StringHelper.LimitStringLength(System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName), 16), openFileDialog.FileNames.Length);
                        var allInOne = new Swatch
                        {
                            FileName = name
                        };

                        foreach (var filename in openFileDialog.FileNames)
                        {
                            var swatch = await SwatchFileManage.LoadAsync(filename, fileType);
                            if (swatch != null)
                            {
                                allInOne.Merge(swatch);
                            }
                        }

                        AddDocumentView(new SwatchView
                        {
                            Swatch = new SwatchViewModel(allInOne)
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == CurrentDocumentViewProperty)
            {
                foreach (var documentView in Documents)
                {
                    documentView.IsSelected = documentView == e.NewValue;
                }
            }
        }

        private void SelectDocumentView_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is DocumentView dv && Documents.Contains(dv))
            {
                CurrentDocumentView = dv;
            }
        }

        private async void CloseTab_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is DocumentView view && Documents.Contains(view) && view.CanClose)
            {
                if (await view.ConfirmToCloseAsync())
                {
                    CloseDocumentView(view);
                }
            }
        }

        private void CloseTab_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is DocumentView view
                && Documents.Contains(view)
                && view.CanClose;
        }

        private async void CloseOtherTabs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var otherTabs = Documents.Where(d => d != e.Parameter && d.CanClose).ToList();
            foreach (var tab in otherTabs)
            {
                if (await tab.ConfirmToCloseAsync())
                {
                    CloseDocumentView(tab);
                }
            }
        }

        private void CloseOtherTabs_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Documents.Any(d => d != e.Parameter && d.CanClose);
        }

        private async void CloseAllTabs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var tabs = Documents.Where(d => d.CanClose).ToList();
            foreach (var tab in tabs)
            {
                if (await tab.ConfirmToCloseAsync())
                {
                    CloseDocumentView(tab);
                }
            }
        }

        private void CloseAllTabs_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Documents.Any(d => d.CanClose);
        }

        private void CopyFileName_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is DocumentView view && !string.IsNullOrEmpty(view.FileName))
            {
                Clipboard.SetText(view.FileName);
            }
        }

        private void CopyFileName_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is DocumentView view && !string.IsNullOrEmpty(view.FileName);
        }

        private void OpenContainerFolder_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is DocumentView view && !string.IsNullOrEmpty(view.FileName))
            {
                if (!File.Exists(view.FileName))
                {
                    MessageBox.Show(this, $"The file \"{view.FileName}\" is not exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    Process.Start("Explorer.exe", string.Format("/select, \"{0}\"", view.FileName));
                }
            }
        }

        private void OpenContainerFolder_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is DocumentView view && !string.IsNullOrEmpty(view.FileName);
        }

        private void PasteImage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Clipboard.ContainsImage() && ExtractImageColorsView.FromClipboard() is DocumentView view)
            {
                view.Title = string.Format("Clipboard {0}", FromClipboardIndex++);
                AddDocumentView(view);
            }
        }

        private void PasteImage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Clipboard.ContainsImage();
        }

        private void Copy_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var formatter = e.Parameter as IColorTextFormatter ?? AppStatus.Default.SelectedFormatter;
            if (e.OriginalSource is ColorViewModel cvm)
            {
                CopyColor(cvm.Color, formatter);
            }
            else if (e.OriginalSource is FrameworkElement fe && fe.DataContext is ColorViewModel cvm2)
            {
                CopyColor(cvm2.Color, formatter);
            }
            else if (AppStatus.Default.SelectedColor is Color selectedColor)
            {
                CopyColor(selectedColor, formatter);
            }
        }

        private void CopyColor(object color, IColorTextFormatter formatter)
        {
            string? text = null;
            if (color is ColorBase cb)
            {
                text = formatter?.Format(cb) ?? ColorHelper.ToHexColor(cb.R, cb.G, cb.B);
            }
            else if (color is Color wc)
            {
                text = formatter?.Format(wc.R, wc.G, wc.B) ?? ColorHelper.ToHexColor(wc.R, wc.G, wc.B);
            }
            else if (color is System.Drawing.Color dc)
            {
                text = formatter?.Format(dc.R, dc.G, dc.B) ?? ColorHelper.ToHexColor(dc.R, dc.G, dc.B);
            }

            if (text != null)
            {
                Clipboard.SetText(text);
            }
        }

        private void SwitchColorTextFormat_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is IColorTextFormatter formatter)
            {
                AppStatus.Default.SelectedFormatter = formatter;
            }
        }

        private void AddColorToSwatch_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (AppStatus.Default.SelectedColor is Color color)
            {
                if (e.Parameter is SwatchView swatchView)
                {
                    if (swatchView?.Swatch?.Swatch != null)
                    {
                        swatchView?.Swatch?.Swatch.Colors.Add(new RgbColor(color.R, color.G, color.B));
                    }
                }
                else if (e.Parameter is SwatchViewModel swatchViewModel)
                {
                    if (swatchViewModel.Swatch != null)
                    {
                        swatchViewModel.Swatch.Colors.Add(new RgbColor(color.R, color.G, color.B));
                    }
                }
                else
                {
                    var name = string.Format("New Swatch {0}", NewSwatchIndex++);
                    AddDocumentView(new SwatchView
                    {
                        Swatch = new SwatchViewModel(new Swatch(name, new ColorBase[] { new RgbColor(color.R, color.G, color.B) }))
                    });
                }
            }
        }

        private void AddColorToSwatch_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AppStatus.Default.SelectedColor != null;
        }

        private void ShowHistoryTab_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (historyListView != null && Documents.Contains(historyListView))
            {
                CurrentDocumentView = historyListView;
            }
            else
            {
                historyListView = new HistoryListView();
                AddDocumentView(historyListView);
            }
        }

        private void SelectColor_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is Color mc)
            {
                AppStatus.Default.SelectColor(mc);
            }
            else if (e.Parameter is System.Drawing.Color dc)
            {
                AppStatus.Default.SelectColor(dc);
            }
            else if (e.Parameter is ColorBase cb)
            {
                AppStatus.Default.SelectColor(Color.FromRgb(cb.R, cb.G, cb.B));
            }
        }

        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var name = string.Format("New Swatch {0}", NewSwatchIndex++);
            AddDocumentView(new SwatchView
            {
                Swatch = new SwatchViewModel(new Swatch(name))
            });
        }

        private void Settings_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (settingsPage != null && Documents.Contains(settingsPage))
            {
                CurrentDocumentView = settingsPage;
            }
            else
            {
                settingsPage = new SettingsPage();
                AddDocumentView(settingsPage);
            }
        }

        private void OpenSwatches_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Swatches");
            if (Directory.Exists(path))
            {
                ShowOpenDialog(path);
            }
        }

        private void OpenSwatches_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Swatches");
            e.CanExecute = Directory.Exists(path);
        }

        private void ShowOpenDialog(string? initialDirectory)
        {
            if (openFileDialog == null)
            {
                openFileDialog = new OpenFileDialog
                {
                    Filter = SwatchFileManage.OpenFileFilter,
                    Multiselect = true
                };
            }

            if (!string.IsNullOrEmpty(initialDirectory))
            {
                openFileDialog.InitialDirectory = initialDirectory;
            }

            openFileDialog.Title = "Open…";
            if (openFileDialog.ShowDialog(this) == true && openFileDialog.FileNames != null)
            {
                ISwatchFile? fileType = null;
                var filterIndex = openFileDialog.FilterIndex - 2;
                if (filterIndex > -1 && filterIndex < SwatchFileManage.SupportedRead.Length)
                {
                    fileType = SwatchFileManage.SupportedRead[filterIndex];
                }

                foreach (var file in openFileDialog.FileNames)
                {
                    OpenFileAsync(file, fileType);
                }
            }
        }
    }
}
