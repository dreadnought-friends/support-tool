using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SupportTool.Command
{
    class DreadnoughtLogs : CommandInterface
    {
        public void Execute(Config config, FileAggregator fileAggregator, LoggerInterface logger)
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
