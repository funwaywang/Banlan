using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace Banlan.Controls
{
    public class Adorner : UserControl
    {
        public Adorner(Control adornedElement)
        {
            AdornedElement = adornedElement;
        }

        public Control AdornedElement { get; init; }
    }
}
