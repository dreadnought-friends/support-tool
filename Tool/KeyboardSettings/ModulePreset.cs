using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SupportTool.Tool.KeyboardSettings
{
    public class ModulePreset
    {
        /// <summary>
        /// Default available presets.
        /// </summary>
        public List<ModulePresetConfiguration> Presets = new List<ModulePresetConfiguration>()
        {
            new ModulePresetConfiguration("Qwerty", "One", "Two", "Three", "Four"),
            new ModulePresetConfiguration("Azerty", "Ampersand", "E_AccentAigu", "Quote", "Apostrophe"),
        };

        /// <summary>
        /// Returns the active configuration.
        /// 
        /// Will be null in the following cases:
        ///  - input.ini does not exist
        ///  - input.ini does not contain the 4 settings
        ///  - found more than 4 things to add
        /// </summary>
        public ModulePresetConfiguration ActiveConfiguration
        {
            get
            {
                if (!InputFile.Exists)
                {
                    return null;
                }

                Dictionary<string, string> settings;

                try
                {
                    settings = SettingFinder.findSettings(InputFile);
                }
                catch (IndexOutOfRangeException)
                {
                    return null;
                }

                if (0 == settings.Count)
                {
                    return null;
                }

                return new ModulePresetConfiguration("Current", settings);
            }
        }
        
        private FileInfo InputFile;

        public ModulePreset(Config config)
        {
            InputFile = new FileInfo(Path.Combine(config.DreadnoughtSettingsLocation, "input.ini"));
        }

        public void savePreset(ModulePresetConfiguration preset)
        {
            Dictionary<string, string> presetDictionary = preset.ToDictionary();
            List<string> replaced = new List<string>(4);

            string[] lines = File.ReadAllLines(InputFile.FullName);

            for(int i = 0; i < lines.Length; i++)
            {
                Tuple<string, string> result = SettingFinder.findMatchingSetting(lines[i]);

                if (null != result && !replaced.Contains(result.Item1))
                {
                    // ensures the setting is replaced only once, not touching the secondary option
                    lines[i] = lines[i].Replace("Key="+result.Item2, "Key="+presetDictionary[result.Item1]);
                    replaced.Add(result.Item1);
                }
            }
            
            using (FileStream stream = InputFile.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                // write back the new configuration
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    foreach (string line in lines)
                    {
                        writer.WriteLine(line);
                    }
                }
            }
        }
    }
}
