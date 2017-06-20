using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;

namespace SupportTool.Dreadnought
{
    class InstallationFinder
    {
        public static string findInRegistry()
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(Installation.InstallationKey);

            if (null == registryKey)
            {
                throw new Exception.KeyNotFoundException();
            }

            string dir = (string)registryKey.GetValue(Installation.InstallationValueName);

            if (!Directory.Exists(dir))
            {
                throw new Exception.InvalidLocationException(dir);
            }

            return dir;
        }

        public static string findInDesktop()
        {
            List<FileInfo> files = new List<FileInfo>();
            files.AddRange(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory)).GetFiles("*.lnk"));
            files.AddRange(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)).GetFiles("*.lnk"));
            
            foreach (FileInfo file in files)
            {
                WshShell shell = new WshShell();
                string shortcut = shell.CreateShortcut(file.FullName).TargetPath;

                if (shortcut.Contains("DreadnoughtLauncher.exe")) {
                    return shortcut;
                }

            }

            throw new Exception.DreadnoughtShortcutNotFoundException();
        }
    }
}
