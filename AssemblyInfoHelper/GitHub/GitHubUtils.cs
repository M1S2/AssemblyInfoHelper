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


                List<Release> releases = new List<Release>(await gitHubClient.Repository.Release.GetAll(repoOwner, repoName));
                SemVersion currentVersion = stripInitialV(AssemblyInfoHelperClass.AssemblyVersion);
                
                /*
                #warning TestCode !!! (Uncomment line above when ready)
                #region TestCode
                List<Release> releases = new List<Release>();
                releases.Add(new Release("www.google.de", "www.google.de", "www.google.de", "www.google.de", 5, "Node5", "v3.0.0", "abcdej", "Release v3.0.0", "#5", false, false, DateTimeOffset.Now, DateTimeOffset.Now, new Author(), "", "", null));
                releases.Add(new Release("www.google.de", "www.google.de", "www.google.de", "www.google.de", 4, "Node4", "v2.1.1", "abcdei", "Release v2.1.1", "#4", false, false, DateTimeOffset.Now, DateTimeOffset.Now, new Author(), "", "", null));
                releases.Add(new Release("www.google.de", "www.google.de", "www.google.de", "www.google.de", 3, "Node3", "v2.1.0", "abcdeh", "Release v2.1.0", "Rel 3", false, false, DateTimeOffset.Now, DateTimeOffset.Now, new Author(), "", "", null));
                releases.Add(new Release("www.google.de", "www.google.de", "www.google.de", "www.google.de", 2, "Node2", "v2.0.0", "abcdeg", "Release v2.0.0", "Release 2", false, false, DateTimeOffset.Now, DateTimeOffset.Now, new Author(), "", "", null));
                releases.Add(new Release("www.google.de", "www.google.de", "www.google.de", "www.google.de", 1, "Node1", "v1.0.0", "abcdef", "Release v1.0.0", "**TestNote**", false, false, DateTimeOffset.Now, DateTimeOffset.Now, new Author(), "", "", null));
                SemVersion currentVersion = new SemVersion(2, 1, 0);

                //throw new Exception("Test exception", new Exception("Inner test exception"));
                #endregion
                */
                
                SemVersion previousVersion = new SemVersion(0, 0, 0);
                releases.Reverse();

                List<GitHubRelease> tmpGitHubReleases = new List<GitHubRelease>();

                foreach (Release release in releases)
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
                        ReleaseType = getReleaseTypeFromVersions(releaseVersion, previousVersion)
                    });

                    previousVersion = releaseVersion;
                }

                tmpGitHubReleases.Reverse();
                foreach(GitHubRelease r in tmpGitHubReleases) { GitHubReleases.Add(r); }
            }
            catch (Exception ex)
            {
                ErrorOccuredWhileLoadingReleases = true;
                ErrorMessage = ex.Message + (ex.InnerException != null ? Environment.NewLine + ex.InnerException.Message : "");
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
