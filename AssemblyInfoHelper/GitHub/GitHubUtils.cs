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
using Onova;
using Onova.Services;
using Onova.Models;
using System.IO;
using System.Diagnostics;

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

        private UpdateStatusInfo _updateStatus = new UpdateStatusInfo();
        /// <summary>
        /// Status of the GitHub update feature (is update running, update progress)
        /// </summary>
        public UpdateStatusInfo UpdateStatus
        {
            get { return _updateStatus; }
            private set { _updateStatus = value; OnPropertyChanged(); }
        }

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
                    _updateCommand = new RelayCommand(async param => await RunUpdate((GitHubRelease)param), ret => !UpdateStatus.IsUpdateRunning);
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
        /// Update the application to the given target release. This can also be a downgrade (lower version) or repair (same version).
        /// </summary>
        /// <param name="targetRelease">New version after update</param>
        private async Task RunUpdate(GitHubRelease targetRelease)
        {
            if (!IsGitHubRepoAssigned) { return; }

            WindowAssemblyInfo windowAssemblyInfo = System.Windows.Application.Current.Windows.OfType<MahApps.Metro.Controls.MetroWindow>().OfType<WindowAssemblyInfo>().FirstOrDefault();
            Version targetVersion = new Version(targetRelease.Version.ToString());
            try
            {
                UpdateStatus.FromVersion = new SemVersion(new Version(AssemblyInfoHelperClass.AssemblyVersion));
                UpdateStatus.ToVersion = targetRelease.Version;
                UpdateStatus.IsUpdateRunning = true;

                MessageDialogResult messageResult = await windowAssemblyInfo.ShowMessageAsync("Confirm update", "Do you really want to " + UpdateStatus.UpdateText, MessageDialogStyle.AffirmativeAndNegative);
                if(messageResult == MessageDialogResult.Negative) { UpdateStatus.IsUpdateRunning = false; return; }

#warning Use GitHubPackageResolver
                //UpdateManager manager = new UpdateManager(new LocalPackageResolver(@"C:\Users\masc107\Desktop\Test\AssemblyInfoTestReleases", "*.zip"), new ZipPackageExtractor());

                // example url: https://github.com/M1S2/AssemblyInfoHelper
                string[] urlSplitted = AssemblyInfoHelperClass.GitHubRepoUrl.Split('/');
                if (urlSplitted.Length < 5) { return; }
                string repoOwner = urlSplitted[3];
                string repoName = urlSplitted[4];
#warning System.Buffer 4.0.2 not found ???!!!
                UpdateManager manager = new UpdateManager(new GithubPackageResolver(repoOwner, repoName, "*.zip"), new ZipPackageExtractor());

                GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue("AssemblyInfoHelper-UpdateCheck"));
#warning Maybe step back to .NET v4.5.2??!!
                List<Release> releases = new List<Release>(await gitHubClient.Repository.Release.GetAll(repoOwner, repoName));
                Release release = releases.Where(r => r.Name.Contains(targetVersion.ToString())).FirstOrDefault();
                //release.Assets.Where(a => a.Name)

                IProgress<double> updateProgress = new Progress<double>(progress => 
                {
                    UpdateStatus.UpdateProgress = (int)(progress * 100);
                });

                // Get the path to the downloaded package
                string onovaApplicationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Onova", AssemblyInfoHelperClass.AssemblyTitle);
                string packagePath = Path.Combine(onovaApplicationPath, targetVersion.ToString());
                string binFolderPath = Path.Combine(packagePath, "bin");
#warning older versions of SpotifyRecorder are using bin_Setup_Spotify_Recorder as name ...
                string binSetupFolderPath = Path.Combine(packagePath, "bin_Setup");
                bool useBinFolder = false, useBinSetupFolder = false;

                /*await Task.Run(() =>
                {
                    if (File.Exists(Path.Combine(onovaApplicationPath, targetVersion.ToString() + ".onv"))) { File.Delete(Path.Combine(onovaApplicationPath, targetVersion.ToString() + ".onv")); }
                    if (File.Exists(Path.Combine(onovaApplicationPath, "Onova.lock"))) { File.Delete(Path.Combine(onovaApplicationPath, "Onova.lock")); }
                    if (Directory.Exists(packagePath)) { Directory.Delete(packagePath, true); }
                });*/

                await manager.PrepareUpdateAsync(targetVersion, updateProgress);    // Prepare an update by downloading and extracting the package

                if(Directory.Exists(binFolderPath) && !Directory.Exists(binSetupFolderPath))            // If only bin folder exists, use this as update source
                {
                    useBinFolder = true;
                }
                else if (!Directory.Exists(binFolderPath) && Directory.Exists(binSetupFolderPath))      // If only bin_Setup folder exists, use the installer for update
                {
                    useBinSetupFolder = true;
                }
                else if (Directory.Exists(binFolderPath) && Directory.Exists(binSetupFolderPath))       // If both bin and bin_Setup folder exist, let the user choose the update source
                {
                    messageResult = await windowAssemblyInfo.ShowMessageAsync("Choose update source", "There are multiple options to update this version. Choose one of the options below.", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() { AffirmativeButtonText = "Use binary folder", NegativeButtonText = "Use Installer", DefaultButtonFocus = MessageDialogResult.Affirmative });
                    if (messageResult == MessageDialogResult.Affirmative) { useBinFolder = true; }
                    else { useBinSetupFolder = true; }
                }
                
                double deleteProgressMaxValueBinFolder = 0.25;          // Maximum value for delete progress when useBinFolder is true (because the remaining progress is then filled up while copying files)

                await Task.Run(() =>
                {
                    // Cleanup as much files from the application directory as possible. Some files are used by the application and can't be deleted.
                    List<string> filePaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.*", SearchOption.AllDirectories).Where(p => Path.GetExtension(p) != ".exe.config").ToList();
                    int filesDeleted = 0;
                    foreach (string filePath in filePaths)
                    {
                        try { File.Delete(filePath); }
                        catch (Exception) { /* Nothing to do here. Files that can't be deleted, are currently used by the application. They are overwritten by the updater later. */ }
                        filesDeleted++;
                        updateProgress.Report(((double)filesDeleted / filePaths.Count) * (useBinFolder ? deleteProgressMaxValueBinFolder : 1));
                    }
                    DeleteEmptyFolders(AppDomain.CurrentDomain.BaseDirectory);
                });

                if (!useBinFolder && !useBinSetupFolder)
                {
                    manager.LaunchUpdater(targetVersion);           // Launch an executable that will apply the update and restart the application afterwards
                }
                else if(useBinSetupFolder)
                {
                    Process.Start(Path.Combine(packagePath, "bin_Setup", "setup.exe"));
                }
                else if(useBinFolder)
                {
                    await Task.Run(() =>
                    {
                        string[] files = Directory.GetFiles(binFolderPath, "*.*", SearchOption.AllDirectories);
                        // Copy all files from bin folder one folder higher (outside bin folder), see: https://stackoverflow.com/questions/58744/copy-the-entire-contents-of-a-directory-in-c-sharp
                        foreach (string dirPath in Directory.GetDirectories(binFolderPath, "*", SearchOption.AllDirectories)) { Directory.CreateDirectory(dirPath.Replace(binFolderPath, packagePath)); }
                        int filesCopied = 0;
                        foreach (string newPath in files)
                        {
                            File.Move(newPath, newPath.Replace(binFolderPath, packagePath));
                            filesCopied++;
                            updateProgress.Report(deleteProgressMaxValueBinFolder + ((double)filesCopied / files.Length) * (1 - deleteProgressMaxValueBinFolder));
                        }

                        Directory.Delete(binFolderPath, true);
                        if (Directory.Exists(binSetupFolderPath)) { Directory.Delete(binSetupFolderPath, true); }
                    });

                    manager.LaunchUpdater(targetVersion);           // Launch an executable that will apply the update and restart the application afterwards
                }

                if (useBinSetupFolder) { await windowAssemblyInfo.ShowMessageAsync("Update", "To finish the update, the application is closed now. Please use the started installer to reinstall the application.", MessageDialogStyle.Affirmative); }
                else { await windowAssemblyInfo.ShowMessageAsync("Update", "To finish the update, the application is closed now. This may take some time. After the update is finished, the application is restarted.", MessageDialogStyle.Affirmative); }

                manager.Dispose();
                Environment.Exit(0);                            // Terminate the running application so that the updater/installer can overwrite files
            }
            catch (Exception ex)
            {
                await windowAssemblyInfo?.ShowMessageAsync("Error while update", ex.Message);
                UpdateStatus.IsUpdateRunning = false;
            }
        }

        //********************************************************************************************************************************************************************

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
                
#warning Remove TestCode
                /*List<Release> releases = new List<Release>();
                releases.Add(new Release("abc", "https://www.google.de", "ghi", "jkl", 0, "0", "v1.0.0", "", "Release v1.0.0", "Hallo1", false, false, DateTime.Now, DateTime.Now, new Author(), "", "", null));
                releases.Add(new Release("abc", "https://www.google.de", "ghi", "jkl", 1, "1", "v2.0.0", "", "Release v2.0.0", "Hallo2", false, false, DateTime.Now, DateTime.Now, new Author(), "", "", null));
                releases.Add(new Release("abc", "https://www.google.de", "ghi", "jkl", 2, "2", "v3.0.0", "", "Release v3.0.0", "Hallo3", false, false, DateTime.Now, DateTime.Now, new Author(), "", "", null));
                releases.Add(new Release("abc", "https://www.google.de", "ghi", "jkl", 3, "3", "v4.0.0", "", "Release v4.0.0", "Hallo4", false, false, DateTime.Now, DateTime.Now, new Author(), "", "", null));
                releases.Add(new Release("abc", "https://www.google.de", "ghi", "jkl", 4, "4", "v4.1.0", "", "Release v4.1.0", "Hallo5", false, false, DateTime.Now, DateTime.Now, new Author(), "", "", null));
                releases.Add(new Release("abc", "https://www.google.de", "ghi", "jkl", 5, "5", "v4.1.1", "", "Release v4.1.1", "Hallo6", false, false, DateTime.Now, DateTime.Now, new Author(), "", "", null));
                releases.Add(new Release("abc", "https://www.google.de", "ghi", "jkl", 6, "6", "v4.2.0", "", "Release v4.2.0", "Hallo7", false, false, DateTime.Now, DateTime.Now, new Author(), "", "", null));
                releases.Add(new Release("abc", "https://www.google.de", "ghi", "jkl", 7, "7", "v4.2.1", "", "Release v4.2.1", "Hallo8", false, false, DateTime.Now, DateTime.Now, new Author(), "", "", null));
                */

                SemVersion currentVersion = stripInitialV(AssemblyInfoHelperClass.AssemblyVersion);
                
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

        //********************************************************************************************************************************************************************

        /// <summary>
        /// This function deletes all empty folders in the given path.
        /// </summary>
        /// <param name="startFolder">The folder to start from.</param>
        /// see: https://stackoverflow.com/questions/2811509/c-sharp-remove-all-empty-subdirectories
        private void DeleteEmptyFolders(string startFolder)
        {
            if (!Directory.Exists(startFolder)) { return; }

            foreach (string directory in Directory.GetDirectories(startFolder))
            {
                DeleteEmptyFolders(directory);
                if (Directory.GetFiles(directory).Length == 0 && Directory.GetDirectories(directory).Length == 0)
                {
                    Directory.Delete(directory, false);
                }
            }
        }

    }
}
