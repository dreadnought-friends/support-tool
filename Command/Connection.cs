using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;

namespace SupportTool.Command
{
    class Connection : CommandInterface
    {
        private string[] IpPings = new string[] {
            "172.86.100.9",
            "172.86.100.100",
            "172.86.100.101",
            "172.86.100.102",
            "172.86.100.103",
            "172.86.100.104",
            "172.86.100.105",
            "172.86.100.106",
            "172.86.100.107"
        };

        public void Execute(Config config, FileAggregator fileAggregator, LoggerInterface logger, Propagation propagation)
        {
            if (!config.IncludeConnection)
            {
                logger.Log("Skipping connection information to the Dreadnought servers");
                return;
            }

            logger.Log("Generating connection information to the Dreadnought servers, this might take a while");

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
