using Microsoft.Win32;
using SupportTool.Dreadnought.Exception;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SupportTool.Dreadnought
{
    partial class ChangeInstallationDirectory : Window
    {
        private LoggerInterface logger;

        public ChangeInstallationDirectory(LoggerInterface logger)
        {
            this.logger = logger;

            InitializeComponent();

            InstallationInput.TextChanged += InstallationInput_TextChanged;

            string installationDirectory = "";

            try
            {
                installationDirectory = InstallationFinder.findInDesktop();
            }
            catch (DreadnoughtShortcutNotFoundException)
            {
            }

            InstallationInput.Text = installationDirectory;
        }

        private void InstallationInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyInstallationDirectory.IsEnabled = !InstallationInput.Text.Equals("");
        }

        private void FindInstallationButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = Path.GetPathRoot(Environment.SystemDirectory);
            fileDialog.Filter = "Dreadnought Launcher|DreadnoughtLauncher.exe";
            fileDialog.FilterIndex = 0;

            if (!InstallationInput.Text.Equals(""))
            {
                FileInfo guessedFile = new FileInfo(InstallationInput.Text);

                if (guessedFile.Exists)
                {
                    fileDialog.InitialDirectory = guessedFile.DirectoryName;
                }
            }
           
            if (true == fileDialog.ShowDialog())
            {
                InstallationInput.Text = fileDialog.FileName;
            }
        }

        private void ApplyInstallationDirectory_Click(object sender, RoutedEventArgs e)
        {
            InstallationFixer.fixInstallationkey(InstallationInput.Text);
            InstallationFixer.fixUninstallLink(InstallationInput.Text);

            this.logger.Log(String.Format("Installation directory set to {0}", InstallationFinder.findInRegistry()));

            Close();
        }
    }
}
