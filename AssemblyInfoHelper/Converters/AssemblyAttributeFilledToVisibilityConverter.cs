using System;
using System.Globalization;
using System.Resources;
using System.Windows;
using System.Windows.Data;

namespace AssemblyInfoHelper.Converters
{
    /// <summary>
    /// Return Visibility.Visible if the given string is not empty.
    /// Return Visibility.Collapsed if the given string is null or empty.
    /// </summary>
    public class AssemblyAttributeFilledToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
