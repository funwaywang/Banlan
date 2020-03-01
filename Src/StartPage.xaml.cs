using System;
using System.Collections.Generic;
using System.Text;
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
    public partial class StartPage : DocumentView
    {
        public StartPage()
        {
            InitializeComponent();

            CanClose = false;
            Title = "Start";
            Lang.SetId(this, Title);
        }

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is SwatchFileSummary file && Settings.Default.RecentFiles.Contains(file))
            {
                Settings.Default.RecentFiles.Remove(file);
            }
        }
    }
}
