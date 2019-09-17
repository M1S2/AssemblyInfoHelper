using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AssemblyInfoHelper.GitHub
{
    public class ReleaseTypeToBadgeTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((GitHubReleaseTimeTypes)value)
            {
                case GitHubReleaseTimeTypes.NEW:
                    return "New";
                case GitHubReleaseTimeTypes.CURRENT:
                    return "Current";
                default:
                    return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
