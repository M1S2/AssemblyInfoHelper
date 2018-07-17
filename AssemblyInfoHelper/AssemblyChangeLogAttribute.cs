using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyInfoHelper
{
    /// <summary>
    /// Attribute to create change log entries
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class AssemblyChangeLogAttribute : Attribute
    {
        /// <summary>
        /// Major version number of the currently described version
        /// </summary>
        public int ChangeLogVersionMajor { get; set; }

        /// <summary>
        /// Minor version number of the currently described version
        /// </summary>
        public int ChangeLogVersionMinor { get; set; }

        /// <summary>
        /// Change log text with all changes compared to the previous version
        /// </summary>
        public string ChangeLogText { get; set; }

        /// <summary>
        /// Constructor of the AssemblyChangeLogAttribute
        /// </summary>
        /// <param name="changeLogVersionMajor">Major version number of the currently described version</param>
        /// <param name="changeLogVersionMinor">Minor version number of the currently described version</param>
        /// <param name="changeLogText">Change log text with all changes compared to the previous version</param>
        public AssemblyChangeLogAttribute(int changeLogVersionMajor, int changeLogVersionMinor, string changeLogText)
        {
            ChangeLogText = changeLogText;
            ChangeLogVersionMajor = changeLogVersionMajor;
            ChangeLogVersionMinor = changeLogVersionMinor;
        }
    }
}
