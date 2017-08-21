using System.IO;
using System.Net;
using System.Xml.Serialization;

namespace SupportTool.AppVersion
{
    class VersionChecker
    {
        private Config config;

        public VersionChecker(Config config)
        {
            this.config = config;
        }

        public VersionInfo getLatestVersionInfo()
        {
            WebClient client = new WebClient();

            string version = "";
            string url = "";
            string motdTitle = "";
            string motdBody = "";

            using (Stream stream = client.OpenRead(config.VersionInfoFileUrl))
            {
                StreamReader reader = new StreamReader(stream);
                XmlSerializer serializer = new XmlSerializer(typeof(Tools));
                Tools tools = (Tools)serializer.Deserialize(reader);

                foreach (ToolsTool tool in tools.Tool)
                {
                    if (!tool.Name.Equals("support-tool"))
                    {
                        continue;
                    }

                    version = tool.Latest;
                    url = tool.ReleasePage;
                    motdTitle = tool.motd.title;
                    motdBody = tool.motd.body;
                    break;
                }
            }

            return new VersionInfo(config.Version.Equals(version), version, url, motdTitle, motdBody);
        }
    }

    class VersionInfo
    {
        public bool IsUpToDate { get; private set; }
        public string Version { get; private set; }
        public string Url { get; private set; }
        public string MotdTitle { get; private set; }
        public string MotdBody { get; private set; }

        public VersionInfo(bool isUpToDate, string version, string url, string motdTitle, string motdBody)
        {
            IsUpToDate = isUpToDate;
            Version = version;
            Url = url;
            MotdTitle = motdTitle;
            MotdBody = motdBody;
        }
    }
}
