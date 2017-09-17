using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SupportTool.Tool.KeyboardSettings
{
    class SettingFinder
    {
        static Regex regex = new Regex("^([a-zA-Z_0-9])+=\\(ActionName=\"(TriggerAbility(One|Two|Three|Four))\",Key=([a-zA-Z_]+),.*\\)$");

        public static Dictionary<string, string> findSettings(FileInfo logFile)
        {
            StreamReader file = new StreamReader(logFile.FullName);
            Dictionary<string, string> settings = new Dictionary<string, string>(4);

            using (FileStream stream = logFile.OpenRead())
            using (StreamReader reader = new StreamReader(stream))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    Tuple<string, string> result = findMatchingSetting(line);

                    if (null != result && !settings.ContainsKey(result.Item1))
                    {
                        settings.Add(result.Item1, result.Item2);
                    }
                }
            }

            return settings;
        }

        /// <summary>
        /// Returns a tuple where the first value is the ActionName and the
        /// second is the selected Key.
        /// 
        /// Returns null if it didn't match
        /// </summary>
        /// <returns></returns>
        public static Tuple<string, string> findMatchingSetting(string line)
        {
            MatchCollection matches = regex.Matches(line);
            if (0 == matches.Count)
            {
                return null;
            }

            string moduleSlot = matches[0].Groups[2].ToString();
            string shortcut = matches[0].Groups[4].ToString();

            return Tuple.Create<string, string>(moduleSlot, shortcut);
        }
    }
}
