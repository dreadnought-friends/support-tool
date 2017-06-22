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
                logger.Log("Skipping host.developer Dump, unable to detect where Dreadnought is installed (Hint: try Tools > Change Installation Directory)");
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

            // some players run it with /debug by default, thus might have useful information in the old file
            if (hostDeveloperFile.Exists)
            {
                hostDeveloperFile.CopyTo(String.Format("{0}.old", Path.Combine(fileAggregator.TempDir, hostDeveloperFile.Name)));
                fileAggregator.AddVirtualFile(String.Format("{0}.old", hostDeveloperFile.Name));
            }

            if (null != FindDreadnoughtLauncherProcess())
            {
                logger.Log("A running Dreadnought launcher is found, close the dreadnought launcher and try again");
                propagation.ShouldStop = true;
                return;
            }

            logger.Log(string.Format("Creating {0}, this might take a while", hostDeveloperFile.FullName));
            logger.Log("A windows user account control popup might appear to run the Dreadnought launcher as administrator");
            
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

            Process launcherProcess = FindDreadnoughtLauncherProcess();

            if (null == launcherProcess)
            {
                logger.Log(String.Format("Expected a Dreadnought launcher process but it was not found, please allow the debug launcher to run in the popup", DeveloperFile));
                propagation.ShouldStop = true;
                return;
            }
            
            launcherProcess.Kill();
            logger.Log("Automatically closed the debug launcher");

            watcher.WaitForChanged(WatcherChangeTypes.Changed);
        }

        private Process FindDreadnoughtLauncherProcess()
        {
            Process[] processes = Process.GetProcessesByName("DreadnoughtLauncher");

            if (1 != processes.Length)
            {
                return null;
            }

            return processes[0];
        }
    }
}
