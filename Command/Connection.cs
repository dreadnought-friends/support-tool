using SupportTool.Ping;
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

            logger.Log("Generating connection information to the Dreadnought servers, this might take a while");

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

            using (StreamWriter writer = reportFile.CreateText())
            {
                logger.Log("Tracing route to one of the servers");
                writer.WriteLine(string.Format("==== TRACERT: {0} ====", IpPings[0]));
                writer.WriteLine(probeConnection("tracert", IpPings[0]));

                logger.Log("Pinging known dreadnought servers");
                foreach (string ipAddress in IpPings)
                {
                    writer.WriteLine(string.Format("==== PING: {0} ====", ipAddress));
                    writer.WriteLine(probeConnection("ping", ipAddress));
                }
            }
        }

        private static string probeConnection(string command, string host)
        {
            Process process = new Process();
            process.EnableRaisingEvents = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = host;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            process.WaitForExit();
            string output = process.StandardOutput.ReadToEnd();
            process.Close();

            return output;
        }
    }
}
