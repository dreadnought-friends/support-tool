using System.Diagnostics;
using System.IO;

namespace SupportTool.Command
{
    class MsInfo : CommandInterface, CommandCheckBoxInterface
    {
        public string ConfigPropertyPath
        {
            get
            {
                return "IncludeMsInfo";
            }
        }

        public string Text
        {
            get
            {
                return "MSInfo32";
            }
        }

        public string ToolTip { get; }

        public void Execute(Config config, FileAggregator fileAggregator, LoggerInterface logger, Propagation propagation)
        {
            if (!config.IncludeMsInfo)
            {
                logger.Log("Skipping msinfo32 dump");
                return;
            }

            logger.Log("Generating msinfo32 dump");

            FileInfo reportFile = fileAggregator.AddVirtualFile("msinfo32.nfo");

            Process process = new Process();
            process.EnableRaisingEvents = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "msinfo32.exe";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.Arguments = string.Format("/nfo {0}", reportFile.FullName);
            process.Start();
            process.WaitForExit();
            process.Close();
        }
    }
}
