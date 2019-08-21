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
using Microsoft.Win32;

using Markdig;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.IO;
using Octokit;
using AssemblyInfoHelper.GitHubReleases;
using Semver;

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

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            setWebBrowserVersion();

            this.Icon = System.Windows.Application.Current.MainWindow.Icon;

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

            await GetAllGitHubReleases();
        }

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Set a registry key for the current user to use Internet Explorer 11 for rendering using the WebBrowser control
        /// </summary>
        //see: https://stackoverflow.com/questions/17922308/use-latest-version-of-internet-explorer-in-the-webbrowser-control
        private void setWebBrowserVersion()
        {
            RegistryKey regKey = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", RegistryKeyPermissionCheck.ReadWriteSubTree);

            string processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe";

            if (regKey.GetValue(processName) == null)
            {
                regKey.SetValue(processName, 11001, RegistryValueKind.DWord);       //11001 = Internet Explorer 11. Webpages are displayed in IE11 edge mode, regardless of the !DOCTYPE directive.
            }
        }

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Open links in the users default browser
        /// </summary>
        /// see: https://stackoverflow.com/questions/15847822/opening-web-browser-click-in-default-browser
        private void webBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.Uri != null)
            {
                e.Cancel = true;
                System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
            }
        }

        //********************************************************************************************************************************************************************

        #region GitHub releases

        private ObservableCollection<GitHubRelease> _gitHubReleases = new ObservableCollection<GitHubRelease>();
        public ObservableCollection<GitHubRelease> GitHubReleases
        {
            get { return _gitHubReleases; }
            set { _gitHubReleases = value; OnPropertyChanged(); OnPropertyChanged("NumberNewReleasesString"); }
        }

        public bool IsGitHubRepoAssigned
        {
            get { return !string.IsNullOrEmpty(AssemblyInfoHelperClass.GitHubRepoUrl); }
        }

        public string NumberNewReleasesString
        {
            get
            {
                int numberNewReleases = GitHubReleases == null ? 0 : GitHubReleases.Where(r => r.ReleaseType == GitHubReleaseTypes.NEW).Count();
                return numberNewReleases > 0 ? numberNewReleases.ToString() : ""; 
            }
        }

        private void GitHubReleases_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("NumberNewReleasesString");
        }

        //********************************************************************************************************************************************************************

        private async Task GetAllGitHubReleases()
        {
            GitHubReleases.Clear();

            if (!IsGitHubRepoAssigned) { GitHubReleases = null; return; }

            // example url: https://github.com/M1S2/AssemblyInfoHelper
            string[] urlSplitted = AssemblyInfoHelperClass.GitHubRepoUrl.Split('/');
            if(urlSplitted.Length < 5) { return; }
            string repoOwner = urlSplitted[3];
            string repoName = urlSplitted[4];

            GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue("AssemblyInfoHelper-UpdateCheck"));

            GitHubReleases.Clear();
            IReadOnlyList<Release> releases = await gitHubClient.Repository.Release.GetAll(repoOwner, repoName);

            SemVersion currentVersion = stripInitialV(AssemblyInfoHelperClass.AssemblyVersion.Substring(0, AssemblyInfoHelperClass.AssemblyVersion.LastIndexOf('.')));

            foreach (Release release in releases)
            {
                SemVersion releaseVersion = stripInitialV(release.TagName);

                GitHubReleases.Add(new GitHubRelease()
                {
                    Name = release.Name,
                    ReleaseTime = release.PublishedAt.Value.ToLocalTime(),
                    Version = stripInitialV(release.TagName),
                    ReleaseType = (releaseVersion > currentVersion ? GitHubReleaseTypes.NEW : (releaseVersion == currentVersion ? GitHubReleaseTypes.CURRENT : GitHubReleaseTypes.OLD)),
                    ReleaseURL = AssemblyInfoHelperClass.GitHubRepoUrl + "/releases/tag/" + release.TagName
                });
            }
        }

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Strip the initial "v" from the version string if existing and parse the result to a SemVersion object.
        /// </summary>
        /// <param name="version">Version string</param>
        /// <returns>SemVersion object</returns>
        /// see: https://github.com/nixxquality/GitHubUpdate/blob/master/GitHubUpdate/Helper.cs
        private SemVersion stripInitialV(string version)
        {
            if (version[0] == 'v')
                version = version.Substring(1);

            SemVersion result = SemVersion.Parse(version);

            return result;
        }

        #endregion
    }
}
