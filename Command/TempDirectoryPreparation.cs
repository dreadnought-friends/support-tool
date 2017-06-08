using System;
using System.IO;

namespace SupportTool.Command
{
    class TempDirectoryPreparation : CommandInterface
    {
        public void Execute(Config config, FileAggregator fileAggregator, LoggerInterface logger, Propagation propagation)
        {
            if (!Directory.Exists(fileAggregator.TempDir))
            {
                logger.Log(string.Format("Created {0}", fileAggregator.TempDir));
                Directory.CreateDirectory(fileAggregator.TempDir);
                return;
            }

            logger.Log(string.Format("Cleaning up old files in {0}", fileAggregator.TempDir));

            Directory.Delete(fileAggregator.TempDir, true);
            Directory.CreateDirectory(fileAggregator.TempDir);

            fileAggregator.AggregatedFiles.Clear();
        }
    }
}
