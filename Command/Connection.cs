using SupportTool.Ping;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace SupportTool.Command
{
    class Connection : CommandInterface, CommandCheckBoxInterface
    {
        private PingStorage PingStorage;

        public Connection(PingStorage pingStorage)
        {
            PingStorage = pingStorage;
        }

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
                return "Pings several game servers and creates a traceroute.";
            }
        }

        public void Execute(Config config, FileAggregator fileAggregator, LoggerInterface logger, Propagation propagation)
        {
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
                logger.Log("Pinging the servers failed, please check your connection.");
                propagation.ShouldStop = true;
            }

            AddStoredPingsAsCsv(config, fileAggregator);
        }

        private void CheckConnnection(List<string> ipAddresses, FileAggregator fileAggregator, LoggerInterface logger)
        {
            FileInfo reportFile = fileAggregator.AddVirtualFile("connection-information.txt");

            // trace route to the first succesful ping in the list
            Process process = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    FileName = "cmd.exe",
                    Arguments = "/C tracert -d -w 60 {0}"
                }
            };

            logger.Log("Pinging known dreadnought servers, this might take a few seconds");

            bool hasStartedTracert = false;

            using (StreamWriter writer = reportFile.CreateText())
            {
                List<string> errors = new List<string>();

                // small packages
                foreach (PingResult result in Pinger.PingHosts(ipAddresses))
                {
                    errors.AddRange(WriteResultAndGetErrors(logger, writer, result));

                    if (!result.Successful || hasStartedTracert)
                    {
                        continue;
                    }

                    process.StartInfo.Arguments = string.Format(process.StartInfo.Arguments, result.Host);
                    process.Start();
                    hasStartedTracert = true;
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
                
                if (hasStartedTracert)
                {
                    process.WaitForExit();
                    writer.WriteLine(process.StandardOutput.ReadToEnd());
                }

                writer.WriteLine("NOTE: 172.86.100.9 is known to have pinging turned off.");
                writer.WriteLine("NOTE: Open ping-log.csv for ping info while the tool was running.");
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

        private void AddStoredPingsAsCsv(Config config, FileAggregator fileAggregator)
        {
            FileInfo reportFile = fileAggregator.AddVirtualFile("ping-log.csv");

            using (StreamWriter writer = reportFile.CreateText())
            {
                writer.WriteLine("Timestamp, AveragePing");

                foreach (var result in PingStorage.Pings)
                {
                    writer.WriteLine(string.Format("{0}, {1}", result.Timestamp, result.AveragePing));
                }
            }
        }
    }
}
