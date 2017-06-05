using System;
using System.IO;

namespace SupportTool.Command
{
    class CleanUp : CommandInterface
    {
        public void Execute(Config config, FileAggregator fileAggregator, LoggerInterface logger, Propagation propagation)
        {
            if (!Directory.Exists(fileAggregator.TempDir))
            {
                return;
            }

            logger.Log(string.Format("Cleaning up old files in {0}", fileAggregator.TempDir));

            Directory.Delete(fileAggregator.TempDir, true);
            Directory.CreateDirectory(fileAggregator.TempDir);

            fileAggregator.AggregatedFiles.Clear();
        }
    }
}
