using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Banlan
{
    public class NumericUpDown : Control
    {
        private RepeatButton? buttonUp;
        private RepeatButton? buttonDown;
        public static readonly DependencyProperty MinimumValueProperty = DependencyProperty.Register(nameof(MinimumValue), typeof(int), typeof(NumericUpDown), new PropertyMetadata(0));
        public static readonly DependencyProperty MaximumValueProperty = DependencyProperty.Register(nameof(MaximumValue), typeof(int), typeof(NumericUpDown), new PropertyMetadata(100));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(int), typeof(NumericUpDown), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));
        }

        public int MinimumValue
        {
            get => (int)GetValue(MinimumValueProperty);
            set => SetValue(MinimumValueProperty, value);
        }

        public int MaximumValue
        {
            get => (int)GetValue(MaximumValueProperty);
            set => SetValue(MaximumValueProperty, value);
        }

        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // textBox = GetTemplateChild("PART_TextBox") as TextBox;
            buttonUp = GetTemplateChild("PART_UpButton") as RepeatButton;
            buttonDown = GetTemplateChild("PART_DownButton") as RepeatButton;

            if (buttonUp != null)
            {
                buttonUp.Click += ButtonUp_Click;
            }

            if (buttonDown != null)
            {
                buttonDown.Click += ButtonDown_Click;
            }
        }

        private void ButtonDown_Click(object sender, RoutedEventArgs e)
        {
            Value = Math.Max(MinimumValue, Value - 1);
        }

        private void ButtonUp_Click(object sender, RoutedEventArgs e)
        {
            Value = Math.Min(MaximumValue, Value + 1);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            switch (e.Key)
            {
                case Key.Up:
                    Value = Math.Min(MaximumValue, Value + 1);
                    break;
                case Key.Down:
                    Value = Math.Max(MinimumValue, Value - 1);
                    break;
            }
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            base.OnPreviewMouseWheel(e);

            Value = Math.Min(MaximumValue, Math.Max(MinimumValue, Value + e.Delta / 120));
        }
    }
}
