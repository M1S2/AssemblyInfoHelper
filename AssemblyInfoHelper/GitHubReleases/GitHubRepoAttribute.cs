using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyInfoHelper.GitHubReleases
{
    /// <summary>
    /// Attribute to assign informations about the GitHub repository
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class GitHubRepoAttribute : Attribute
    {
        /// <summary>
        /// Repository url
        /// </summary>
        public string RepoUrl { get; set; }

        /// <summary>
        /// Constructor of the GitHubRepoAttribute
        /// </summary>
        /// <param name="repoUrl">Repository url</param>
        public GitHubRepoAttribute(string repoUrl)
        {
            RepoUrl = repoUrl;
        }
    }
}
