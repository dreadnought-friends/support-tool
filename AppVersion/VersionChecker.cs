using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

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

            string version;
            string url;

            using (Stream stream = client.OpenRead(config.VersionInfoFileUrl))
            {
                StreamReader reader = new StreamReader(stream);
                JObject json = JObject.Parse(reader.ReadToEnd());

                version = (string)json.SelectToken("support-tool.latest");
                url = (string)json.SelectToken("support-tool.releases");
            }

            return new VersionInfo(config.Version.Equals(version), version, url);
        }
    }

    class VersionInfo
    {
        public bool IsUpToDate { get; private set; }
        public string Version { get; private set; }
        public string Url { get; private set; }

        public VersionInfo(bool isUpToDate, string version, string url)
        {
            IsUpToDate = isUpToDate;
            Version = version;
            Url = url;
        }
    }
}
