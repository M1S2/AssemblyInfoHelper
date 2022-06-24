using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using AssemblyInfoHelper.GitHub;
using Semver;
using MahApps.Metro.Controls.Dialogs;
using Octokit;

namespace AssemblyInfoHelper.GitHub
{
    public static class UpdateUtils
    {
        /// <summary>
        /// Status of the GitHub update feature (is update running, update progress)
        /// </summary>
        public static UpdateStatusInfo UpdateStatus { get; set; } = new UpdateStatusInfo();

        //####################################################################################################################################################################

        /// <summary>
        /// Update the application to the given target release. This can also be a downgrade (lower version) or repair (same version).
        /// </summary>
        /// <param name="targetRelease">New version after update</param>
        public static async Task RunUpdate(GitHubRelease targetRelease)
        {
            WindowAssemblyInfo windowAssemblyInfo = System.Windows.Application.Current.Windows.OfType<MahApps.Metro.Controls.MetroWindow>().OfType<WindowAssemblyInfo>().FirstOrDefault();
            Version targetVersion = new Version(targetRelease.Version.ToString());
            try
            {
                UpdateStatus.FromVersion = new SemVersion(new Version(AssemblyInfoHelperClass.AssemblyVersion));
                UpdateStatus.ToVersion = targetRelease.Version;
                UpdateStatus.IsUpdateRunning = true;

                MessageDialogResult messageResult = await windowAssemblyInfo.ShowMessageAsync(Properties.Resources.UpdateUtilConfirmUpdateTitleString, Properties.Resources.UpdateUtilDoYouReallyWantToString + UpdateStatus.UpdateText + "?" + ((UpdateStatus.FromVersion > UpdateStatus.ToVersion) ? Environment.NewLine + Environment.NewLine + Properties.Resources.UpdateUtilDowngradeWarningString : ""), MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() { AffirmativeButtonText = Properties.Resources.MessageDialogOKString, NegativeButtonText = Properties.Resources.MessageDialogCancelString });
                if (messageResult == MessageDialogResult.Negative) { UpdateStatus.IsUpdateRunning = false; return; }

                bool useBinaries = false, useInstaller = false;

                if (targetRelease.BinAsset != null && targetRelease.InstallerAsset == null) { useBinaries = true; }         // If only bin asset exists, use this as update source
                else if (targetRelease.BinAsset == null && targetRelease.InstallerAsset != null) { useInstaller = true; }   // If only installer asset exists, use the installer for update
                else if (targetRelease.BinAsset != null && targetRelease.InstallerAsset != null)                            // If both bin asset and installer asset exist, let the user choose the update source
                {
                    messageResult = await windowAssemblyInfo.ShowMessageAsync(Properties.Resources.UpdateUtilChooseSourceTitleString, Properties.Resources.UpdateUtilChooseSourceString, MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() { AffirmativeButtonText = "Binaries", NegativeButtonText = "Installer", DefaultButtonFocus = MessageDialogResult.Affirmative });
                    if (messageResult == MessageDialogResult.Affirmative) { useBinaries = true; }
                    else { useInstaller = true; }
                }

                // Decide which asset to download
                ReleaseAsset downloadAsset = (useBinaries ? targetRelease.BinAsset : (useInstaller ? targetRelease.InstallerAsset : null));
                if (downloadAsset == null)
                {
                    await windowAssemblyInfo.ShowMessageAsync(Properties.Resources.UpdateUtilReleaseAssetNotFoundTitleString, Properties.Resources.UpdateUtilReleaseAssetNotFoundString.Replace("0.0.0", targetVersion.ToString()));
                    UpdateStatus.IsUpdateRunning = false;
                    return;
                }

                // Delete and recreate the download folder
                string downloadFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AssemblyInfoHelperClass.AssemblyTitle, targetVersion.ToString());
                if (Directory.Exists(downloadFolder)) { Directory.Delete(downloadFolder, true); }
                Directory.CreateDirectory(downloadFolder);

                // Download Assets; if download fails, try 10 times
                WebClient client = new WebClient();
                client.DownloadProgressChanged += (sender, args) => { UpdateStatus.UpdateProgress = args.ProgressPercentage; };
                string downloadFilePath = Path.Combine(downloadFolder, Path.GetFileName(downloadAsset.BrowserDownloadUrl));

                int failedDownloads = 0;
                do
                {
                    try
                    {
                        await client.DownloadFileTaskAsync(downloadAsset.BrowserDownloadUrl, downloadFilePath);
                        failedDownloads = -1;
                    }
                    catch (WebException) { failedDownloads++; }
                    catch (InvalidOperationException) { failedDownloads++; }
                } while (failedDownloads != -1 && failedDownloads < 10);

                if (failedDownloads != -1)
                {
                    await windowAssemblyInfo.ShowMessageAsync(Properties.Resources.UpdateUtilDownloadFailedTitleString, Properties.Resources.UpdateUtilDownloadFailedString.Replace("<Path>", "\"" + downloadAsset.BrowserDownloadUrl + "\""));
                    UpdateStatus.IsUpdateRunning = false;
                    return;
                }

                // Extract zip files if asset is zip file
                if (Path.GetExtension(downloadFilePath) == ".zip")
                {
                    ZipFile.ExtractToDirectory(downloadFilePath, downloadFolder);
                    File.Delete(downloadFilePath);                                  // delete zip file after extraction
                }

                if (useBinaries)
                {
                    UpdateUtils.LaunchUpdater(downloadFolder, true);           // Launch an executable that will apply the update and restart the application afterwards
                }
                else if (useInstaller)
                {
                    if (!File.Exists(Path.Combine(downloadFolder, "Setup.exe")))
                    {
                        await windowAssemblyInfo.ShowMessageAsync("Setup.exe", Properties.Resources.UpdateUtilSetupExeFailureString);
                        UpdateStatus.IsUpdateRunning = false;
                        return;
                    }
                    Process.Start(Path.Combine(downloadFolder, "Setup.exe"));
                }

                if (useInstaller) { await windowAssemblyInfo.ShowMessageAsync("Update", Properties.Resources.UpdateUtilUpdateFinishedInstallerString, MessageDialogStyle.Affirmative); }
                else if (useBinaries) { await windowAssemblyInfo.ShowMessageAsync("Update", Properties.Resources.UpdateUtilUpdateFinishedBinariesString, MessageDialogStyle.Affirmative); }

                Environment.Exit(0);                            // Terminate the running application so that the updater/installer can overwrite files
            }
            catch (Exception ex)
            {
                await windowAssemblyInfo?.ShowMessageAsync(Properties.Resources.UpdateUtilErrorString, ex.Message);
                UpdateStatus.IsUpdateRunning = false;
            }
        }

        //********************************************************************************************************************************************************************

        //see: https://github.com/Tyrrrz/Onova/blob/master/Onova/UpdateManager.cs
        public static async void LaunchUpdater(string downloadFolder, bool restart)
        {
            string updaterFileExePath = Path.Combine(Directory.GetParent(downloadFolder).FullName, $"{AssemblyInfoHelperClass.AssemblyTitle}.Updater.exe");
           
            // Extract updater exe
            await ExtractManifestResourceAsync(Assembly.GetExecutingAssembly(), "AssemblyInfoHelper.Updater.exe", updaterFileExePath);

            string updaterFileResourcesPath = Path.Combine(Directory.GetParent(downloadFolder).FullName, $"de-DE\\AssemblyInfoHelper.Updater.resources.dll");

            // Extract updater resources dll
            if (!Directory.Exists(Path.GetDirectoryName(updaterFileResourcesPath))) { Directory.CreateDirectory(Path.GetDirectoryName(updaterFileResourcesPath)); }
            await ExtractManifestResourceAsync(Assembly.GetExecutingAssembly(), "AssemblyInfoHelper.Updater.resources.dll", updaterFileResourcesPath);

#if NETCOREAPP || NET
            string updaterFileDllPath = Path.Combine(Directory.GetParent(downloadFolder).FullName, "AssemblyInfoHelper.Updater.dll");
            string updaterFileRuntimeConfigPath = Path.Combine(Directory.GetParent(downloadFolder).FullName, "AssemblyInfoHelper.Updater.runtimeconfig.json");

            // Extract updater dll
            await ExtractManifestResourceAsync(Assembly.GetExecutingAssembly(), "AssemblyInfoHelper.Updater.dll", updaterFileDllPath);

            // Extract updater runtime config
            await ExtractManifestResourceAsync(Assembly.GetExecutingAssembly(), "AssemblyInfoHelper.Updater.runtimeconfig.json", updaterFileRuntimeConfigPath);
#endif

            // Get original command line arguments and encode them to avoid issues with quotes
            string routedArgs = Convert.ToBase64String(Encoding.UTF8.GetBytes(GetCommandLineWithoutExecutable()));

            // Prepare arguments
            string updaterArgs = $"\"{Process.GetCurrentProcess().MainModule.FileName}\" \"{downloadFolder}\" \"{restart}\" \"{routedArgs}\"";

            // Create updater process start info
            var updaterStartInfo = new ProcessStartInfo
            {
                FileName = updaterFileExePath,
                Arguments = updaterArgs,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process updaterProcess = Process.Start(updaterStartInfo);
        }

        //********************************************************************************************************************************************************************

        private static async Task ExtractManifestResourceAsync(Assembly assembly, string resourceName, string destFilePath)
        {
            using (Stream input = assembly.GetManifestResourceStream(resourceName)) // ?? throw new Exception($"Could not find resource [{resourceName}]."))
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
