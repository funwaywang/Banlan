using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Banlan
{
    public partial class SettingsPage : DocumentView
    {
        public static readonly DependencyProperty CurrentLanguageProperty = DependencyProperty.Register(nameof(CurrentLanguage), typeof(Language), typeof(SettingsPage));

        public SettingsPage()
        {
            InitializeComponent();

            Title = "Settings";
            Lang.SetId(this, Title);
            CanClose = true;
            Initialize();
        }

        public Language CurrentLanguage
        {
            get => (Language)GetValue(CurrentLanguageProperty);
            set => SetValue(CurrentLanguageProperty, value);
        }

        private void Initialize()
        {
            CurrentLanguage = LanguageManage.Default.CurrentLanguage;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == CurrentLanguageProperty)
            {
                if (e.NewValue is Language language && language != Banlan.Language.Default)
                {
                    Settings.Default["UI-Language"] = language.Code;
                    LanguageManage.Default.SwitchLanguage(language);
                }
                else
                {
                    Settings.Default.Remove("UI-Language");
                    LanguageManage.Default.SwitchLanguage(Banlan.Language.Default);
                }
            }
        }
    }
}
