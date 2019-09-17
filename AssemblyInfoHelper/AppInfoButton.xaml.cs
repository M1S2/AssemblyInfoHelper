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
        private ICommand _infoCommand;
        public ICommand InfoCommand
        {
            get
            {
                if (_infoCommand == null)
                {
                    _infoCommand = new RelayCommand(param =>
                    {
                        WindowAssemblyInfo windowAssemblyInfo = new WindowAssemblyInfo();
                        windowAssemblyInfo.ShowDialog();
                    });
                }
                return _infoCommand;
            }
        }

        //********************************************************************************************************************************************************************

        public AppInfoButton()
        {
#warning Collapse width of batch (margin) if no new versions are available
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
