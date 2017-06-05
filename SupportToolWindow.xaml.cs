﻿using SupportTool.Command;
using SupportTool.Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;

namespace SupportTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SupportToolWindow : Window
    {
        public readonly string Version = "1.0.0";

        private InMemoryLogger logger = new InMemoryLogger();
        private Config config;
        private FileAggregator fileAggregator;
        private List<CommandInterface> commands = new List<CommandInterface>();
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        private BackgroundReportLogger backgroundReportLogger;
        private Runner runner;

        public SupportToolWindow()
        {
            InitializeComponent();

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

            config = new Config(
                Version,
                Path.Combine(home, @"AppData\Local\DreadGame\Saved\Logs"),
                Path.Combine(home, "Desktop"),
                "DN_Support.zip"
            );

            backgroundReportLogger = new BackgroundReportLogger(logger, backgroundWorker);
            fileAggregator = new FileAggregator(Path.Combine(Path.GetTempPath() + "DN_Support"));
            runner = new Runner(config, fileAggregator, backgroundReportLogger);

            commands.Add(new CleanUp());
            commands.Add(new HostDeveloper());
            commands.Add(new CustomerSupportReadme());
            commands.Add(new DxDiag());
            commands.Add(new MsInfo());
            commands.Add(new DreadnoughtLogs());
            commands.Add(new AggregatedFileCollector());
            commands.Add(new Archiver());

            ConfigurationOptions.DataContext = config;
        }

        private void StartAggregateData(object sender, DoWorkEventArgs e)
        {
            logger.Clear();

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
            backgroundReportLogger.Log("[ERROR]-------------------------------------");
            backgroundReportLogger.Log("An unexpected error has occured preventing the program from collecting data.");
            backgroundReportLogger.Log(message);
            backgroundReportLogger.Log("[ERROR]-------------------------------------");
            backgroundReportLogger.Log("Please report this error at https://github.com/dreadnought-friends/support-tool");
        }

        private void FinishAggregateData(object sender, RunWorkerCompletedEventArgs e)
        {
            SettingDxDiag.IsEnabled = true;
            SettingMsInfo.IsEnabled = true;
            SettingLogFiles.IsEnabled = true;
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
            Process.Start(fileAggregator.TempDir);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
        }

        private void OpenDreadnoughtInstallationDirectory_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(config.DnInstallationDirectory);
        }
    }
}
