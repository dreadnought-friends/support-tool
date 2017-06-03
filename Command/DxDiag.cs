using System.Diagnostics;

namespace SupportTool.Command
{
    class DxDiag : CommandInterface
    {
        public void Execute(Config config, FileAggregator fileAggregator, LoggerInterface logger)
        {
            if (!config.IncludeDxDiag)
            {
                logger.Log("Skipping DxDiag Dump");
                return;
            }

            logger.Log("Generating DxDiag dump");

            string reportFile = fileAggregator.AddVirtualFile("dxdiag.txt");

            Process process = new Process();
            process.EnableRaisingEvents = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "dxdiag.exe";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.Arguments = "/t" + reportFile;
            process.Start();
            process.WaitForExit();
            process.Close();
        }
    }
}
