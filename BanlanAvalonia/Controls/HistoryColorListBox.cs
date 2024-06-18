using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using Avalonia.Controls;
using Avalonia.Media;

namespace Banlan
{
    public class HistoryColorListBox : ListBox
    {
        public HistoryColorListBox() 
        {
            SelectionChanged += HistoryColorListBox_SelectionChanged;
        }

        private void HistoryColorListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (SelectedItem is Color color)
            {
                AppStatus.Default.SelectColor(color);
            }
        }
    }
}
