using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SupportTool.Ping
{
    class IpFinder
    {
        public static HashSet<string> findIps(FileInfo logFile)
        {
            StreamReader file = new StreamReader(logFile.FullName);
            Regex regex = new Regex(@"(172\.)((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){2}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)");
            HashSet<string> ipAddresses = new HashSet<string>();

            string line;
            while ((line = file.ReadLine()) != null)
            {
                MatchCollection matches = regex.Matches(line);
                if (0 == matches.Count)
                {
                    continue;
                }

                for (int i = 0; i < matches.Count; i++)
                {
                    ipAddresses.Add(matches[i].ToString());
                }
            }

            return ipAddresses;
        }
    }
}
