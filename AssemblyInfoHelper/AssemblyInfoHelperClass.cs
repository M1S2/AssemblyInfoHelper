using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using AssemblyInfoHelper.GitHubReleases;

namespace AssemblyInfoHelper
{
    /// <summary>
    /// Get the values of the Assembly attributes
    /// </summary>
    public static class AssemblyInfoHelperClass
    {
        /// <summary>
        /// Types of assembly attributes
        /// </summary>
        public enum AssemblyAttributeTypes
        {
            TITLE,
            DESCRIPTION,
            CONFIGURATION,
            COMPANY,
            PRODUCT,
            COPYRIGHT,
            TRADEMARK,
            CULTURE,
            FILEVERSION,
            VERSION,
            GITHUB_URL
        }

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Get the specified attribute value from the given assembly
        /// </summary>
        /// <param name="assembly">Assembly from which to get the attribute</param>
        /// <param name="attributeType">Type of the attribute to get</param>
        /// <returns>String with attribute value</returns>
        public static string GetAttributeFromAssembly(Assembly assembly, AssemblyAttributeTypes attributeType)
        {
            object[] assemblyObjects;
            string attributeValue = "";

            switch (attributeType)
            {
                case AssemblyAttributeTypes.TITLE:
                {
                    assemblyObjects = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), true);
                    if (assemblyObjects.Length > 0) { attributeValue = ((AssemblyTitleAttribute)assemblyObjects[0]).Title; }
                    break;
                }
                case AssemblyAttributeTypes.DESCRIPTION:
                {
                    assemblyObjects = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), true);
                    if (assemblyObjects.Length > 0) { attributeValue = ((AssemblyDescriptionAttribute)assemblyObjects[0]).Description; }
                    break;
                }
                case AssemblyAttributeTypes.CONFIGURATION:
                {
                    assemblyObjects = assembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), true);
                    if (assemblyObjects.Length > 0) { attributeValue = ((AssemblyConfigurationAttribute)assemblyObjects[0]).Configuration; }
                    break;
                }
                case AssemblyAttributeTypes.COMPANY:
                {
                    assemblyObjects = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
                    if (assemblyObjects.Length > 0) { attributeValue = ((AssemblyCompanyAttribute)assemblyObjects[0]).Company; }
                    break;
                }
                case AssemblyAttributeTypes.PRODUCT:
                {
                    assemblyObjects = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), true);
                    if (assemblyObjects.Length > 0) { attributeValue = ((AssemblyProductAttribute)assemblyObjects[0]).Product; }
                    break;
                }
                case AssemblyAttributeTypes.COPYRIGHT:
                {
                    assemblyObjects = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true);
                    if (assemblyObjects.Length > 0) { attributeValue = ((AssemblyCopyrightAttribute)assemblyObjects[0]).Copyright; }
                    break;
                }
                case AssemblyAttributeTypes.TRADEMARK:
                {
                    assemblyObjects = assembly.GetCustomAttributes(typeof(AssemblyTrademarkAttribute), true);
                    if (assemblyObjects.Length > 0) { attributeValue = ((AssemblyTrademarkAttribute)assemblyObjects[0]).Trademark; }
                    break;
                }
                case AssemblyAttributeTypes.CULTURE:
                {
                    assemblyObjects = assembly.GetCustomAttributes(typeof(AssemblyCultureAttribute), true);
                    if (assemblyObjects.Length > 0) { attributeValue = ((AssemblyCultureAttribute)assemblyObjects[0]).Culture; }
                    break;
                }
                case AssemblyAttributeTypes.FILEVERSION:
                {
                    assemblyObjects = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true);
                    if (assemblyObjects.Length > 0)
                    {
                        attributeValue = ((AssemblyFileVersionAttribute)assemblyObjects[0]).Version;
                        attributeValue = attributeValue.Substring(0, attributeValue.LastIndexOf("."));
                    }
                    break;
                }
                case AssemblyAttributeTypes.VERSION:
                {
                    attributeValue = Assembly.GetEntryAssembly().GetName().Version.ToString();
                    attributeValue = attributeValue.Substring(0, attributeValue.LastIndexOf("."));
                    break;
                }
                case AssemblyAttributeTypes.GITHUB_URL:
                {
                    assemblyObjects = assembly.GetCustomAttributes(typeof(GitHubRepoAttribute), true);
                    if (assemblyObjects.Length > 0) { attributeValue = ((GitHubRepoAttribute)assemblyObjects[0]).RepoUrl; }
                    break;
                }
            }

            return attributeValue;
        }

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Get the time of the last build of the assembly.
        /// </summary>
        /// <param name="assembly">Assembly from which to get the last linker time</param>
        /// <returns>last build time</returns>
        /// see: https://stackoverflow.com/questions/1600962/displaying-the-build-date?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa
        public static DateTime GetLinkerTimeFromAssembly(Assembly assembly)
        {
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

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Assembly title attribute
        /// </summary>
        public static string AssemblyTitle => GetAttributeFromAssembly(Assembly.GetEntryAssembly(), AssemblyAttributeTypes.TITLE);

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Assembly description attribute
        /// </summary>
        public static string AssemblyDescription => GetAttributeFromAssembly(Assembly.GetEntryAssembly(), AssemblyAttributeTypes.DESCRIPTION);

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Assembly configuration attribute
        /// </summary>
        public static string AssemblyConfiguration => GetAttributeFromAssembly(Assembly.GetEntryAssembly(), AssemblyAttributeTypes.CONFIGURATION);

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Assembly company attribute
        /// </summary>
        public static string AssemblyCompany => GetAttributeFromAssembly(Assembly.GetEntryAssembly(), AssemblyAttributeTypes.COMPANY);

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Assembly product attribute
        /// </summary>
        public static string AssemblyProduct => GetAttributeFromAssembly(Assembly.GetEntryAssembly(), AssemblyAttributeTypes.PRODUCT);

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Assembly copyright attribute
        /// </summary>
        public static string AssemblyCopyright => GetAttributeFromAssembly(Assembly.GetEntryAssembly(), AssemblyAttributeTypes.COPYRIGHT);

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Assembly trademark attribute
        /// </summary>
        public static string AssemblyTrademark => GetAttributeFromAssembly(Assembly.GetEntryAssembly(), AssemblyAttributeTypes.TRADEMARK);

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Assembly culture attribute
        /// </summary>
        public static string AssemblyCulture => GetAttributeFromAssembly(Assembly.GetEntryAssembly(), AssemblyAttributeTypes.CULTURE);

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Assembly version attribute
        /// </summary>
        public static string AssemblyVersion => GetAttributeFromAssembly(Assembly.GetEntryAssembly(), AssemblyAttributeTypes.VERSION);

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Assembly file version attribute
        /// </summary>
        public static string AssemblyFileVersion => GetAttributeFromAssembly(Assembly.GetEntryAssembly(), AssemblyAttributeTypes.FILEVERSION);

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Get the time of the last build of the assembly.
        /// </summary>
        /// <returns>last build time</returns>
        public static DateTime AssemblyLinkerTime => GetLinkerTimeFromAssembly(Assembly.GetEntryAssembly());

        //********************************************************************************************************************************************************************

        /// <summary>
        /// GitHubRepo attribute
        /// </summary>
        public static string GitHubRepoUrl => GetAttributeFromAssembly(Assembly.GetEntryAssembly(), AssemblyAttributeTypes.GITHUB_URL);

    }
}
