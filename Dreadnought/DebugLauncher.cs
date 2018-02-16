using System.Diagnostics;

namespace SupportTool.Dreadnought
{
    class DebugLauncher
    {
        public static Process CreateProcess(string executable)
        {
            return new Process
            {
                EnableRaisingEvents = true,
                StartInfo =
                {
                    UseShellExecute = true,
                    FileName = executable,
                    CreateNoWindow = true,
                    Arguments = "/debug",
                    Verb = "runas"
                }
            };
        }
    }
}
