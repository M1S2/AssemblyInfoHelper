using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Semver;

namespace AssemblyInfoHelper.GitHubReleases
{
    public class GitHubRelease
    {
        public string Name { get; set; }
        public DateTimeOffset ReleaseTime { get; set; }
        public SemVersion Version { get; set; }
        public GitHubReleaseTypes ReleaseType { get; set; }
        public string ReleaseURL { get; set; }
    }
}
