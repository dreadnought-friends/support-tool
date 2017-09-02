using SupportTool.Dreadnought;

namespace SupportTool
{
    class Config
    {
        public string Version { get; private set; }
        public string DnInstallationDirectory
        {
            get
            {
                try
                {
                    return InstallationFinder.findInRegistry();
                }
                catch
                {
                    return null;
                }
            }
        }
        public string LogFileLocation { get; private set; }
        public string ZipFileLocation { get; private set; }
        public string ZipFileName { get; private set; }
        public string VersionInfoFileUrl { get; private set; }
        public bool IsElevated { get; private set; }

        public bool ShowLogTimes = false;

        public bool CreateZipArchive { get; set; } = true;
        public bool IncludeMsInfo { get; set; } = true;
        public bool IncludeDxDiag { get; set; } = true;
        public bool IncludeConnection { get; set; } = true;
        public bool IncludeDreadnoughtLogs { get; set; } = true;
        public bool IncludeHostDeveloper { get; set; } = true;
        public bool IncludeDreadnoughtCrashDumps { get; set; } = true;

        public bool CanCreateArchive
        {
            get { return IncludeDreadnoughtLogs || IncludeDreadnoughtCrashDumps || IncludeConnection || IncludeDxDiag || IncludeMsInfo || IncludeHostDeveloper; }
        }

        public Config(string version, string logFileLocation, string zipFileLocation, string zipFileName, string versionInfoFileUrl, bool isElevated)
        {
            Version = version;
            LogFileLocation = logFileLocation;
            ZipFileLocation = zipFileLocation;
            ZipFileName = zipFileName;
            VersionInfoFileUrl = versionInfoFileUrl;
            IsElevated = isElevated;
        }
    }
}
