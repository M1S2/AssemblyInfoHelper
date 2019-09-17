using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Semver;
using Octokit;
using System.Collections.ObjectModel;

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
        public static void CheckAndDisplayNewReleases()
        {
            if (Instance.AreNewReleasesAvailable)
            {
#warning Open with GitHub tab opened
                WindowAssemblyInfo window = new WindowAssemblyInfo();
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

        //####################################################################################################################################################################

        public GitHubUtils()
        {
            GitHubReleases.CollectionChanged += GitHubReleases_CollectionChanged;
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
                GitHubReleases.Clear();

                if (!IsGitHubRepoAssigned) { GitHubReleases = null; return; }

                // example url: https://github.com/M1S2/AssemblyInfoHelper
                string[] urlSplitted = AssemblyInfoHelperClass.GitHubRepoUrl.Split('/');
                if (urlSplitted.Length < 5) { return; }
                string repoOwner = urlSplitted[3];
                string repoName = urlSplitted[4];

                GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue("AssemblyInfoHelper-UpdateCheck"));
                
                IReadOnlyList<Release> releases = await gitHubClient.Repository.Release.GetAll(repoOwner, repoName);

                SemVersion currentVersion = stripInitialV(AssemblyInfoHelperClass.AssemblyVersion);
#warning Remove test code
                //currentVersion = new SemVersion(4, 1, 1);

                foreach (Release release in releases)
                {
                    SemVersion releaseVersion = stripInitialV(release.TagName);

                    GitHubReleases.Add(new GitHubRelease()
                    {
                        Name = release.Name,
                        ReleaseTime = release.CreatedAt.ToLocalTime(),
                        Version = stripInitialV(release.TagName),
                        ReleaseTimeType = (releaseVersion > currentVersion ? GitHubReleaseTimeTypes.NEW : (releaseVersion == currentVersion ? GitHubReleaseTimeTypes.CURRENT : GitHubReleaseTimeTypes.OLD)),
                        ReleaseURL = release.HtmlUrl,
                        ReleaseNotes = release.Body
#warning Set and use ReleaseType property (maybe "M", "m" and "P" in tag icon)
                    });
                }
            }
            catch (Exception ex)
            {
                //await this.ShowMessageAsync("Error loading GitHub releases", ex.Message + (ex.InnerException != null ? Environment.NewLine + Environment.NewLine + ex.InnerException.Message : ""), MessageDialogStyle.Affirmative, new MetroDialogSettings() { OwnerCanCloseWithDialog = true });
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
    }
}
