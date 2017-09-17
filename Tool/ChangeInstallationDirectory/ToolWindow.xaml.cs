using Microsoft.Win32;
using SupportTool.Dreadnought;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SupportTool.Tool.ChangeInstallationDirectory
{
    partial class ToolWindow : Window
    {
        private LoggerInterface Logger;

        public ToolWindow(LoggerInterface logger)
        {
            Logger = logger;

            InitializeComponent();
        }

        private void InstallationInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            SaveChanges.IsEnabled = !InstallationInput.Text.Equals("");
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

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            InstallationFixer.fixInstallationkey(InstallationInput.Text);
            InstallationFixer.fixUninstallLink(InstallationInput.Text);

            Logger.Log(String.Format("Installation directory set to {0}", InstallationFinder.findInRegistry()));

            Hide();
        }

        public void GuessInputValue()
        {
            InstallationInput.TextChanged += InstallationInput_TextChanged;

            string installationDirectory = "";

            try
            {
                installationDirectory = InstallationFinder.findInDesktop();
            }
            catch
            {
                try
                {
                    installationDirectory = Path.Combine(InstallationFinder.findInRegistry(), "DreadnoughtLauncher.exe");
                }
                catch
                {
                    installationDirectory = "";
                }
            }

            InstallationInput.Text = installationDirectory;
        }
    }
}
