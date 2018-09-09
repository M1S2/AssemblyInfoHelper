using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Markdig;
using System.IO;

namespace AssemblyInfoHelper_WPF
{
    /// <summary>
    /// Interaktionslogik für WindowAssemblyInfo.xaml
    /// </summary>
    public partial class WindowAssemblyInfo : Window
    {
        private string _readmePath;
        private string _changeLogPath;

        /// <summary>
        /// Show the WindowAssemblyInfo and get the README.md and CHANGELOG.md files from the given paths.
        /// </summary>
        /// <param name="readmePath">Path for the README.md file.</param>
        /// <param name="changeLogPath">Path for the CHANGELOG.md file.</param>
        public WindowAssemblyInfo(string readmePath, string changeLogPath)
        {
            InitializeComponent();
            _readmePath = readmePath;
            _changeLogPath = changeLogPath;
        }

        /// <summary>
        /// Show the WindowAssemblyInfo and get the readme and changelog content from the README.md and CHANGELOG.md files in the same folder as the executable. (Application.StartupPath)
        /// </summary>
        public WindowAssemblyInfo()
        {
            InitializeComponent();
            string startupPath = System.AppDomain.CurrentDomain.BaseDirectory;
            _readmePath = startupPath + @"README.md";
            _changeLogPath = startupPath + @"CHANGELOG.md";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Icon = Application.Current.MainWindow.Icon;

            MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            string readmeText = "<font face = \"calibri\">";
            string changelogText = "<font face = \"calibri\">";

            if (File.Exists(_readmePath))
            {
                readmeText += Markdig.Markdown.ToHtml(File.ReadAllText(_readmePath), pipeline);
            }
            else
            {
                readmeText += "No readme file found in: <br><br>" + _readmePath;
            }

            if (File.Exists(_changeLogPath))
            {
                changelogText += Markdig.Markdown.ToHtml(File.ReadAllText(_changeLogPath), pipeline);
            }
            else
            {
                changelogText += "No changelog file found in: <br><br>" + Environment.NewLine + _changeLogPath;
            }

            webBrowser_Readme.NavigateToString(readmeText);
            webBrowser_Changelog.NavigateToString(changelogText);
        }
    }
}
