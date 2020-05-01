using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Semver;

namespace AssemblyInfoHelper.GitHub
{
    public class UpdateStatusInfo : INotifyPropertyChanged
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

        private bool _isUpdateRunning;
        /// <summary>
        /// True if the update process is running
        /// </summary>
        public bool IsUpdateRunning
        {
            get { return _isUpdateRunning; }
            set { _isUpdateRunning = value; OnPropertyChanged(); }
        }

        private int _updateProgress;
        /// <summary>
        /// Update progress in percent (0 ... 100)
        /// </summary>
        public int UpdateProgress
        {
            get { return _updateProgress; }
            set { _updateProgress = value; OnPropertyChanged(); }
        }

        private SemVersion _fromVersion;
        /// <summary>
        /// This is the old version before the update
        /// </summary>
        public SemVersion FromVersion
        {
            get { return _fromVersion; }
            set { _fromVersion = value; OnPropertyChanged(); OnPropertyChanged("UpdateText"); }
        }

        private SemVersion _toVersion;
        /// <summary>
        /// This is the new version after the update
        /// </summary>
        public SemVersion ToVersion
        {
            get { return _toVersion; }
            set { _toVersion = value; OnPropertyChanged(); OnPropertyChanged("UpdateText"); }
        }
        
        /// <summary>
        /// Text that is shown while updating
        /// </summary>
        public string UpdateText
        {
            get
            {
                if (FromVersion < ToVersion) { return "Update from v" + FromVersion?.ToString() + " to v" + ToVersion?.ToString(); }
                else if(FromVersion > ToVersion) { return "Downgrade from v" + FromVersion?.ToString() + " to v" + ToVersion?.ToString(); }
                else { return "Repair v" + FromVersion?.ToString(); }
            }
        }
    }
}
