using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyInfoHelper.Update
{
    public static class UpdateUtils
    {
        //see: https://github.com/Tyrrrz/Onova/blob/master/Onova/UpdateManager.cs
        public static async void LaunchUpdater(string downloadFolder, bool restart)
        {
            string updaterFilePath = Path.Combine(Directory.GetParent(downloadFolder).FullName, $"{AssemblyInfoHelperClass.AssemblyTitle}.Updater.exe");

            // Extract updater exe
            await ExtractManifestResourceAsync(Assembly.GetExecutingAssembly(), "AssemblyInfoHelper.Updater.exe", updaterFilePath);

            // Get original command line arguments and encode them to avoid issues with quotes
            string routedArgs = Convert.ToBase64String(Encoding.UTF8.GetBytes(GetCommandLineWithoutExecutable()));

            // Prepare arguments
            string updaterArgs = $"\"{Process.GetCurrentProcess().MainModule.FileName}\" \"{downloadFolder}\" \"{restart}\" \"{routedArgs}\"";

            // Create updater process start info
            var updaterStartInfo = new ProcessStartInfo
            {
                FileName = updaterFilePath,
                Arguments = updaterArgs,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process updaterProcess = Process.Start(updaterStartInfo);
        }

        //********************************************************************************************************************************************************************

        private static async Task ExtractManifestResourceAsync(Assembly assembly, string resourceName, string destFilePath)
        {
            using (Stream input = assembly.GetManifestResourceStream(resourceName) ?? throw new Exception($"Could not find resource [{resourceName}]."))
            {
                using (FileStream output = File.Create(destFilePath))
                {
                    await input.CopyToAsync(output);
                }
            }
        }

        //********************************************************************************************************************************************************************

        private static string GetCommandLineWithoutExecutable()
        {
            // Get the executable name
            string exeName = Environment.GetCommandLineArgs().First();
            string quotedExeName = $"\"{exeName}\"";

            // Remove the quoted executable name from command line and return it
            if (Environment.CommandLine.StartsWith(quotedExeName, StringComparison.OrdinalIgnoreCase))
                return Environment.CommandLine.Substring(quotedExeName.Length).Trim();

            // Remove the unquoted executable name from command line and return it
            if (Environment.CommandLine.StartsWith(exeName, StringComparison.OrdinalIgnoreCase))
                return Environment.CommandLine.Substring(exeName.Length).Trim();

            // Safe guard, shouldn't reach here
            return Environment.CommandLine;
        }

    }
}
