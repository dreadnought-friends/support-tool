using Microsoft.Win32;
using System.IO;

namespace SupportTool
{
    class Config
    {
        public string Version { get; private set; }
        public string DnInstallationDirectory
        {
            get
            {
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Grey Box\Dreadnought");

                if (null == registryKey)
                {
                    return null;
                    
                }

                string dir = (string)registryKey.GetValue("Install_Dir");

                if (!Directory.Exists(dir))
                {
                    return null;
                }

                return dir;

            }
        }
        public string LogFileLocation { get; private set; }
        public string ZipFileLocation { get; private set; }
        public string ZipFileName { get; private set; }

        public bool CreateZipArchive { get; set; } = true;
        public bool IncludeMsInfo { get; set; } = true;
        public bool IncludeDxDiag { get; set; } = true;
        public bool IncludeDreadnoughtLogs { get; set; } = true;
        public bool IncludeHostDeveloper { get; set; } = false;

        public bool CanCreateArchive
        {
            get { return IncludeDreadnoughtLogs || IncludeDxDiag || IncludeMsInfo || IncludeHostDeveloper; }
        }

        public Config(string version, string logFileLocation, string zipFileLocation, string zipFileName)
        {
            this.Version = version;
            this.LogFileLocation = logFileLocation;
            this.ZipFileLocation = zipFileLocation;
            this.ZipFileName = zipFileName;
        }
    }
}
