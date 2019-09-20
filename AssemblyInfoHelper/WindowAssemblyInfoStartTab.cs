using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyInfoHelper
{
    /// <summary>
    /// Tab that is shown on startup of the WindowAssemblyInfo
    /// </summary>
    public enum WindowAssemblyInfoStartTab
    {
        /// <summary>
        /// Show the general infos tab
        /// </summary>
        GENERAL_INFOS = 0,

        /// <summary>
        /// Show the readme tab
        /// </summary>
        README = 1,

        /// <summary>
        /// Show the changelog tab
        /// </summary>
        CHANGELOG = 2,

        /// <summary>
        /// Show the github tab
        /// </summary>
        GITHUB = 3
    }
}
