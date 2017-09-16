using Microsoft.Win32;
using SupportTool.Dreadnought;
using System.IO;

namespace SupportTool.Tool.ChangeInstallationDirectory
{
    class InstallationFixer
    {
        public static void fixInstallationkey(string installDir)
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(Installation.InstallationKey, true);

            if (null == registryKey)
            {
                Registry.LocalMachine.CreateSubKey(Installation.InstallationKey);
            }

            registryKey.SetValue(Installation.InstallationValueName, installDir.Replace(@"\DreadnoughtLauncher.exe", ""));
        }

        public static void fixUninstallLink(string installDir)
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(Installation.UninstallKey, true);

            if (null == registryKey)
            {
                Registry.LocalMachine.CreateSubKey(Installation.UninstallKey);
            }

            string dir = installDir.Replace(@"\DreadnoughtLauncher.exe", "");

            registryKey.SetValue(Installation.UninstallValueName, Path.Combine(dir, "Uninstall.exe"));
            registryKey.SetValue(Installation.IconValueName, Path.Combine(dir, "DreadGame.ico"));
        }
    }
}
