using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyInfoHelper.GitHub
{
    /// <summary>
    /// Time relation of the release to the current version
    /// </summary>
    public enum GitHubReleaseTimeTypes
    {
        /// <summary>
        /// Release version is newer than the current version
        /// </summary>
        NEW,

        /// <summary>
        /// Release version is equal to the current version
        /// </summary>
        CURRENT,

        /// <summary>
        /// Release version is older than the current version
        /// </summary>
        OLD
    }
}
