using SupportTool.Ping;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SupportTool.Command
{
    class Connection : CommandInterface, CommandCheckBoxInterface
    {
        public string ConfigPropertyPath
        {
            get
            {
                return "IncludeConnection";
            }
        }

        public string Text
        {
            get
            {
                return "Connection info to game servers";
            }
        }

        public string ToolTip
        {
            get
            {
                return "A ping and traceroute will be executed for several IP addresses. Depending on how fast your connection to the server is at moment, it might take a while.";
            }
        }

        public void Execute(Config config, FileAggregator fileAggregator, LoggerInterface logger, Propagation propagation)
        {
            if (!config.IncludeConnection)
            {
                logger.Log("Skipping connection information to the Dreadnought servers");
                return;
            }

            List<FileInfo> files = Directory.GetFiles(config.LogFileLocation)
                .Select(x => new FileInfo(x))
                .ToList();

            if (0 == files.Count)
            {
                logger.Log("No IP addresses found to ping as no log files exist, skipping connection information.");
                return;
            }

            HashSet<string> ipAddresses = new HashSet<string>();

            foreach (FileInfo file in files)
            {
                ipAddresses.UnionWith(IpFinder.findIps(file));
            }

            if (0 == ipAddresses.Count)
            {
                logger.Log("No IP addresses found to ping, skipping connection information.");
                return;
            }

            try
            {
                CheckConnnection(ipAddresses.ToList(), fileAggregator, logger);
            }
            catch (Exception)
            {
                propagation.ShouldStop = true;
                logger.Log("Pinging the servers failed, please check your connection.");
            }
        }

        private void CheckConnnection(List<string> ipAddresses, FileAggregator fileAggregator, LoggerInterface logger)
        {
            FileInfo reportFile = fileAggregator.AddVirtualFile("connection-information.txt");

            // trace route to the first IP in the list
            Process process = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    FileName = "cmd.exe",
                    Arguments = "/C tracert -d " + ipAddresses[0]
                }
            };
            process.Start();

            logger.Log("Pinging known dreadnought servers, this might take a few seconds");
            using (StreamWriter writer = reportFile.CreateText())
            {
                List<string> errors = new List<string>();

                // small packages
                foreach (PingResult result in Pinger.PingHosts(ipAddresses))
                {
                    errors.AddRange(WriteResultAndGetErrors(logger, writer, result));
                }

                writer.WriteLine("");

                // large packages
                foreach (PingResult result in Pinger.PingHosts(ipAddresses, 256))
                {
                    errors.AddRange(WriteResultAndGetErrors(logger, writer, result));
                }

                writer.WriteLine("");
                foreach (string error in errors.Distinct())
                {
                    writer.WriteLine(error);
                }
                
                process.WaitForExit();
                writer.WriteLine(process.StandardOutput.ReadToEnd());
            }
        }

        private List<string> WriteResultAndGetErrors(LoggerInterface logger, StreamWriter writer, PingResult result)
        {
            string pingMessage = result.Errors.Count > 0
                ? String.Format("{0}/{1} pings failed", result.Errors.Count, result.PingAttempts)
                : String.Format("{0} pings", result.PingAttempts);

            writer.WriteLine(String.Format(
                "Host: {0}\t{1} bytes\t{2}\t{3}",
                result.Host,
                result.PayloadSize,
                result.Errors.Count != result.PingAttempts ? result.AveragePing + "ms" : "-----",
                pingMessage
            ));
            
            return result.Errors;
        }
    }
}
