﻿using System;
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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using Microsoft.Win32;

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.IO;
using Octokit;
using AssemblyInfoHelper.GitHub;
using Semver;
using System.Net;
using System.Diagnostics;

namespace AssemblyInfoHelper
{
    /// <summary>
    /// Interaktionslogik für WindowAssemblyInfo.xaml
    /// </summary>
    public partial class WindowAssemblyInfo : MetroWindow, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementation
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// This method is called by the Set accessor of each property. The CallerMemberName attribute that is applied to the optional propertyName parameter causes the property name of the caller to be substituted as an argument.
        /// </summary>
        /// <param name="propertyName">Name of the property that is changed</param>
        /// see: https://docs.microsoft.com/de-de/dotnet/framework/winforms/how-to-implement-the-inotifypropertychanged-interface
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        //##############################################################################################################################################################################################

        /// <summary>
        /// Get the version of the AssemblyInfoHelper
        /// </summary>
        public string AssemblyInfoHelperVersion
        {
            get { return AssemblyInfoHelperClass.GetAttributeFromAssembly(System.Reflection.Assembly.GetExecutingAssembly(), AssemblyInfoHelperClass.AssemblyAttributeTypes.FILEVERSION); }
        }

        //********************************************************************************************************************************************************************

        private string _readmeMarkdown;
        /// <summary>
        /// Markdown string containing the content of the Readme file
        /// </summary>
        public string ReadmeMarkdown
        {
            get { return _readmeMarkdown; }
            set { _readmeMarkdown = value; OnPropertyChanged(); }
        }

        private bool _readmeFound = true;
        /// <summary>
        /// True if the Readme file was found
        /// </summary>
        public bool ReadmeFound
        {
            get { return _readmeFound; }
            set { _readmeFound = value; OnPropertyChanged(); }
        }

        //********************************************************************************************************************************************************************

        private string _changelogMarkdown;
        /// <summary>
        /// Markdown string containing the content of the Changelog file
        /// </summary>
        public string ChangelogMarkdown
        {
            get { return _changelogMarkdown; }
            set { _changelogMarkdown = value; OnPropertyChanged(); }
        }

        private bool _changelogFound = true;
        /// <summary>
        /// True if the Changelog file was found
        /// </summary>
        public bool ChangelogFound
        {
            get { return _changelogFound; }
            set { _changelogFound = value; OnPropertyChanged(); }
        }

        //********************************************************************************************************************************************************************

        private int _selectedTabIndex;
        /// <summary>
        /// Index of the selected Tab (General infos, Readme, Changelog, Github releases)
        /// </summary>
        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set { _selectedTabIndex = value; OnPropertyChanged(); }
        }

        //********************************************************************************************************************************************************************

        private string _readmePath;         // Path to the Readme file
        private string _changeLogPath;      // Path to the Changelog file

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Show the WindowAssemblyInfo and get the README.md and CHANGELOG.md files from the given paths.
        /// </summary>
        /// <param name="readmePath">Path for the README.md file.</param>
        /// <param name="changeLogPath">Path for the CHANGELOG.md file.</param>
        /// <param name="startTab">Tab that is shown at startup</param>
        public WindowAssemblyInfo(string readmePath, string changeLogPath, WindowAssemblyInfoStartTab startTab)
        {
            InitializeComponent();
            _readmePath = readmePath;
            _changeLogPath = changeLogPath;
            SelectedTabIndex = (int)startTab;
            this.DataContext = this;
        }

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Show the WindowAssemblyInfo and get the readme and changelog content from the README.md and CHANGELOG.md files in the same folder as the executable. (Application.StartupPath)
        /// </summary>
        /// <param name="startTab">Tab that is shown at startup</param>
        public WindowAssemblyInfo(WindowAssemblyInfoStartTab startTab)
        {
            InitializeComponent();
            string startupPath = System.AppDomain.CurrentDomain.BaseDirectory;
            _readmePath = startupPath + @"README.md";
            _changeLogPath = startupPath + @"CHANGELOG.md";
            SelectedTabIndex = (int)startTab;
            this.DataContext = this;
        }

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Show the WindowAssemblyInfo and get the readme and changelog content from the README.md and CHANGELOG.md files in the same folder as the executable. (Application.StartupPath)
        /// </summary>
        public WindowAssemblyInfo()
        {
            InitializeComponent();
            string startupPath = System.AppDomain.CurrentDomain.BaseDirectory;
            _readmePath = startupPath + @"README.md";
            _changeLogPath = startupPath + @"CHANGELOG.md";
            SelectedTabIndex = (int)WindowAssemblyInfoStartTab.GENERAL_INFOS;
            this.DataContext = this;
        }

        //********************************************************************************************************************************************************************

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Enable navigation to link URLs for MdXAML MarkdownScrollViewer (taken from https://github.com/whistyun/MdXaml/blob/master/samples/MdXaml.Demo/MainWindow.xaml.cs)
            CommandBindings.Add(new CommandBinding(
                NavigationCommands.GoToPage,
                (sender, e) =>
                {
                    Process proc = new Process();
                    proc.StartInfo.UseShellExecute = true;
                    proc.StartInfo.FileName = (string)e.Parameter;
                    proc.Start();
                }));

            this.Icon = System.Windows.Application.Current.MainWindow.Icon;

            await GitHubUtils.Instance.GetAllGitHubReleases();
            if (GitHubUtils.Instance.ErrorOccuredWhileLoadingReleases)
            {
                await this.ShowMessageAsync("Error loading GitHub releases", GitHubUtils.Instance.ErrorMessage, MessageDialogStyle.Affirmative, new MetroDialogSettings() { OwnerCanCloseWithDialog = true });
            }

            await Task.Run(() =>
            {
                ReadmeFound = File.Exists(_readmePath);
                if (ReadmeFound)
                {
                    ReadmeMarkdown = File.ReadAllText(_readmePath);
                }
                else
                {
                    ReadmeMarkdown = "No readme file found in: " + Environment.NewLine + Environment.NewLine + _readmePath;
                }

                ChangelogFound = File.Exists(_changeLogPath);
                if (ChangelogFound)
                {
                    ChangelogMarkdown = File.ReadAllText(_changeLogPath);
                }
                else
                {
                    ChangelogMarkdown = "No changelog file found in: " + Environment.NewLine + Environment.NewLine + _changeLogPath;
                }
            });
        }

    }
}
