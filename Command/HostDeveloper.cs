using System;
using System.Diagnostics;
using System.IO;

namespace SupportTool.Command
{
    class HostDeveloper : CommandInterface
    {
        private const string DeveloperFile = "host.developer.log";

        public void Execute(Config config, FileAggregator fileAggregator, LoggerInterface logger, Propagation propagation)
        {
            if (!config.IncludeHostDeveloper)
            {
                logger.Log("Skipping host.developer Dump");
                return;
            }

            if (null == config.DnInstallationDirectory)
            {
                logger.Log("Skipping host.developer Dump, unable to detect where Dreadnought is installed");
                propagation.ShouldStop = true;
                return;
            }

            FileInfo launcherExecutable = new FileInfo(Path.Combine(config.DnInstallationDirectory, "DreadnoughtLauncher.exe"));

            if (!launcherExecutable.Exists)
            {
                logger.Log("Skipping host.developer Dump, cannot find the DreadnoughtLauncher.exe");
                propagation.ShouldStop = true;
                return;
            }

            FileInfo hostDeveloperFile = new FileInfo(Path.Combine(config.DnInstallationDirectory, DeveloperFile));
            FileSystemWatcher watcher = new FileSystemWatcher(config.DnInstallationDirectory, DeveloperFile);

            logger.Log(string.Format("Creating {0}, this might take a while", hostDeveloperFile.FullName));
            logger.Log("A windows user control popup will appear to run DreadnoughtLauncher.exe as administrator");
            
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.EnableRaisingEvents = true;

            FileInfo reportFile = fileAggregator.AddExistingFile(hostDeveloperFile.FullName, DeveloperFile);

            Process process = new Process();
            process.EnableRaisingEvents = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = launcherExecutable.FullName;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.Arguments = "/debug";
            process.Start();
            process.WaitForExit();
            process.Close();

            Process[] processes = Process.GetProcessesByName("DreadnoughtLauncher");

            foreach (Process launcherProcess in processes)
            {
                // failsafe to prevent older processes from dying, ensures it's only recent
                if ((DateTime.Now - launcherProcess.StartTime).TotalSeconds < 10)
                {
                    launcherProcess.Kill();
                    logger.Log("Automatically closed the debug launcher");
                }
            }

            if (0 == processes.Length)
            {
                logger.Log(String.Format("Expected a DreadnoughtLauncher.exe process but it was not found, please allow the debug launcher to run in the popup", DeveloperFile));
                propagation.ShouldStop = true;
                return;
            }

            watcher.WaitForChanged(WatcherChangeTypes.Changed);
        }
    }
}
