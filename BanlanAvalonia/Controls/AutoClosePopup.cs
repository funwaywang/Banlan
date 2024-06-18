using Avalonia.Controls.Primitives;

namespace Banlan
{
    public class AutoClosePopup : Popup
    {
        public bool AutoClose { get; set; } = true;

        //protected override void OnPreviewMouseUp2(MouseButtonEventArgs e)
        //{
        //    base.OnPreviewMouseUp(e);

        //    if(!e.Handled && IsOpen)
        //    {
        //        IsOpen = false;
        //    }
        //}
    }
}
