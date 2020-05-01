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

namespace AssemblyInfoHelper.GitHub
{
    class GitHubReleaseToUpdateButtonStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            StackPanel stackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
            PackIconBase icon = null;
            string text = "";

            GitHubRelease release = (GitHubRelease)value;
            
            switch (release.ReleaseTimeType)
            {
                case GitHubReleaseTimeTypes.OLD:
                    icon = new PackIconMaterial() { Kind = PackIconMaterialKind.History };
                    text = "Downgrade";
                    break;
                case GitHubReleaseTimeTypes.CURRENT:
                    icon = new PackIconOcticons() { Kind = PackIconOcticonsKind.Tools };
                    text = "Repair";
                    break;
                case GitHubReleaseTimeTypes.NEW:
                    icon = new PackIconOcticons() { Kind = PackIconOcticonsKind.DesktopDownload };
                    text = "Upgrade";
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
