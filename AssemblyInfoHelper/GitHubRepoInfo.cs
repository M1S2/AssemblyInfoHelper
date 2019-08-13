using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyInfoHelper
{
    /// <summary>
    /// Informations about the GitHub repository
    /// </summary>
    public class GitHubRepoInfo
    {
        /// <summary>
        /// Owner of the repository on GitHub
        /// </summary>
        public string RepoOwner { get; set; }

        /// <summary>
        /// Name of the repository on GitHub
        /// </summary>
        public string RepoName { get; set; }

        /// <summary>
        /// Constructor of the GitHubRepoInfo
        /// </summary>
        /// <param name="repoOwner">Owner of the repository on GitHub</param>
        /// <param name="repoName">Name of the repository on GitHub</param>
        public GitHubRepoInfo(string repoOwner, string repoName)
        {
            RepoOwner = repoOwner;
            RepoName = repoName;
        }
    }
}
