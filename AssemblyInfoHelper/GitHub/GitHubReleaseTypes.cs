using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyInfoHelper.GitHub
{
    /// <summary>
    /// Type of the release (major, minor, patch)
    /// </summary>
    public enum GitHubReleaseTypes
    {
        /// <summary>
        /// Major version release
        /// </summary>
        MAJOR,

        /// <summary>
        /// Minor version release
        /// </summary>
        MINOR,

        /// <summary>
        /// Patch version release
        /// </summary>
        PATCH
    }
}
