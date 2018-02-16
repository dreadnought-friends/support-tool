using SupportTool.Dreadnought;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace SupportTool.Command
{
    class HostDeveloper : CommandInterface, CommandCheckBoxInterface
    {
        private const string DeveloperFile = "host.developer.log";

        public string ConfigPropertyPath
        {
            get
            {
                return "IncludeHostDeveloper";
            }
        }

        public string Text
        {
            get
            {
                return "Launcher debug information";
            }
        }

        public string ToolTip
        {
            get
            {
                return "Starts the launcher in debug mode, needs admin privileges.";
            }
        }

        public void Execute(Config config, FileAggregator fileAggregator, LoggerInterface logger, Propagation propagation)
        {
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
            if (!config.IsElevated)
            {
                logger.Log("A windows user account control popup will appear to run the Dreadnought launcher as administrator");
            }
                        
            Process process = DebugLauncher.CreateProcess(launcherExecutable.FullName);

            try
            {
                process.Start();
            }
            catch (Win32Exception e)
            {
                if (e.NativeErrorCode == 1223) // operation cancelled by user
                {
                    logger.Log("Skipping host.developer dump, this option requires administrative permissions.");
                    return;
                }
                else
                {
                    throw e;
                }
            }

            fileAggregator.AddExistingFile(hostDeveloperFile.FullName, DeveloperFile);

            DateTime firstWrite = hostDeveloperFile.Exists ? hostDeveloperFile.LastWriteTime : new DateTime();

            Thread.Sleep(3000);

            // have to manually poll, in program files it will not create the file
            // in any other directory, it will have write permissions and thus write
            // to the log file on the first run as well.
            while (hostDeveloperFile.LastWriteTime.CompareTo(firstWrite) < 1)
            {
                Thread.Sleep(2000);
                hostDeveloperFile.Refresh();
            }

            process = FindDreadnoughtLauncherProcess();

            if (null == process)
            {
                logger.Log(String.Format("Expected a Dreadnought launcher process but it was not found, please allow the debug launcher to run in the popup", DeveloperFile));
                propagation.ShouldStop = true;
                return;
            }

            process.Kill();
            logger.Log("Automatically closed the debug launcher");
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
