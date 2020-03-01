using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Banlan
{
    public class DropDownMenuButton : ToggleButton
    {
        public static readonly DependencyProperty MenuProperty = DependencyProperty.Register(nameof(Menu), typeof(ContextMenu), typeof(DropDownMenuButton));
        public static readonly DependencyProperty PlacementTargetProperty = DependencyProperty.Register(nameof(PlacementTarget), typeof(UIElement), typeof(DropDownMenuButton));
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register(nameof(Placement), typeof(PlacementMode), typeof(DropDownMenuButton), new PropertyMetadata(PlacementMode.Bottom));

        static DropDownMenuButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownMenuButton), new FrameworkPropertyMetadata(typeof(DropDownMenuButton)));
        }

        public DropDownMenuButton()
        {
            PlacementTarget = this;
        }

        public ContextMenu Menu
        {
            get => (ContextMenu)GetValue(MenuProperty);
            set => SetValue(MenuProperty, value);
        }

        public UIElement PlacementTarget
        {
            get => (UIElement)GetValue(PlacementTargetProperty);
            set => SetValue(PlacementTargetProperty, value);
        }

        public PlacementMode Placement
        {
            get => (PlacementMode)GetValue(PlacementProperty);
            set => SetValue(PlacementProperty, value);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == MenuProperty)
            {
                if (e.NewValue is ContextMenu menu)
                {
                    menu.SetBinding(ContextMenu.IsOpenProperty, new Binding { Source = this, Path = new PropertyPath(nameof(IsChecked)) });
                    menu.SetBinding(ContextMenu.PlacementTargetProperty, new Binding { Source = this, Path = new PropertyPath(nameof(PlacementTarget)), Mode = BindingMode.OneWay });
                    menu.SetBinding(ContextMenu.PlacementProperty, new Binding { Source = this, Path = new PropertyPath(nameof(Placement)), Mode = BindingMode.OneWay });
                    menu.SetBinding(DataContextProperty, new Binding { Source = this, Path = new PropertyPath(nameof(DataContext)), Mode = BindingMode.OneWay });
                }
            }
        }
    }
}
