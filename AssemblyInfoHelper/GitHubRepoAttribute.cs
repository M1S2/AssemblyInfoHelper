using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyInfoHelper
{
    /// <summary>
    /// Attribute to assign informations about the GitHub repository
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class GitHubRepoAttribute : Attribute
    {
        /// <summary>
        /// Repository info
        /// </summary>
        public GitHubRepoInfo RepoInfo { get; set; }

        /// <summary>
        /// Constructor of the GitHubRepoAttribute
        /// </summary>
        /// <param name="repoInfo">Repository info</param>
        public GitHubRepoAttribute(GitHubRepoInfo repoInfo)
        {
            RepoInfo = repoInfo;
        }

        /// <summary>
        /// Constructor of the GitHubRepoAttribute
        /// </summary>
        /// <param name="repoOwner">Owner of the repository on GitHub</param>
        /// <param name="repoName">Name of the repository on GitHub</param>
        public GitHubRepoAttribute(string repoOwner, string repoName)
        {
            RepoInfo = new GitHubRepoInfo(repoOwner, repoName);
        }
    }
}
