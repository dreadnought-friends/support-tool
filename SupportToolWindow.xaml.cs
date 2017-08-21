using SupportTool.AppVersion;
using SupportTool.Command;
using SupportTool.Dreadnought;
using SupportTool.Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Windows;
using System.Windows.Navigation;

namespace SupportTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SupportToolWindow : Window
    {
        private Config config;
        private FileAggregator fileAggregator;
        private List<CommandInterface> commands = new List<CommandInterface>();
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        private TextBoxLogger textBoxLogger;
        private Runner runner;
        private ChangeInstallationDirectory changeInstallationDirectory;

        public SupportToolWindow()
        {
            InitializeComponent();
            
            InMemoryLogger inMemoryLogger = new InMemoryLogger();
            textBoxLogger = new TextBoxLogger(inMemoryLogger, ExecutionOutput);
            changeInstallationDirectory = new ChangeInstallationDirectory(textBoxLogger);

            Version versionInfo = Assembly.GetExecutingAssembly().GetName().Version;
            string version = String.Format("{0}.{1}.{2}", versionInfo.Major, versionInfo.Minor, versionInfo.Build);

            Title = String.Format("{0} - {1}", Title, version);

            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += new DoWorkEventHandler(StartAggregateData);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(FinishAggregateData);
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(WorkerProgressChanged);

            string home = Environment.GetEnvironmentVariable("userprofile");

            bool isElevated;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }

            config = new Config(
                version,
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"DreadGame\Saved\Logs"),
                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                "DN_Support.zip",
                "https://raw.githubusercontent.com/dreadnought-friends/tool-versions/master/versions.xml",
                isElevated
            );

            VersionChecker versionChecker = new VersionChecker(config);

            BackgroundReportLogger backgroundReportLogger = new BackgroundReportLogger(inMemoryLogger, backgroundWorker);
            fileAggregator = new FileAggregator(Path.Combine(Path.GetTempPath() + "DN_Support"));
            runner = new Runner(config, fileAggregator, backgroundReportLogger);

            commands.Add(new TempDirectoryPreparation());
            commands.Add(new HostDeveloper());
            commands.Add(new CustomerSupportReadme());
            commands.Add(new DxDiag());
            commands.Add(new MsInfo());
            commands.Add(new DreadnoughtLogs());
            commands.Add(new DreadnoughtCrashDumps());
            commands.Add(new AggregatedFileCollector());
            commands.Add(new Archiver());

            ConfigurationOptions.DataContext = config;
            DownloadNewVersionText.Text = "";

            try
            {
                VersionInfo info = versionChecker.getLatestVersionInfo();

                textBoxLogger.Log(info.MotdTitle);
                textBoxLogger.Log(info.MotdBody);

                if (!info.IsUpToDate)
                {
                    DownloadNewVersionLink.NavigateUri = new Uri(info.Url);
                    DownloadNewVersionText.Text = String.Format("Version {0} is available!", info.Version);
                }
            }
            catch (Exception e)
            {
                textBoxLogger.Log(String.Format("Unable to check for a new version: {0}", e.Message));
            }

            if (!config.IsElevated)
            {
                ChangeInstallationDirectory.IsEnabled = false;
                ChangeInstallationDirectory.Header += " (Restart as Admin)";
            }
        }

        private void StartAggregateData(object sender, DoWorkEventArgs e)
        {
            textBoxLogger.Clear();

            try
            {
                runner.Run(commands);
            }
            catch (Exception exception)
            {
                LogCriticalError(exception.Message);
            }
        }

        private void LogCriticalError(string message)
        {
            textBoxLogger.Log("[ERROR]-------------------------------------");
            textBoxLogger.Log("An unexpected error has occured preventing the program from collecting data.");
            textBoxLogger.Log(message);
            textBoxLogger.Log("[ERROR]-------------------------------------");
            textBoxLogger.Log("Please report this error at https://github.com/dreadnought-friends/support-tool");
        }

        private void FinishAggregateData(object sender, RunWorkerCompletedEventArgs e)
        {
            SettingDxDiag.IsEnabled = true;
            SettingMsInfo.IsEnabled = true;
            SettingLogFiles.IsEnabled = true;
            SettingCrashDumps.IsEnabled = true;
            SettingArchive.IsEnabled = true;
            SettingHostDeveloper.IsEnabled = true;
            StartAggregation.IsEnabled = true;
            OpenAggregatedFiles.IsEnabled = true;
        }

        private void WorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ExecutionOutput.AppendText(e.UserState.ToString() + Environment.NewLine);
            ExecutionOutput.ScrollToEnd();
        }

        private void StartAggregation_Click(object sender, RoutedEventArgs e)
        {
            SettingDxDiag.IsEnabled = false;
            SettingMsInfo.IsEnabled = false;
            SettingLogFiles.IsEnabled = false;
            SettingCrashDumps.IsEnabled = false;
            SettingArchive.IsEnabled = false;
            SettingHostDeveloper.IsEnabled = false;
            StartAggregation.IsEnabled = false;
            OpenAggregatedFiles.IsEnabled = false;

            // ensure a clean text field if generating again
            ExecutionOutput.Clear();

            backgroundWorker.RunWorkerAsync();
        }

        private void OpenAggregatedFiles_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(fileAggregator.TempDir))
            {
                textBoxLogger.Log("No aggregated files found to show");
                return;
            }

            Process.Start(fileAggregator.TempDir);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
        }

        private void OpenDreadnoughtInstallationDirectory_Click(object sender, RoutedEventArgs e)
        {
            if (null == config.DnInstallationDirectory)
            {
                textBoxLogger.Log("Could not reliably find the Dreadnought installation directory (Hint: try Tools > Change Installation Directory)");
                return;
            }

            var process = Process.Start(new ProcessStartInfo()
            {
                FileName = config.DnInstallationDirectory,
                UseShellExecute = true,
                Verb = "Open"
            });
        }

        private void ChangeChangeInstallationDirectory_Click(object sender, RoutedEventArgs e)
        {
            if (changeInstallationDirectory.IsVisible)
            {
                changeInstallationDirectory.Activate();
                return;
            }

            changeInstallationDirectory.Show();
            changeInstallationDirectory.Activate();
            changeInstallationDirectory.guessInputValue();
        }
    }
}
