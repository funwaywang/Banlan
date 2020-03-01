using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Banlan
{
    public static class DependencyObjectExtensions
    {
        public static void RunInUIThread(this DependencyObject dependencyObject, Action action)
        {
            dependencyObject.Dispatcher.BeginInvoke(action, System.Windows.Threading.DispatcherPriority.Background);
        }
    }
}
