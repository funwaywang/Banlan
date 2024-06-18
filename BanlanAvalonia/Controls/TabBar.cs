using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Banlan
{
    public class TabBar : ListBox
    {
        private ScrollViewer? scrollViewer;
        public static readonly DependencyProperty IsOverflowProperty = DependencyProperty.Register(nameof(IsOverflow), typeof(bool), typeof(TabBar), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty CanScrollLeftProperty = DependencyProperty.Register(nameof(CanScrollLeft), typeof(bool), typeof(TabBar), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty CanScrollRightProperty = DependencyProperty.Register(nameof(CanScrollRight), typeof(bool), typeof(TabBar), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));

        static TabBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabBar), new FrameworkPropertyMetadata(typeof(TabBar)));
        }

        public bool IsOverflow
        {
            get => (bool)GetValue(IsOverflowProperty);
            set => SetValue(IsOverflowProperty, value);
        }

        public bool CanScrollLeft
        {
            get => (bool)GetValue(CanScrollLeftProperty);
            set => SetValue(CanScrollLeftProperty, value);
        }

        public bool CanScrollRight
        {
            get => (bool)GetValue(CanScrollRightProperty);
            set => SetValue(CanScrollRightProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            scrollViewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
            if (scrollViewer != null)
            {
                scrollViewer.SizeChanged += ScrollViewer_SizeChanged;
                scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
            }
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            RefreshScrollInfo();
        }

        private void ScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshScrollInfo();
        }

        private void RefreshScrollInfo()
        {
            if (scrollViewer != null)
            {
                IsOverflow = scrollViewer.ScrollableWidth > 0;
                CanScrollLeft = scrollViewer.ScrollableWidth > 0 && scrollViewer.HorizontalOffset > 0;
                CanScrollRight = scrollViewer.ScrollableWidth > 0 && scrollViewer.HorizontalOffset < scrollViewer.ScrollableWidth;
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (SelectedItem != null)
            {
                ScrollIntoView(SelectedItem);
            }
        }
    }
}
