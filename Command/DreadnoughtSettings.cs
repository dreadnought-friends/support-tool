using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SupportTool.Command
{
    class DreadnoughtSettings : CommandInterface, CommandCheckBoxInterface
    {
        public string ConfigPropertyPath
        {
            get
            {
                return "IncludeDreadnoughtSettings";
            }
        }

        public string Text
        {
            get
            {
                return "Game settings";
            }
        }

        public string ToolTip
        {
            get
            {
                return "Adds the game settings such as resolution and graphics quality.";
            }
        }

        public void Execute(Config config, FileAggregator fileAggregator, LoggerInterface logger, Propagation propagation)
        {
            logger.Log("Gathering Dreadnought game settings");

            List<FileInfo> files = Directory.GetFiles(config.DreadnoughtSettingsLocation)
                .Select(x => new FileInfo(x))
                .ToList();

            foreach (FileInfo file in files)
            {
                fileAggregator.AddExistingFile(file.FullName, Path.Combine("settings", file.Name));
            }
        }
    }
}
