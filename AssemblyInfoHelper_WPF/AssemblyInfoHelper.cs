using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace AssemblyInfoHelper_WPF
{
    /// <summary>
    /// Get the values of the Assembly attributes
    /// </summary>
    public static class AssemblyInfoHelperClass
    {
        /// <summary>
        /// Assembly title attribute
        /// </summary>
        public static string AssemblyTitle
        {
            get
            {
                object[] assemblyObjects = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), true);

                //object[] assemblyObjects = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), true);

                if (assemblyObjects.Length > 0)
                {
                    return ((AssemblyTitleAttribute)assemblyObjects[0]).Title;
                }
                return "";
            }
        }

        /// <summary>
        /// Assembly description attribute
        /// </summary>
        public static string AssemblyDescription
        {
            get
            {
                object[] assemblyObjects = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), true);

                if (assemblyObjects.Length > 0)
                {
                    return ((AssemblyDescriptionAttribute)assemblyObjects[0]).Description;
                }
                return "";
            }
        }

        /// <summary>
        /// Assembly configuration attribute
        /// </summary>
        public static string AssemblyConfiguration
        {
            get
            {
                object[] assemblyObjects = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyConfigurationAttribute), true);

                if (assemblyObjects.Length > 0)
                {
                    return ((AssemblyConfigurationAttribute)assemblyObjects[0]).Configuration;
                }
                return "";
            }
        }

        /// <summary>
        /// Assembly company attribute
        /// </summary>
        public static string AssemblyCompany
        {
            get
            {
                object[] assemblyObjects = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);

                if (assemblyObjects.Length > 0)
                {
                    return ((AssemblyCompanyAttribute)assemblyObjects[0]).Company;
                }
                return "";
            }
        }

        /// <summary>
        /// Assembly product attribute
        /// </summary>
        public static string AssemblyProduct
        {
            get
            {
                object[] assemblyObjects = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), true);

                if (assemblyObjects.Length > 0)
                {
                    return ((AssemblyProductAttribute)assemblyObjects[0]).Product;
                }
                return "";
            }
        }

        /// <summary>
        /// Assembly copyright attribute
        /// </summary>
        public static string AssemblyCopyright
        {
            get
            {
                object[] assemblyObjects = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true);

                if (assemblyObjects.Length > 0)
                {
                    return ((AssemblyCopyrightAttribute)assemblyObjects[0]).Copyright;
                }
                return "";
            }
        }

        /// <summary>
        /// Assembly trademark attribute
        /// </summary>
        public static string AssemblyTrademark
        {
            get
            {
                object[] assemblyObjects = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyTrademarkAttribute), true);

                if (assemblyObjects.Length > 0)
                {
                    return ((AssemblyTrademarkAttribute)assemblyObjects[0]).Trademark;
                }
                return "";
            }
        }

        /// <summary>
        /// Assembly culture attribute
        /// </summary>
        public static string AssemblyCulture
        {
            get
            {
                object[] assemblyObjects = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyCultureAttribute), true);

                if (assemblyObjects.Length > 0)
                {
                    return ((AssemblyCultureAttribute)assemblyObjects[0]).Culture;
                }
                return "";
            }
        }

        /// <summary>
        /// Assembly version attribute
        /// </summary>
        public static string AssemblyVersion
        {
            get
            {
                return Assembly.GetEntryAssembly().GetName().Version.ToString();
            }
        }

        /// <summary>
        /// Assembly file version attribute
        /// </summary>
        public static string AssemblyFileVersion
        {
            get
            {
                object[] assemblyObjects = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true);

                if (assemblyObjects.Length > 0)
                {
                    return ((AssemblyFileVersionAttribute)assemblyObjects[0]).Version;
                }
                return "";
            }
        }

        /// <summary>
        /// Get the time of the last build of the assembly.
        /// </summary>
        /// <returns>last build time</returns>
        /// see: https://stackoverflow.com/questions/1600962/displaying-the-build-date?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa
        public static DateTime AssemblyLinkerTime
        {
            get
            {
                Assembly assembly = Assembly.GetEntryAssembly();

                var filePath = assembly.Location;
                const int c_PeHeaderOffset = 60;
                const int c_LinkerTimestampOffset = 8;

                var buffer = new byte[2048];

                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    stream.Read(buffer, 0, 2048);

                var offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
                var secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

                var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, TimeZoneInfo.Local);

                return localTime;
            }
        }

    }
}
