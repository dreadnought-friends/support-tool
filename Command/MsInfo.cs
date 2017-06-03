using System.Diagnostics;
using System.IO;

namespace SupportTool.Command
{
    class MsInfo : CommandInterface
    {
        public void Execute(Config config, FileAggregator fileAggregator, LoggerInterface logger)
        {
            if (!config.IncludeMsInfo)
            {
                logger.Log("Skipping msinfo32 dump");
                return;
            }

            logger.Log("Generating msinfo32 dump");

            FileInfo reportFile = fileAggregator.AddVirtualFile("msinfo32.txt");

            Process process = new Process();
            process.EnableRaisingEvents = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "msinfo32.exe";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.Arguments = "/report " + reportFile.FullName;
            process.Start();
            process.WaitForExit();
            process.Close();
        }
    }
}
