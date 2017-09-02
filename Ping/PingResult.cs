using System.Collections.Generic;

namespace SupportTool.Ping
{
    class PingResult
    {
        public string Host { get; private set; }
        public bool Successful { get; private set; }
        public double AveragePing { get; private set; }
        public int PayloadSize { get; private set; }
        public int PingAttempts { get; private set; }
        public List<string> Errors { get; private set; }

        public PingResult(string host, double averagePing, int payloadSize, int pingAttempts, List<string> errors)
        {
            Host = host;
            AveragePing = averagePing;
            PayloadSize = payloadSize;
            PingAttempts = pingAttempts;
            Successful = 0 == errors.Count;
            Errors = errors;
        }
    }
}
