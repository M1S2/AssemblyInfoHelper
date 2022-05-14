using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AssemblyInfoHelper.Updater
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    /// see: https://github.com/Tyrrrz/Onova/blob/master/Onova.Updater/Program.cs
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string updateeFilePath = (e.Args.Length >= 1 ? e.Args[0] : "");
            string packageContentDirPath = (e.Args.Length >= 2 ? e.Args[1] : "");
            bool restartUpdatee = (e.Args.Length >= 3 ? bool.Parse(e.Args[2]) : true);
            string routedArgs = (e.Args.Length >= 4 ? Encoding.UTF8.GetString(Convert.FromBase64String(e.Args[3])) : "");

            UpdaterWindow updaterWindow = new UpdaterWindow(updateeFilePath, packageContentDirPath, restartUpdatee, routedArgs);
            updaterWindow.Show();
            updaterWindow.RunUpdate();
        }
    }
}
