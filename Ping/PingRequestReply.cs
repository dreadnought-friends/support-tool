using System.Net.NetworkInformation;

namespace SupportTool.Ping
{
    class PingRequestReply
    {
        public string OriginalHost { get; private set; }
        public PingReply PingReply { get; private set; }

        public PingRequestReply(string originalHost, PingReply pingReply)
        {
            OriginalHost = originalHost;
            PingReply = pingReply;
        }
    }
}
