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
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AssemblyInfoHelper.Updater
{
    /// <summary>
    /// Interaktionslogik für UpdaterWindow.xaml
    /// </summary>
    /// see: https://github.com/Tyrrrz/Onova/blob/master/Onova.Updater/Updater.cs
    public partial class UpdaterWindow : Window, INotifyPropertyChanged
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

        //####################################################################################################################################################################

        private double _updateProgress;
        /// <summary>
        /// Update Progress in percent (0 ... 100)
        /// </summary>
        public double UpdateProgress
        {
            get { return _updateProgress; }
            set { _updateProgress = value; OnPropertyChanged(); }
        }

        private bool _updateProgressIndeterminate;
        /// <summary>
        /// Should the progressbar be indeterminate (true) or show the UpdateProgress (false)
        /// </summary>
        public bool UpdateProgressIndeterminate
        {
            get { return _updateProgressIndeterminate; }
            set { _updateProgressIndeterminate = value; OnPropertyChanged(); }
        }

        private string _updateStatus;
        /// <summary>
        /// Text representing the update status
        /// </summary>
        public string UpdateStatus
        {
            get { return _updateStatus; }
            set { _updateStatus = value; OnPropertyChanged(); }
        }

        //********************************************************************************************************************************************************************

        private readonly string _updateeFilePath;
        private readonly string _packageContentDirPath;
        private readonly bool _restartUpdatee;
        private readonly string _persistentFilesList;             // list with files relative to the _updateeFilePath that are persistent (kept) during updating. Files are separated by ; e.g. "File1.txt;File2.txt"
        private readonly string _routedArgs;

        public UpdaterWindow(string updateeFilePath, string packageContentDirPath, bool restartUpdatee, string persistentFilesList, string routedArgs)
        {
            this.DataContext = this;
            InitializeComponent();
            _updateeFilePath = updateeFilePath;
            _packageContentDirPath = packageContentDirPath;
            _restartUpdatee = restartUpdatee;
            _persistentFilesList = persistentFilesList;
            _routedArgs = routedArgs;
        }

        public UpdaterWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public async void RunUpdate()
        {
            UpdateProgressIndeterminate = true;
            IProgress<double> _updateProgress = new Progress<double>(progress => { UpdateProgress = Math.Round(progress * 100, 0); });

            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // Wait until updatee is writable to ensure all running instances have exited
            UpdateStatus = Properties.Resources.UpdateStatusWaitForExit;
            while (!FileDirectoryExtensions.CheckFileWriteAccess(_updateeFilePath))
                await Task.Delay(100);

            UpdateProgressIndeterminate = false;
            string updateeDirPath = Path.GetDirectoryName(_updateeFilePath);

            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------

            List<string> persistentFilesFullPathResolved = new List<string>();
            string persistentFilesFolderName = Path.Combine(_packageContentDirPath, @"..\PersistentFiles");
            // Create folder to store files to keep if available
            if (!string.IsNullOrEmpty(_persistentFilesList))
            {
                UpdateStatus = Properties.Resources.UpdateStatusCopyPersistentFiles;
                if (Directory.Exists(persistentFilesFolderName)) { Directory.Delete(persistentFilesFolderName, true); }
                Directory.CreateDirectory(persistentFilesFolderName);

                foreach (string persistentFile in _persistentFilesList.Split(';').ToList())
                {
                    List<string> files = Directory.GetFiles(updateeDirPath, persistentFile).Select(f => f.Replace(updateeDirPath, "").TrimStart('\\', '/')).ToList();
                    persistentFilesFullPathResolved.AddRange(files);
                }
            }
            // Loop all files marked as persitent during the update and copy them
            foreach (string persistentFile in persistentFilesFullPathResolved)
            {
                string persistentFileSourcePath = Path.Combine(updateeDirPath, persistentFile);
                string persistentFileDestPath = Path.Combine(persistentFilesFolderName, persistentFile);
                if (File.Exists(persistentFileSourcePath)) 
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(persistentFileDestPath));
                    File.Copy(persistentFileSourcePath, persistentFileDestPath, true); 
                }
            }

            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------

            UpdateStatus = Properties.Resources.UpdateStatusDeleteAppDirectory;
            await FileDirectoryExtensions.DeleteDirectory(updateeDirPath, true, true, _updateProgress);

            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // Copy over the package contents
            UpdateStatus = Properties.Resources.UpdateStatusCopyContents;
            await FileDirectoryExtensions.CopyDirectory(_packageContentDirPath, updateeDirPath, _updateProgress);

            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------

            if (!string.IsNullOrEmpty(_persistentFilesList)) { UpdateStatus = Properties.Resources.UpdateStatusRestorePersistentFiles; }
            // Loop all files marked as persistent during the update and restore them
            foreach (string persistentFile in persistentFilesFullPathResolved)
            {
                string persistentFileSourcePath = Path.Combine(persistentFilesFolderName, persistentFile);
                string persistentFileDestPath = Path.Combine(updateeDirPath, persistentFile);
                if (File.Exists(persistentFileSourcePath)) 
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(persistentFileDestPath));
                    File.Copy(persistentFileSourcePath, persistentFileDestPath, true); 
                }
            }

            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // Restart updatee if requested
            if (_restartUpdatee)
            {
                var startInfo = new ProcessStartInfo
                {
                    WorkingDirectory = updateeDirPath,
                    Arguments = _routedArgs,
                    UseShellExecute = true // avoid sharing console window with updatee
                };
                
                // If updatee is an .exe file - start it directly
                if (string.Equals(Path.GetExtension(_updateeFilePath), ".exe", StringComparison.OrdinalIgnoreCase))
                {
                    startInfo.FileName = _updateeFilePath;
                }

                UpdateStatus = Properties.Resources.UpdateStatusRestartApplication + $" [{startInfo.FileName} {startInfo.Arguments}]...";
                
                if (File.Exists(startInfo.FileName))
                {
                    Process.Start(startInfo);
                }
            }
            
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // Delete package content directory
            UpdateStatus = Properties.Resources.UpdateStatusDeletePackageDirectory;
            await FileDirectoryExtensions.DeleteDirectory(_packageContentDirPath, false, true, _updateProgress);
            await FileDirectoryExtensions.DeleteEmptyFolders(Directory.GetParent(_packageContentDirPath).FullName);

            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // Delete persistent files directory
            UpdateStatus = Properties.Resources.UpdateStatusDeletePersistentFiles;
            await FileDirectoryExtensions.DeleteDirectory(persistentFilesFolderName, false, true, _updateProgress);
            await FileDirectoryExtensions.DeleteEmptyFolders(Directory.GetParent(persistentFilesFolderName).FullName);
            Environment.Exit(0);
        }
    }
}
