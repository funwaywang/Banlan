using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Banlan
{
    public static class Commands
    {
        public static readonly RoutedUICommand OK = new RoutedUICommand("OK", nameof(OK), typeof(Commands));
        public static readonly RoutedUICommand Cancel = new RoutedUICommand("Cancel", nameof(Cancel), typeof(Commands));
        public static readonly RoutedUICommand BrowseFile = new RoutedUICommand("Browse File", nameof(BrowseFile), typeof(Commands));
        public static readonly RoutedUICommand CloseTab = new RoutedUICommand("Close Tab", nameof(CloseTab), typeof(Commands));
        public static readonly RoutedUICommand CloseOtherTabs = new RoutedUICommand("Close Other Tabs", nameof(CloseOtherTabs), typeof(Commands));
        public static readonly RoutedUICommand CloseAllTabs = new RoutedUICommand("Close All Tabs", nameof(CloseAllTabs), typeof(Commands));
        public static readonly RoutedUICommand Reanalyse = new RoutedUICommand("Reanalyse", nameof(Reanalyse), typeof(Commands));
        public static readonly RoutedUICommand ExtractColorsFromImage = new RoutedUICommand("Extract Colors", nameof(ExtractColorsFromImage), typeof(Commands));
        public static readonly RoutedUICommand Exit = new RoutedUICommand("Exit", nameof(Exit), typeof(Commands));
        public static readonly RoutedUICommand OpenAsOne = new RoutedUICommand("Open Multi-Files As One", nameof(OpenAsOne), typeof(Commands));
        public static readonly RoutedUICommand CopyAs = new RoutedUICommand("Copy As", nameof(CopyAs), typeof(Commands));
        public static readonly RoutedUICommand SortBy = new RoutedUICommand("Sort By", nameof(SortBy), typeof(Commands));
        public static readonly RoutedUICommand ExpandAll = new RoutedUICommand("Expand All", nameof(ExpandAll), typeof(Commands));
        public static readonly RoutedUICommand CollapseAll = new RoutedUICommand("Collapse All", nameof(CollapseAll), typeof(Commands));
        public static readonly RoutedUICommand SelectDocumentView = new RoutedUICommand("Select Document View", nameof(SelectDocumentView), typeof(Commands));
        public static readonly RoutedUICommand CopyFileName = new RoutedUICommand("Copy File Name", nameof(CopyFileName), typeof(Commands));
        public static readonly RoutedUICommand OpenContainerFolder = new RoutedUICommand("Open Container Folder", nameof(OpenContainerFolder), typeof(Commands));
        public static readonly RoutedUICommand SelectColor = new RoutedUICommand("Select Color", nameof(SelectColor), typeof(Commands));
        public static readonly RoutedUICommand SwitchColorTextFormat = new RoutedUICommand("Switch Color Text Format", nameof(SwitchColorTextFormat), typeof(Commands));
        public static readonly RoutedUICommand PasteImage = new RoutedUICommand("Paste Image", nameof(PasteImage), typeof(Commands));
        public static readonly RoutedUICommand AddColorToSwatch = new RoutedUICommand("Add to Swatch", nameof(AddColorToSwatch), typeof(Commands));
        public static readonly RoutedUICommand ShowHistoryTab = new RoutedUICommand("Show History Tab", nameof(ShowHistoryTab), typeof(Commands));
        public static readonly RoutedUICommand Settings = new RoutedUICommand("Settings", nameof(Settings), typeof(Commands));
    }
}
