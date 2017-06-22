namespace SupportTool.Dreadnought
{
    class Installation
    {
        public const string UninstallKey = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Dreadnought";
        public const string InstallationKey = @"SOFTWARE\WOW6432Node\Grey Box\Dreadnought";
        public const string InstallationValueName = "Install_Dir";
        public const string UninstallValueName = "UninstallString";
        public const string IconValueName = "DisplayIcon";
    }
}
