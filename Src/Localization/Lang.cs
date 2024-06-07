using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Banlan
{
    public static class Lang
    {
        public static readonly DependencyProperty IdProperty = DependencyProperty.RegisterAttached("Id", typeof(LangId), typeof(Lang), new FrameworkPropertyMetadata(OnLangIdChanged));
        public static readonly DependencyProperty IdsProperty = DependencyProperty.RegisterAttached("Ids", typeof(string), typeof(Lang), new FrameworkPropertyMetadata(OnLangIdsChanged));
        public static readonly DependencyProperty LocalizableProperty = DependencyProperty.RegisterAttached("Localizable", typeof(bool), typeof(Lang), new FrameworkPropertyMetadata(OnLocalizableChanged));

        public static void SetId(DependencyObject element, LangId value)
        {
            element.SetValue(IdProperty, value);
        }

        public static LangId GetId(DependencyObject element)
        {
            return (LangId)element.GetValue(IdProperty);
        }

        public static void SetIds(DependencyObject element, string value)
        {
            element.SetValue(IdsProperty, value);
        }

        public static string GetIds(DependencyObject element)
        {
            return (string)element.GetValue(IdsProperty);
        }

        public static void SetLocalizable(DependencyObject element, bool value)
        {
            element.SetValue(LocalizableProperty, value);
        }

        public static bool GetLocalizable(DependencyObject element)
        {
            return (bool)element.GetValue(LocalizableProperty);
        }

        private static void OnLangIdChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is LangId id)
            {
                RefreshElement(sender, id);
            }
        }

        private static void OnLangIdsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is UIElement uiElement)
            {
                if (e.NewValue is string idString)
                {
                    SetId(uiElement, new LangId(idString));
                    RefreshElement(sender, idString);
                }
            }
        }

        private static void OnLocalizableChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool value)
            {
                if (value)
                {
                    if (sender is TextBlock textBlock)
                    {
                        SetId(sender, textBlock.Text);
                    }
                    else if (sender is HeaderedItemsControl control && control.Header is string header)
                    {
                        SetId(sender, header);
                    }
                    else if (sender is ContentControl contentControl && contentControl.Content is string content)
                    {
                        SetId(sender, content);
                    }
                    else if (sender is Run run)
                    {
                        SetId(sender, run.Text);
                    }
                    else if (sender is Window window)
                    {
                        SetId(sender, window.Title);
                    }
                }
                else
                {
                    sender.ClearValue(IdProperty);
                }
            }
        }

        private static void RefreshElement(DependencyObject element, LangId id)
        {
            var text = LanguageManage.Default.CurrentLanguage?.GetText(id);
            if (text == null)
            {
                return;
            }

            if (element is ILocalizable localizable)
            {
                localizable.SetLocalizedText(text);
            }
            else if (element is TextBlock textBlock)
            {
                textBlock.Text = text;
            }
            else if (element is HeaderedItemsControl control)
            {
                control.Header = text;
            }
            else if (element is Run run)
            {
                run.Text = text;
            }
            else if (element is Window window)
            {
                window.Title = text;
            }
            else if (element is ContentControl contentControl)
            {
                contentControl.Content = text;
            }
        }

        [return: NotNullIfNotNull(nameof(name))]
        public static string? GetText(string? name)
        {
            return LanguageExtensions.GetText(LanguageManage.Default.CurrentLanguage ?? Language.Default, name);
        }

        public static string GetText(string name, string defaultValue)
        {
            return LanguageExtensions.GetText(LanguageManage.Default.CurrentLanguage ?? Language.Default, name, defaultValue);
        }

        public static string? GetTextAny(string name, params string[] names)
        {
            return LanguageExtensions.GetTextAny(LanguageManage.Default.CurrentLanguage ?? Language.Default, name, names);
        }

        public static string? GetTextWithEllipsis(string name)
        {
            return LanguageExtensions.GetTextWithEllipsis(LanguageManage.Default.CurrentLanguage ?? Language.Default, name);
        }

        public static string? GetTextWithColon(string name)
        {
            return LanguageExtensions.GetTextWithColon(LanguageManage.Default.CurrentLanguage ?? Language.Default, name);
        }

        public static string? GetTextWithAccelerator(string name, bool withEllipsis, char accelerator)
        {
            return LanguageExtensions.GetTextWithAccelerator(LanguageManage.Default.CurrentLanguage ?? Language.Default, name, accelerator);
        }

        public static string? GetTextWithAccelerator(string name, char accelerator)
        {
            return LanguageExtensions.GetTextWithAccelerator(LanguageManage.Default.CurrentLanguage ?? Language.Default, name, accelerator);
        }

        public static string? GetText(LangId langId)
        {
            return LanguageExtensions.GetText(LanguageManage.Default.CurrentLanguage ?? Language.Default, langId);
        }

        public static string? GetText(string name, bool withEllipsis, bool withColon, char accelerator)
        {
            return LanguageExtensions.GetText(LanguageManage.Default.CurrentLanguage ?? Language.Default, name);
        }

        public static string Format(string name, params object[] args)
        {
            return LanguageExtensions.Format(LanguageManage.Default.CurrentLanguage ?? Language.Default, name, args);
        }

        public static string Format(string name, bool withArgs, params object[] args)
        {
            return LanguageExtensions.Format(LanguageManage.Default.CurrentLanguage ?? Language.Default, name, withArgs, args);
        }
    }
}
