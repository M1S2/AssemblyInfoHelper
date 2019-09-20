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

namespace AssemblyInfoHelper
{
    /// <summary>
    /// Interaktionslogik für AppInfoButton.xaml
    /// </summary>
    public partial class AppInfoButton : UserControl
    {
        public bool EnableNewVersionNotification { get; set; } = true;

        //********************************************************************************************************************************************************************

        private ICommand _infoCommand;
        public ICommand InfoCommand
        {
            get
            {
                if (_infoCommand == null)
                {
                    _infoCommand = new RelayCommand(param =>
                    {
                        WindowAssemblyInfo windowAssemblyInfo = new WindowAssemblyInfo(GitHub.GitHubUtils.Instance.AreNewReleasesAvailable ? WindowAssemblyInfoStartTab.GITHUB : WindowAssemblyInfoStartTab.GENERAL_INFOS);
                        windowAssemblyInfo.ShowDialog();
                    });
                }
                return _infoCommand;
            }
        }

        //********************************************************************************************************************************************************************

        public AppInfoButton()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Loaded += AppInfoButton_Loaded;
        }

        //see: https://stackoverflow.com/questions/4708039/what-event-is-fired-when-a-usercontrol-is-displayed
        private void AppInfoButton_Loaded(object sender, RoutedEventArgs e)
        {
            PresentationSource presentationSource = PresentationSource.FromVisual((Visual)sender);      // Get PresentationSource
            presentationSource.ContentRendered += PresentationSource_ContentRendered;                   // Subscribe to PresentationSource's ContentRendered event
        }

        /// <summary>
        /// This function is called when the user control was rendered (was already visible)
        /// </summary>
        private void PresentationSource_ContentRendered(object sender, EventArgs e)
        {
            ((PresentationSource)sender).ContentRendered -= PresentationSource_ContentRendered;         // Don't forget to unsubscribe from the event

            if (EnableNewVersionNotification) { GitHub.GitHubUtils.CheckAndDisplayNewReleases(); }
        }
    }
}
