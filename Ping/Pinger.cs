using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SupportTool.Ping
{
    class Pinger
    {
        public static List<PingResult> PingHosts(string host, int bufferSizeInBytes = 32, int totalPings = 4)
        {
            return PingHosts(new List<string> { host }, bufferSizeInBytes, totalPings);
        }

        public static List<PingResult> PingHosts(List<string> hosts, int bufferSizeInBytes = 32, int totalPings = 4)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(new string('a', bufferSizeInBytes));

            Dictionary<string, List<PingReply>> sortedResults = new Dictionary<string, List<PingReply>>();

            // sort per IP
            foreach (PingRequestReply reply in PingAsync(hosts, buffer, totalPings))
            {
                List<PingReply> item;
                string host = reply.OriginalHost;
                
                if (!sortedResults.TryGetValue(host, out item))
                {
                    item = new List<PingReply>();
                    sortedResults[host] = item;
                }

                item.Add(reply.PingReply);
            }

            List<PingResult> pingResults = new List<PingResult>();

            // calculate result per IP based on average
            foreach (KeyValuePair<string, List<PingReply>> item in sortedResults)
            {
                List<string> errors = new List<string>();
                long totalTime = 0;

                foreach (PingReply reply in item.Value)
                {
                    if (reply.Status == IPStatus.Success)
                    {
                        totalTime += reply.RoundtripTime;
                        continue;
                    }

                    string error = item.Key.Equals(reply.Address.ToString())
                        ? String.Format("Connecting to {0} failed. Error: {2}.", item.Key, reply.Status.ToString())
                        : String.Format("Connecting to {0} (resolved to {1}) failed. Error: {2}.", item.Key, reply.Address, reply.Status.ToString());

                    errors.Add(error);
                }
                
                int actualPings = item.Value.Count - errors.Count;
                double average = actualPings > 0 ? totalTime / actualPings : 0;

                pingResults.Add(new PingResult(item.Key, average, bufferSizeInBytes, item.Value.Count, errors));
            }


            return pingResults;
        }

        private static List<PingRequestReply> PingAsync(List<string> hosts, byte[] buffer, int totalPings = 4)
        {
            List<string> repeatedHosts = new List<string>();

            for (int i = 0; i < totalPings; i++)
            {
                repeatedHosts.AddRange(hosts);
            }

            List<Task<PingRequestReply>> pingTasks = new List<Task<PingRequestReply>>();
            foreach (var address in repeatedHosts)
            {
                pingTasks.Add(PingAsync(address));
            }
            
            Task.WaitAll(pingTasks.ToArray());

            return pingTasks.Select(t => t.Result).ToList();
        }

        private static Task<PingRequestReply> PingAsync(string address)
        {
            var tcs = new TaskCompletionSource<PingRequestReply>();
            var ping = new System.Net.NetworkInformation.Ping();
            ping.PingCompleted += (object sender, PingCompletedEventArgs e) =>
            {
                tcs.SetResult(new PingRequestReply(address, e.Reply));
            };
            ping.SendAsync(address, new object());
            return tcs.Task;
        }
    }
}
