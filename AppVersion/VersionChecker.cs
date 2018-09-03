using System.Collections.Generic;
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
            string primaryIp = "";
            string motdTitle = "";
            string motdBody = "";
            Dictionary<string, string> settings = new Dictionary<string, string>();


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
                    primaryIp = tool.primaryIp;
                    motdTitle = tool.motd.title;
                    motdBody = tool.motd.body;

                    foreach (var setting in tool.settings)
                    {
                        settings.Add(setting.key, setting.Value);
                    }

                    break;
                }
            }

            return new VersionInfo(config.Version.Equals(version), version, url, primaryIp, motdTitle, motdBody, settings);
        }
    }

    class VersionInfo
    {
        public bool IsUpToDate { get; private set; }
        public string Version { get; private set; }
        public string Url { get; private set; }
        public string PrimaryIp { get; private set; }
        public string MotdTitle { get; private set; }
        public string MotdBody { get; private set; }
        public Dictionary<string, string> Settings { get; private set; }

        public VersionInfo(bool isUpToDate, string version, string url, string primaryIp, string motdTitle, string motdBody, Dictionary<string, string> settings)
        {
            IsUpToDate = isUpToDate;
            Version = version;
            Url = url;
            PrimaryIp = primaryIp;
            MotdTitle = motdTitle;
            MotdBody = motdBody;
            Settings = settings;
        }
    }

    class Setting
    {
        public static readonly string Enabled = "Enabled";
        public static readonly string Disabled = "Disabled";
    }
}
