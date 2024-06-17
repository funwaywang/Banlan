using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Avalonia.Controls;
using Avalonia.Threading;

namespace Banlan
{
    public static class DependencyObjectExtensions
    {
        [Obsolete]
        public static void RunInUIThread(this Control dependencyObject, Action action)
        {
            // dependencyObject.Dispatcher.BeginInvoke(action, System.Windows.Threading.DispatcherPriority.Background);

            Dispatcher.UIThread.Post(action);
        }
    }
}
