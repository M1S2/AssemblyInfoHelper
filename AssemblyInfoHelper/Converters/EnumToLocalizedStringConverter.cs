using System;
using System.Globalization;
using System.Resources;
using System.Windows.Data;

namespace AssemblyInfoHelper.Converters
{
    /// <summary>
    /// Get a localized string based on the enum value to convert.
    /// The entries in the separate Enums .resx file must have the format "{EnumType}_{EnumValue}"
    /// </summary>
    public class EnumToLocalizedStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum enumValue = (Enum)value;

            // https://stackoverflow.com/questions/17380900/enum-localization
            ResourceManager rm = new ResourceManager(typeof(Properties.Enums));
            string resourceDisplayName = rm.GetString(enumValue.GetType().Name + "_" + enumValue);
            return string.IsNullOrWhiteSpace(resourceDisplayName) ? string.Format("{0}", enumValue) : resourceDisplayName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
