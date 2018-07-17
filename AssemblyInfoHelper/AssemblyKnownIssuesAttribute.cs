using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyInfoHelper
{
    /// <summary>
    /// Attribute to report known issues/bugs
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class AssemblyKnownIssuesAttribute : Attribute
    {
        /// <summary>
        /// Text describing the issue
        /// </summary>
        public string IssueText { get; set; }

        /// <summary>
        /// Constructor of the AssemblyKnownIssuesAttribute
        /// </summary>
        /// <param name="issueText">Text describing the issue</param>
        public AssemblyKnownIssuesAttribute(string issueText)
        {
            IssueText = issueText;
        }

    }
}
