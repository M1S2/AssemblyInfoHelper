using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using Semver;

namespace AssemblyInfoHelper.GitHub
{
    public class GitHubRelease
    {
        public string Name { get; set; }
        public DateTimeOffset ReleaseTime { get; set; }
        public SemVersion Version { get; set; }
        public GitHubReleaseTimeTypes ReleaseTimeType { get; set; }
        public GitHubReleaseTypes ReleaseType { get; set; }
        public string ReleaseURL { get; set; }
        public string ReleaseNotes { get; set; }
        public ReleaseAsset BinAsset { get; set; }
        public ReleaseAsset InstallerAsset { get; set; }

        public bool BinAssetAvailable { get { return BinAsset != null; } }
        public bool InstallerAssetAvailable { get { return InstallerAsset != null; } }
    }
}
