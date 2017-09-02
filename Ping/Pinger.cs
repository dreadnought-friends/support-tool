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
        public static List<PingResult> PingHosts(List<string> hosts, int bufferSizeInBytes = 32)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(new string('a', bufferSizeInBytes));

            Dictionary<string, List<PingReply>> sortedResults = new Dictionary<string, List<PingReply>>();

            // sort per IP
            foreach (PingReply reply in PingAsync(hosts, buffer))
            {
                List<PingReply> item;
                string host = reply.Address.ToString();
                
                if (!sortedResults.TryGetValue(host, out item))
                {
                    item = new List<PingReply>();
                    sortedResults[host] = item;
                }

                item.Add(reply);
            }

            List<PingResult> pingResults = new List<PingResult>();

            // calculate result per IP based on average
            foreach (KeyValuePair<string, List<PingReply>> item in sortedResults)
            {
                List<string> errors = new List<string>();
                long totalTime = 0;


                foreach (PingReply reply in item.Value)
                {
                    if (reply.Status != IPStatus.Success)
                    {
                        errors.Add(String.Format("Connecting to {0} failed. Error: {1}.", reply.Address, reply.Status.ToString()));
                        continue;
                    }

                    totalTime += reply.RoundtripTime;
                }
                
                int actualPings = item.Value.Count - errors.Count;
                if (actualPings == 0)
                {
                    Console.WriteLine(errors.ToString());
                }

                double average = actualPings > 0 ? totalTime / actualPings : 0;
                pingResults.Add(new PingResult(item.Key, average, bufferSizeInBytes, item.Value.Count, errors));
            }


            return pingResults;
        }

        private static List<PingReply> PingAsync(List<string> hosts, byte[] buffer)
        {
            List<string> repeatedHosts = new List<string>();

            for (int i = 0; i < 4; i++)
            {
                repeatedHosts.AddRange(hosts);
            }

            List<Task<PingReply>> pingTasks = new List<Task<PingReply>>();
            foreach (var address in repeatedHosts)
            {
                pingTasks.Add(PingAsync(address));
            }
            
            Task.WaitAll(pingTasks.ToArray());

            return pingTasks.Select(t => t.Result).ToList();
        }

        private static Task<PingReply> PingAsync(string address)
        {
            var tcs = new TaskCompletionSource<PingReply>();
            var ping = new System.Net.NetworkInformation.Ping();
            ping.PingCompleted += (object sender, PingCompletedEventArgs e) =>
            {
                tcs.SetResult(e.Reply);
            };
            ping.SendAsync(address, new object());
            return tcs.Task;
        }
    }
}
