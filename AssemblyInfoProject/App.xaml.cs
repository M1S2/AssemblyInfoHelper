using AssemblyInfoProject;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace AssemblyInfoProject_WPF
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            // Set the language for all FrameworkElements to the current culture
            // see: https://serialseb.com/blog/2007/04/03/wpf-tips-1-have-all-your-dates-times/
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            MainWindow window = new MainWindow();
            window.Show();
        }
    }
}
