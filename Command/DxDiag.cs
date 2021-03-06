﻿using System.Diagnostics;
using System.IO;

namespace SupportTool.Command
{
    class DxDiag : CommandInterface, CommandCheckBoxInterface
    {
        public string ConfigPropertyPath
        {
            get
            {
                return "IncludeDxDiag";
            }
        }

        public string Text
        {
            get
            {
                return "DxDiag";
            }
        }

        public string ToolTip
        {
            get
            {
                return "Runs DxDiag to gather useful system information.";
            }
        }

        public void Execute(Config config, FileAggregator fileAggregator, LoggerInterface logger, Propagation propagation)
        {
            logger.Log("Generating DxDiag dump");

            FileInfo reportFile = fileAggregator.AddVirtualFile("dxdiag.txt");

            Process process = new Process();
            process.EnableRaisingEvents = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "dxdiag.exe";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.Arguments = string.Format("/t {0}", reportFile.FullName);
            process.Start();
            process.WaitForExit();
            process.Close();
        }
    }
}
