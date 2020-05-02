using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AssemblyInfoHelper.Updater
{
    public static class FileDirectoryExtensions
    {
        /// <summary>
        /// Check the write access to the given file
        /// </summary>
        /// <param name="filePath">file to check</param>
        /// <returns>true if file is writeable, otherwise false</returns>
        /// see: https://github.com/Tyrrrz/Onova/blob/master/Onova.Updater/Internal/FileEx.cs
        public static bool CheckFileWriteAccess(string filePath)
        {
            try
            {
                File.Open(filePath, FileMode.Open, FileAccess.Write).Dispose();
                return true;
            }
            catch (UnauthorizedAccessException) { return false; }
            catch (IOException) { return false; }
            catch (Exception) { return false; }
        }

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Copy one directory to another
        /// </summary>
        /// <param name="sourceDirPath">Source directory</param>
        /// <param name="destDirPath">Destination directory</param>
        /// <param name="overwrite">Allow to overwrite files</param>
        /// see: https://github.com/Tyrrrz/Onova/blob/master/Onova.Updater/Internal/DirectoryEx.cs
        public async static Task CopyDirectory(string sourceDirPath, string destDirPath, bool overwrite = true)
        {
#warning Report progress while copying
            await Task.Run(async() =>
            {
                Directory.CreateDirectory(destDirPath);

                // Get all files in source directory
                var sourceFilePaths = Directory.EnumerateFiles(sourceDirPath);

                // Copy them
                foreach (var sourceFilePath in sourceFilePaths)
                {
                    // Get destination file path
                    var destFileName = Path.GetFileName(sourceFilePath);
                    var destFilePath = Path.Combine(destDirPath, destFileName);
                    File.Copy(sourceFilePath, destFilePath, overwrite);
                }

                // Get all subdirectories in source directory
                var sourceSubDirPaths = Directory.EnumerateDirectories(sourceDirPath);

                // Recursively copy them
                foreach (var sourceSubDirPath in sourceSubDirPaths)
                {
                    var destSubDirName = Path.GetFileName(sourceSubDirPath);
                    var destSubDirPath = Path.Combine(destDirPath, destSubDirName);
                    await CopyDirectory(sourceSubDirPath, destSubDirPath, overwrite);
                }
            });
        }
        //********************************************************************************************************************************************************************

        /// <summary>
        /// Delete the given directory with all subfolders and files. This method is async.
        /// </summary>
        /// <param name="dirPath">directory to delete</param>
        /// <param name="keepExeConfigFiles">Keep all .exe.config files</param>
        /// <param name="ignoreDeleteErrors">Ignore errors while deleting files</param>
        /// <param name="progress">Interface for progress reporting</param>
        public async static Task DeleteDirectory(string dirPath, bool keepExeConfigFiles, bool ignoreDeleteErrors, IProgress<double> progress)
        {
            await Task.Run(async() =>
            {
                List<string> filePaths = Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories).ToList();
                if (keepExeConfigFiles) { filePaths = filePaths.Where(p => Path.GetExtension(p) != ".exe.config").ToList(); }

                int filesDeleted = 0;
                foreach (string filePath in filePaths)
                {
                    try { File.Delete(filePath); }
                    catch (Exception ex)
                    {
                        if (!ignoreDeleteErrors) { throw ex; }
                    }

                    filesDeleted++;
                    progress.Report(((double)filesDeleted / filePaths.Count));
                }
                await DeleteEmptyFolders(dirPath);
            });
        }

        //********************************************************************************************************************************************************************

        /// <summary>
        /// This function deletes all empty folders in the given path.
        /// </summary>
        /// <param name="startFolder">The folder to start from.</param>
        /// see: https://stackoverflow.com/questions/2811509/c-sharp-remove-all-empty-subdirectories
        public async static Task DeleteEmptyFolders(string startFolder)
        {
            await Task.Run(async() =>
            {
                if (!Directory.Exists(startFolder)) { return; }

                foreach (string directory in Directory.GetDirectories(startFolder))
                {
                    await DeleteEmptyFolders(directory);
                    if (Directory.GetFiles(directory).Length == 0 && Directory.GetDirectories(directory).Length == 0)
                    {
                        Directory.Delete(directory, false);
                    }
                }
            });
        }
    }
}
