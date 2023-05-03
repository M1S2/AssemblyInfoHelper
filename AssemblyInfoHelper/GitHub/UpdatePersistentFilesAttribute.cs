using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyInfoHelper.GitHub
{
    /// <summary>
    /// Attribute to assign informations about files that should be persistent (kept) during update
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class UpdatePersistentFilesAttribute : Attribute
    {
        /// <summary>
        /// Name of the file to persist relative to the updatee directory
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Constructor of the <see cref="UpdatePersistentFilesAttribute"/>
        /// </summary>
        /// <param name="fileName">Name of the file to persist relative to the updatee directory</param>
        public UpdatePersistentFilesAttribute(string fileName)
        {
            FileName = fileName;
        }
    }
}
