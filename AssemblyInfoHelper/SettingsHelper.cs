using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Reflection;

namespace AssemblyInfoHelper
{
    /// <summary>
    /// Class that is used to save settings of the library to an .dll.config file
    /// </summary>
    ///see: https://stackoverflow.com/questions/5190539/equivalent-to-app-config-for-a-library-dll
    public class SettingsHelper
    {
        private Configuration _configuration;
        /// <summary>
        /// Configuration object of the actual assembly
        /// </summary>
        public Configuration Config
        {
            get
            {
                if (_configuration == null)
                {
                    if (System.IO.File.Exists(_assemblyLocation)) { _configuration = ConfigurationManager.OpenExeConfiguration(_assemblyLocation); }
                    else { _configuration = null; }
                }
                return _configuration;
            }
        }

        private string _assemblyLocation;
        
        //####################################################################################################################################################################

        /// <summary>
        /// Loads config file settings for libraries that use assembly.dll.config files
        /// </summary>
        /// <param name="assemblyLocation">The full path or UNC location of the loaded file that contains the manifest.</param>
        public SettingsHelper(string assemblyLocation)
        {
            _assemblyLocation = assemblyLocation;
        }

        /// <summary>
        /// Loads config file settings for libraries that use assembly.dll.config files
        /// </summary>
        /// <param name="assembly">The loaded file that contains the manifest.</param>
        public SettingsHelper(Assembly assembly)
        {
            _assemblyLocation = assembly.Location;
        }

        //####################################################################################################################################################################

        /// <summary>
        /// Get an setting from the .dll.config file. The setting is identified by the given key. If the key isn't found, the keyNotFoundReturnValue is returned (or default if variable is not assigned)
        /// </summary>
        /// <param name="key">Key to identify the setting</param>
        /// <param name="keyNotFoundReturnValue">Value that is returned when the setting isn't found</param>
        /// <returns>Setting value or default value if key isn't found</returns>
        public string GetAppSetting(string key, string keyNotFoundReturnValue = default(string))
        {
            return GetAppSetting<string>(key);
        }

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Get an setting from the .dll.config file. The setting is identified by the given key. If the key isn't found, the keyNotFoundReturnValue is returned (or default if variable is not assigned)
        /// </summary>
        /// <typeparam name="T">Type of the setting that is returned</typeparam>
        /// <param name="key">Key to identify the setting</param>
        /// <param name="keyNotFoundReturnValue">Value that is returned when the setting isn't found</param>
        /// <returns>Setting value or default value if key isn't found</returns>
        public T GetAppSetting<T>(string key, T keyNotFoundReturnValue = default(T))
        {
            string result = string.Empty;
            if (Config != null)
            {
                KeyValueConfigurationElement keyValueConfigurationElement = Config.AppSettings.Settings[key];
                if (keyValueConfigurationElement != null)
                {
                    string value = keyValueConfigurationElement.Value;
                    if (!string.IsNullOrEmpty(value)) result = value;
                }
            }
            T returnVal;
            try
            {
                returnVal = (T)Convert.ChangeType(result, typeof(T));
            }
            catch (Exception) { returnVal = keyNotFoundReturnValue; }
            return returnVal;
        }

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Set an setting from the .dll.config file. The setting is identified by the given key. If the setting isn't found, it is created.
        /// </summary>
        /// <param name="key">Key to identify the setting</param>
        /// <param name="val">Value of the setting to set</param>
        public void SetOrCreateAppSetting(string key, object val)
        {
            if (Config != null)
            {
                if(Config.AppSettings.Settings[key] == null)
                {
                    Config.AppSettings.Settings.Add(key, val.ToString());
                }
                else
                {
                    Config.AppSettings.Settings[key].Value = val.ToString();
                }
                Config.Save();
            }
        }
    }
}
