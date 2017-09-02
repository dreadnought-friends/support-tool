using SupportTool.Ping;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SupportTool.Command
{
    class Connection : CommandInterface
    {
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

            List<string> IpPings = ipAddresses.ToList();
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
                    Arguments = "/C tracert -d " + IpPings[0]
                }
            };
            process.Start();

            logger.Log("Pinging known dreadnought servers");
            using (StreamWriter writer = reportFile.CreateText())
            {
                // small packages
                foreach (PingResult result in Pinger.PingHosts(IpPings))
                {
                    writeResult(logger, writer, result);
                }
                writer.WriteLine("");

                // large packages
                foreach (PingResult result in Pinger.PingHosts(IpPings, 256))
                {
                    writeResult(logger, writer, result);
                }

                writer.WriteLine("");
                process.WaitForExit();
                writer.WriteLine(process.StandardOutput.ReadToEnd());

            }
        }

        private void writeResult(LoggerInterface logger, StreamWriter writer, PingResult result)
        {
            writer.WriteLine(String.Format(
                "Host: {0}\t{1} bytes\t{2}\t{3} pings",
                result.Host,
                result.PayloadSize.ToString(),
                result.Errors.Count != result.PingAttempts ? result.AveragePing + "ms" : "-----",
                result.PingAttempts
            ));

            if (result.Errors.Count > 0)
            {
                // ensure a new line for readability
                writer.WriteLine("");
            }

            foreach (string error in result.Errors.Distinct())
            {
                writer.WriteLine(error);
                logger.Log(error);
            }
        }
    }
}
