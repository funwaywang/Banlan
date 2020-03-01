using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Banlan
{
    public class AutoClosePopup : Popup
    {
        public bool AutoClose { get; set; } = true;

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);

            if(!e.Handled && IsOpen)
            {
                IsOpen = false;
            }
        }
    }
}
