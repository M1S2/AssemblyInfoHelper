using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Controls;
using MahApps.Metro.IconPacks;
using AssemblyInfoHelper.GitHub;
using System.Windows.Documents;
using System.Resources;

namespace AssemblyInfoHelper.GitHub
{
    class GitHubReleaseToUpdateButtonStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            StackPanel stackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
            PackIconBase icon = null;
            string text = "";

            ResourceManager resourceManager = new ResourceManager(typeof(Properties.Resources));

            GitHubRelease release = (GitHubRelease)value;
            
            switch (release.ReleaseTimeType)
            {
                case GitHubReleaseTimeTypes.OLD:
                    icon = new PackIconMaterial() { Kind = PackIconMaterialKind.History };
                    text = resourceManager.GetString("DowngradeString");
                    break;
                case GitHubReleaseTimeTypes.CURRENT:
                    icon = new PackIconOcticons() { Kind = PackIconOcticonsKind.Tools };
                    text = resourceManager.GetString("RepairString");
                    break;
                case GitHubReleaseTimeTypes.NEW:
                    icon = new PackIconOcticons() { Kind = PackIconOcticonsKind.DesktopDownload };
                    text = resourceManager.GetString("UpgradeString");
                    break;
            }

            icon.Width = 20;
            icon.Height = double.NaN;
            icon.Margin = new System.Windows.Thickness(5, 0, 10, 0);
            icon.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            stackPanel.Children.Add(icon);

            stackPanel.Children.Add(new TextBlock(new Run(text)) { FontSize = 12, VerticalAlignment = System.Windows.VerticalAlignment.Center });

            return stackPanel;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
