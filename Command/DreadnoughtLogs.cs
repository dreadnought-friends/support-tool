using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SupportTool.Command
{
    class DreadnoughtLogs : CommandInterface, CommandCheckBoxInterface
    {
        public string ConfigPropertyPath
        {
            get
            {
                return "IncludeDreadnoughtLogs";
            }
        }

        public string Text
        {
            get
            {
                return "Game logs";
            }
        }

        public string ToolTip { get; }

        public void Execute(Config config, FileAggregator fileAggregator, LoggerInterface logger, Propagation propagation)
        {
            if (!config.IncludeDreadnoughtLogs)
            {
                logger.Log("Skipping Dreadnought logs");
                return;
            }

            logger.Log("Gathering Dreadnought game logs");

            List<FileInfo> files = Directory.GetFiles(config.LogFileLocation)
                .Select(x => new FileInfo(x))
                .OrderByDescending(x => x.LastWriteTime)
                .Take(10)
                .ToList();

            foreach (FileInfo file in files)
            {
                fileAggregator.AddExistingFile(file.FullName, Path.Combine("logs", file.Name));
            }
        }
    }
}
