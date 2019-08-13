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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

using Markdig;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.IO;
using Octokit;

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

        public string AssemblyInfoHelperVersion
        {
            get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        //********************************************************************************************************************************************************************

        private ICommand _assemblyInfoHelperVersionCommand;
        public ICommand AssemblyInfoHelperVersionCommand
        {
            get
            {
                if (_assemblyInfoHelperVersionCommand == null)
                {
                    _assemblyInfoHelperVersionCommand = new RelayCommand(async param =>
                    {
                        await this.ShowMessageAsync("AssemblyInfoHelper Version", AssemblyInfoHelperVersion, MessageDialogStyle.Affirmative, new MetroDialogSettings() { OwnerCanCloseWithDialog = true });
                    });
                }
                return _assemblyInfoHelperVersionCommand;
            }
        }

        //********************************************************************************************************************************************************************

        private ObservableCollection<Release> _gitHubReleases = new ObservableCollection<Release>();
        public ObservableCollection<Release> GitHubReleases
        {
            get { return _gitHubReleases; }
            set { _gitHubReleases = value; OnPropertyChanged(); OnPropertyChanged("AreGitHubReleasesAvailable"); }
        }

        public bool AreGitHubReleasesAvailable
        {
            get { return GitHubReleases != null && GitHubReleases.Count > 0; }
        }

        private void GitHubReleases_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("AreGitHubReleasesAvailable");
        }

        //********************************************************************************************************************************************************************

        private string _readmePath;
        private string _changeLogPath;

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Show the WindowAssemblyInfo and get the README.md and CHANGELOG.md files from the given paths.
        /// </summary>
        /// <param name="readmePath">Path for the README.md file.</param>
        /// <param name="changeLogPath">Path for the CHANGELOG.md file.</param>
        public WindowAssemblyInfo(string readmePath, string changeLogPath)
        {
            GitHubReleases.CollectionChanged += GitHubReleases_CollectionChanged;
            InitializeComponent();
            _readmePath = readmePath;
            _changeLogPath = changeLogPath;
            this.DataContext = this;
        }

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Show the WindowAssemblyInfo and get the readme and changelog content from the README.md and CHANGELOG.md files in the same folder as the executable. (Application.StartupPath)
        /// </summary>
        public WindowAssemblyInfo()
        {
            GitHubReleases.CollectionChanged += GitHubReleases_CollectionChanged;
            InitializeComponent();
            string startupPath = System.AppDomain.CurrentDomain.BaseDirectory;
            _readmePath = startupPath + @"README.md";
            _changeLogPath = startupPath + @"CHANGELOG.md";
            this.DataContext = this;
        }

        //********************************************************************************************************************************************************************

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Icon = System.Windows.Application.Current.MainWindow.Icon;

            MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

            string readmeText = "<font face = \"calibri\">";
            //string readmeText = "<!DOCTYPE html><html><head><meta http - equiv = \"X-UA-Compatible\" content = \"IE=Edge\"/></head><body><font face = \"calibri\">";
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

            //readmeText += "</body></html>";

            webBrowser_Readme.NavigateToString(readmeText);
            webBrowser_Changelog.NavigateToString(changelogText);

            GetAllGitHubReleases();
        }

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Open links in the users default browser
        /// </summary>
        /// see: https://stackoverflow.com/questions/15847822/opening-web-browser-click-in-default-browser
        //private void webBrowser_Readme_Navigating(object sender, NavigatingCancelEventArgs e)
        //{
        //    //e.Cancel = true;
        //    //if (e.Uri != null) { System.Diagnostics.Process.Start(e.Uri.AbsoluteUri); }
        //}

        //********************************************************************************************************************************************************************

        private void GetAllGitHubReleases()
        {
            if(AssemblyInfoHelperClass.GitHubRepo == null) { return; }

            GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue("AssemblyInfoHelper-UpdateCheck"));

            GitHubReleases.Clear();
            List<Release> releases = gitHubClient.Repository.Release.GetAll(AssemblyInfoHelperClass.GitHubRepo.RepoOwner, AssemblyInfoHelperClass.GitHubRepo.RepoName).Result.ToList();
            foreach (Release release in releases) { GitHubReleases.Add(release); }
        }
    }
}
