using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace Banlan
{
    public class HistoryColorListBox : ListBox
    {
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (SelectedItem is Color color)
            {
                AppStatus.Default.SelectColor(color);
            }
        }
    }
}
