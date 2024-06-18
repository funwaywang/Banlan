using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace Banlan
{
    public class DropDownMenuButton : ToggleButton
    {
        public static readonly StyledProperty<ContextMenu?> MenuProperty = AvaloniaProperty.Register<DropDownMenuButton, ContextMenu?>(nameof(Menu));
        public static readonly StyledProperty<Control?> PlacementTargetProperty = AvaloniaProperty.Register<DropDownMenuButton, Control?>(nameof(PlacementTarget));
        public static readonly StyledProperty<PlacementMode> PlacementProperty = AvaloniaProperty.Register<DropDownMenuButton, PlacementMode>(nameof(Placement), PlacementMode.Bottom);

        public DropDownMenuButton()
        {
            PlacementTarget = this;
        }

        public ContextMenu? Menu
        {
            get => GetValue(MenuProperty);
            set => SetValue(MenuProperty, value);
        }

        public Control? PlacementTarget
        {
            get => GetValue(PlacementTargetProperty);
            set => SetValue(PlacementTargetProperty, value);
        }

        public PlacementMode Placement
        {
            get => GetValue(PlacementProperty);
            set => SetValue(PlacementProperty, value);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == MenuProperty)
            {
                if (e.NewValue is ContextMenu menu)
                {
                    menu.Bind(ContextMenu.IsOpenProperty, new Binding { Source = this, Path = nameof(IsChecked) });
                    menu.Bind(ContextMenu.PlacementTargetProperty, new Binding { Source = this, Path = nameof(PlacementTarget), Mode = BindingMode.OneWay });
                    menu.Bind(ContextMenu.PlacementProperty, new Binding { Source = this, Path = nameof(Placement), Mode = BindingMode.OneWay });
                    menu.Bind(DataContextProperty, new Binding { Source = this, Path = nameof(DataContext), Mode = BindingMode.OneWay });
                }
            }
        }
    }
}
