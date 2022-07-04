using System;
using System.Globalization;
using System.Resources;
using System.Windows;
using System.Windows.Data;

namespace AssemblyInfoHelper.Converters
{
    /// <summary>
    /// Return Visibility.Visible if the fileVersion (given value[0]) doesn't equals the version (given value[1]).
    /// Return Visibility.Collapsed if the fileVersion (given value[0]) equals the version (given value[1]).
    /// </summary>
    public class AssemblyFileVersionVersionToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values == null || values.Length <= 1) { return Visibility.Collapsed; }

            string fileVersion = (string)values[0];
            string version = (string)values[1];
            return (fileVersion == version) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
