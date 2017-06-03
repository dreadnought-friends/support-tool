using System;
using System.IO;

namespace SupportTool.Command
{
    class CleanUp : CommandInterface
    {
        public void Execute(Config config, FileAggregator fileAggregator, LoggerInterface logger)
        {
            if (!Directory.Exists(fileAggregator.TempDir))
            {
                return;
            }

            logger.Log("Cleaning up old files in " + fileAggregator.TempDir);

            Directory.Delete(fileAggregator.TempDir, true);
            Directory.CreateDirectory(fileAggregator.TempDir);

            fileAggregator.AggregatedFiles.Clear();
        }
    }
}
