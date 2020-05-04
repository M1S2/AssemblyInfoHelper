using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using Semver;
using Octokit;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Net.Http;
using System.Net;
using System.Threading;
using System.Reflection;

namespace AssemblyInfoHelper.GitHub
{
    public class GitHubUtils : INotifyPropertyChanged
    {
        #region Singleton
        private static GitHubUtils _instance = null;
        public static GitHubUtils Instance => _instance ?? (_instance = new GitHubUtils());
        #endregion

        //####################################################################################################################################################################

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

        //####################################################################################################################################################################

        /// <summary>
        /// Check if new releases are available and show them if available
        /// </summary>
        public async static Task CheckAndDisplayNewReleases()
        {
            await Instance.SemaphoreGetReleases.WaitAsync(30000);
            if (Instance.AreNewReleasesAvailable && Instance.UserEnableDisableReleaseNotification && Instance.AppEnableDisableReleaseNotification)
            {
                WindowAssemblyInfo window = new WindowAssemblyInfo(WindowAssemblyInfoStartTab.GITHUB);
                window.ShowDialog();
            }
        }

        //####################################################################################################################################################################

        private ObservableCollection<GitHubRelease> _gitHubReleases = new ObservableCollection<GitHubRelease>();
        /// <summary>
        /// List with all GitHub releases
        /// </summary>
        public ObservableCollection<GitHubRelease> GitHubReleases
        {
            get { return _gitHubReleases; }
            set { _gitHubReleases = value; OnPropertyChanged(); OnPropertyChanged("NumberNewReleases"); OnPropertyChanged("NumberNewReleasesString"); }
        }

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Is a GitHubRepo attribute assigned or not
        /// </summary>
        public bool IsGitHubRepoAssigned
        {
            get { return !string.IsNullOrEmpty(AssemblyInfoHelperClass.GitHubRepoUrl); }
        }

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Are new releases available
        /// </summary>
        public bool AreNewReleasesAvailable
        {
            get { return NumberNewReleases > 0; }
        }

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Number of new releases.
        /// </summary>
        public int NumberNewReleases
        {
            get
            {
                return GitHubReleases == null ? 0 : GitHubReleases.Where(r => r.ReleaseTimeType == GitHubReleaseTimeTypes.NEW).Count();
            }
        }

        /// <summary>
        /// String containing the number of new releases. Empty string if number equals 0
        /// </summary>
        public string NumberNewReleasesString
        {
            get
            {
                return NumberNewReleases > 0 ? NumberNewReleases.ToString() : "";
            }
        }

        private void GitHubReleases_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("NumberNewReleases");
            OnPropertyChanged("NumberNewReleasesString");
        }

        //********************************************************************************************************************************************************************

        private bool _errorOccuredWhileLoadingReleases;
        /// <summary>
        /// Is an error occured while loading the releases
        /// </summary>
        public bool ErrorOccuredWhileLoadingReleases
        {
            get { return _errorOccuredWhileLoadingReleases; }
            private set { _errorOccuredWhileLoadingReleases = value; OnPropertyChanged(); }
        }

        private string _errorMessage;
        /// <summary>
        /// Error message if error is occured. Otherwise empty string.
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
            private set { _errorMessage = value; OnPropertyChanged(); }
        }

        //********************************************************************************************************************************************************************

        private bool _appEnableDisableReleaseNotification;
        /// <summary>
        /// If this is set to false, no notifications about new releases are shown. The user has no option to enable or disable notification about new releases.
        /// </summary>
        public bool AppEnableDisableReleaseNotification
        {
            get { return _appEnableDisableReleaseNotification; }
            set { _appEnableDisableReleaseNotification = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Has the user enabled the notification about new releases or not
        /// </summary>
        public bool UserEnableDisableReleaseNotification
        {
            get { return _settingsHelper.GetAppSetting<bool>("UserEnableDisableReleaseNotification", true); }
            set { _settingsHelper.SetOrCreateAppSetting("UserEnableDisableReleaseNotification", value); OnPropertyChanged(); }
        }

        //********************************************************************************************************************************************************************

        private ICommand _updateCommand;
        /// <summary>
        /// Command to update (or downgrade) to the version assigned as command parameter
        /// </summary>
        public ICommand UpdateCommand
        {
            get
            {
                if (_updateCommand == null)
                {
                    _updateCommand = new RelayCommand(async param => await UpdateUtils.RunUpdate((GitHubRelease)param), ret => !UpdateUtils.UpdateStatus.IsUpdateRunning);
                }
                return _updateCommand;
            }
        }

        //####################################################################################################################################################################

        private SettingsHelper _settingsHelper;
        public System.Threading.SemaphoreSlim SemaphoreGetReleases;

        //####################################################################################################################################################################

        public GitHubUtils()
        {
            _settingsHelper = new SettingsHelper(this.GetType().Assembly);
            GitHubReleases.CollectionChanged += GitHubReleases_CollectionChanged;
            SemaphoreGetReleases = new System.Threading.SemaphoreSlim(0, 1);
            Task.Run(async() => await GetAllGitHubReleases());
        }

        //####################################################################################################################################################################

        /// <summary>
        /// Get all releases from the GitHub repository
        /// </summary>
        /// see: https://github.com/nixxquality/GitHubUpdate
        public async Task GetAllGitHubReleases()
        {
            try
            {
                ErrorOccuredWhileLoadingReleases = false;
                ErrorMessage = string.Empty;
                GitHubReleases.Clear();
                
                if (!IsGitHubRepoAssigned) { GitHubReleases = null; return; }

                // example url: https://github.com/M1S2/AssemblyInfoHelper
                string[] urlSplitted = AssemblyInfoHelperClass.GitHubRepoUrl.Split('/');
                if (urlSplitted.Length < 5) { return; }
                string repoOwner = urlSplitted[3];
                string repoName = urlSplitted[4];

                GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue("AssemblyInfoHelper-UpdateCheck"));

                List<Release> originalReleases = new List<Release>(await gitHubClient.Repository.Release.GetAll(repoOwner, repoName));

                SemVersion currentVersion = stripInitialV(AssemblyInfoHelperClass.AssemblyVersion);                
                SemVersion previousVersion = new SemVersion(0, 0, 0);
                originalReleases.Reverse();

                List<GitHubRelease> tmpGitHubReleases = new List<GitHubRelease>();

                foreach (Release release in originalReleases)
                {
                    SemVersion releaseVersion = stripInitialV(release.TagName);

                    tmpGitHubReleases.Add(new GitHubRelease()
                    {
                        Name = release.Name,
                        ReleaseTime = release.CreatedAt.ToLocalTime(),
                        Version = releaseVersion,
                        ReleaseTimeType = (releaseVersion > currentVersion ? GitHubReleaseTimeTypes.NEW : (releaseVersion == currentVersion ? GitHubReleaseTimeTypes.CURRENT : GitHubReleaseTimeTypes.OLD)),
                        ReleaseURL = release.HtmlUrl,
                        ReleaseNotes = release.Body,
                        ReleaseType = getReleaseTypeFromVersions(releaseVersion, previousVersion),

                        /* Asset Names should be:
                           For binaries: %ProjectName%_Binaries.zip, %ProjectName%.zip, %ProjectName%_v1.0.0.zip, bin.zip
                           For installer: %ProjectName%_Installer.zip, Installer.zip, Setup.zip, Setup.exe 
                        */
                        BinAsset = release.Assets.Where(a => a.Name.ToLower().Contains("bin") || (a.Name.ToLower().StartsWith(AssemblyInfoHelperClass.AssemblyTitle.ToLower()) && !a.Name.ToLower().Contains("inst") && !a.Name.ToLower().Contains("setup"))).FirstOrDefault(),
                        InstallerAsset = release.Assets.Where(a => a.Name.ToLower().Contains("inst") || a.Name.ToLower().Contains("setup")).FirstOrDefault()
                    });

                    previousVersion = releaseVersion;
                }

                tmpGitHubReleases.Reverse();
                foreach(GitHubRelease r in tmpGitHubReleases) { GitHubReleases.Add(r); }

                if (SemaphoreGetReleases.CurrentCount == 0) { SemaphoreGetReleases.Release(); }
            }
            catch (Exception ex)
            {
                ErrorOccuredWhileLoadingReleases = true;
                ErrorMessage = ex.Message + (ex.InnerException != null ? Environment.NewLine + ex.InnerException.Message : "");
                if (SemaphoreGetReleases.CurrentCount == 0) { SemaphoreGetReleases.Release(); }
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

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Compare the two versions and determine if it's a major, minor or patch release
        /// </summary>
        /// <param name="version1">Version 1</param>
        /// <param name="version2">Version 2</param>
        /// <returns>Release type</returns>
        private GitHubReleaseTypes getReleaseTypeFromVersions(SemVersion version1, SemVersion version2)
        {
            if(version1.Major != version2.Major)
            {
                return GitHubReleaseTypes.MAJOR;
            }
            else if(version1.Minor != version2.Minor)
            {
                return GitHubReleaseTypes.MINOR;
            }
            else if(version1.Patch != version2.Patch)
            {
                return GitHubReleaseTypes.PATCH;
            }
            else
            {
                return GitHubReleaseTypes.NONE;
            }
        }

    }
}
