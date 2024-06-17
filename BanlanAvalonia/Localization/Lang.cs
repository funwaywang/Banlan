using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;

namespace Banlan
{
    public class Lang : AvaloniaObject
    {
        public static readonly AttachedProperty<LangId> IdProperty = AvaloniaProperty.RegisterAttached<Lang, AvaloniaObject, LangId>("Id");
        public static readonly AttachedProperty<string> IdsProperty = AvaloniaProperty.RegisterAttached<Lang, AvaloniaObject, string>("Ids");
        public static readonly AttachedProperty<bool> LocalizableProperty = AvaloniaProperty.RegisterAttached<Lang, AvaloniaObject, bool>("Localizable");

        static Lang()
        {
            IdProperty.Changed.AddClassHandler<AvaloniaObject>(OnLangIdChanged);
            IdProperty.Changed.AddClassHandler<AvaloniaObject>(OnLangIdsChanged);
            IdProperty.Changed.AddClassHandler<AvaloniaObject>(OnLocalizableChanged);
        }

        public static void SetId(AvaloniaObject element, LangId value)
        {
            element.SetValue(IdProperty, value);
        }

        public static LangId GetId(AvaloniaObject element)
        {
            return (LangId)element.GetValue(IdProperty);
        }

        public static void SetIds(AvaloniaObject element, string value)
        {
            element.SetValue(IdsProperty, value);
        }

        public static string GetIds(AvaloniaObject element)
        {
            return (string)element.GetValue(IdsProperty);
        }

        public static void SetLocalizable(AvaloniaObject element, bool value)
        {
            element.SetValue(LocalizableProperty, value);
        }

        public static bool GetLocalizable(AvaloniaObject element)
        {
            return (bool)element.GetValue(LocalizableProperty);
        }

        private static void OnLangIdChanged(AvaloniaObject target, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is LangId id)
            {
                RefreshElement(target, id);
            }
        }

        private static void OnLangIdsChanged(AvaloniaObject target, AvaloniaPropertyChangedEventArgs e)
        {
            if (target != null)
            {
                if (e.NewValue is string idString)
                {
                    SetId(target, new LangId(idString));
                    RefreshElement(target, idString);
                }
            }
        }

        private static void OnLocalizableChanged(AvaloniaObject target, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool value)
            {
                if (value)
                {
                    if (target is TextBlock textBlock)
                    {
                        if (textBlock.Text != null)
                        {
                            SetId(target, new LangId(textBlock.Text));
                        }
                        else
                        {
                            target.ClearValue(IdProperty);
                        }
                    }
                    else if (target is HeaderedItemsControl control && control.Header is string header)
                    {
                        SetId(target, header);
                    }
                    else if (target is ContentControl contentControl && contentControl.Content is string content)
                    {
                        SetId(target, content);
                    }
                    else if (target is Run run)
                    {
                        if (run.Text != null)
                        {
                            SetId(target, run.Text);
                        }
                        else
                        {
                            target.ClearValue(IdProperty);
                        }
                    }
                    else if (target is Window window)
                    {
                        if (window.Title != null)
                        {
                            SetId(target, window.Title);
                        }
                        else
                        {
                            target.ClearValue(IdProperty);
                        }
                    }
                }
                else
                {
                    target.ClearValue(IdProperty);
                }
            }
        }

        private static void RefreshElement(AvaloniaObject element, LangId id)
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
