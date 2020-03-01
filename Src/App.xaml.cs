using Banlan.SwatchFiles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace Banlan
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // load settings
                Settings.Default.Load();

                // load & select language
                LanguageManage.Default.LoadFolder(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Langs"));
                LanguageManage.Default.LoadFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(Banlan), "Langs"));
                LanguageManage.Default.SelectOrDefault(Settings.Default["UI-Language"]);

                // init AppStatus
                AppStatus.Default.Startup();
            }
            catch
            {
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            try
            {
                AppStatus.Default.Exit();
                Settings.Default.Save();
            }
            catch
            {
            }
        }
    }
}
