namespace SupportTool
{
    class Config
    {
        public string LogFileLocation { get; private set; }
        public string ZipFileLocation { get; private set; }
        public string ZipFileName { get; private set; }

        public bool CreateZipArchive { get; set; } = true;
        public bool IncludeMsInfo { get; set; } = true;
        public bool IncludeDxDiag { get; set; } = true;
        public bool IncludeDreadnoughtLogs { get; set; } = true;

        public bool CanCreateArchive
        {
            get { return IncludeDreadnoughtLogs || IncludeDxDiag || IncludeMsInfo; }
        }

        public Config(string logFileLocation, string zipFileLocation, string zipFileName)
        {
            this.LogFileLocation = logFileLocation;
            this.ZipFileLocation = zipFileLocation;
            this.ZipFileName = zipFileName;
        }
    }
}
